# Event Testing [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Build status](https://ci.appveyor.com/api/projects/status/0wckkllo1i5n8c49?svg=true)](https://ci.appveyor.com/project/f-tischler/eventtesting) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=f-tischler_EventTesting&metric=alert_status)](https://sonarcloud.io/dashboard?id=f-tischler_EventTesting)

Writing test code for event-driven APIs in C# involves a lot of boiler plate code which obfuscates tests. This library intents to alleviate this problem by provding a fluent programming model for verifying event invocations and validating event arguments. It also supports use cases where events may be fired asynchonously with some delay by allowing to specify a timeout for the invocation verification.

# Installation

[![NuGet: EventTesting](https://img.shields.io/nuget/v/ftischler.EventTesting?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/ftischler.EventTesting/)

# Instructions

Verification is based on hooks wich are created by subscribing to the event to be tested:

```cs
using EventTesting;

var obj = new TestObject();

var hook = EventHook.For(obj)
    .HookOnly((o, h) => o.OnTest += h);
``` 

To assert that the event has been raised, use `Verify`:

```cs
using EventTesting;

var obj = new TestObject();

var hook = EventHook.For(obj)
    .HookOnly((o, h) => o.OnTest += h);

obj.InvokeEvent();
obj.InvokeEvent();

hook.Verify(Called.Twice());
```

Verification is implemented using the `EventTesting.IVerifier` interface and can be extended with custom verifiers. For the most common use cases there are implementations in the `EventTesting.Verifiers` namespace. 

The class `EventTesting.Called` provides a simplified interface for creating verifiers to build a more fluent API.

In case you have multiple events being fired within a call, a list of event arguments `EventHook<T, TEventArgs>.CallsEventArgs` is saved.

```cs
var hook = EventHook.For(obj)
    .HookOnly<TestEventArgs>((o, h) => o.OnTest += h) as EventHook<TestObject, TestEventArgs>;
    // or .Hook<TestEventArgs>((o, h) => o.OnTest += h).Build() as EventHook<TestObject, TestEventArgs>

o.InvokeComplexCustomArgEvent(new TestEventArgs("event #99"));
o.InvokeComplexCustomArgEvent(new TestEventArgs("event #0"));

Assert.AreEqual(2, hook.CallsEventArgs.Count);
Assert.AreEqual("event #99", hook.CallsEventArgs[0].Arg);
Assert.AreEqual("event #0", hook.CallsEventArgs[1].Arg);
```

## Argument Verification

To test arguments passed to event handlers, verification actions can be registered e.g. to assert that the sender is not null:

```cs
using EventTesting;

var obj = new TestObject();

var hook = EventHook.For(obj)
    .Hook((o, h) => o.OnTest += h);
    .Verify((s, e) => Assert.NotNull(s)) // assert
    .Build();

obj.InvokeEvent();
```

## Asynchronous Events

Sometimes the event under test is raised asynchronously by another thread/process which may involve some delay. In this case it may be desirable to have some kind of timeout to give the service time to raise the event (e.g. when testing SignalR events). This can be achieved with the following:

```cs
using EventTesting;

var obj = new TestObject();
var hook = EventTesting.EventHook.For(obj)
    .HookOnly((o, h) => o.OnTest += h);

// raise asynchronously
var t = Task.Run(() =>
{
    Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
    obj.InvokeEvent();
});

// Use extension method Within() to add a timeout
hook.Verify(Called.Once().Within(TimeSpan.FromSeconds(1)));

// join task
t.Wait();
```
### Argument Verification for asynchronous Events

When verifying arguments of asynchronously raised events it is often useful to wait for the event to be raised to ensure that the verification action is executed. `WaitForCall` does just that:

```cs
using EventTesting;

var obj = new TestObject();
var hook = EventTesting.EventHook.For(obj)
    .Hook((o, h) => o.OnTest += h)
    .Verify((s, e) => Assert.NotNull(e)) // assert
    .Build();

// Trigger invocation inside EnsureCalled() to ensure that the invocation is picked up
// EnsureCalled() will block until the event is raised at least once
hook.EnsureCalled(() => 
{
    // trigger
    Task.Run(() =>
    {
        Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
        o.InvokeEvent();
    });
});
```

## Examples

### Minimum Raise Count

```cs
using EventTesting;

var obj = new TestObject();

var hook = EventHook.For(obj)
    .HookOnly((o, h) => o.OnTest += h);

obj.InvokeEvent();
obj.InvokeEvent();

hook.Verify(Called.AtLeast(1));
```
### Maximum Raise Count

```cs
using EventTesting;

var obj = new TestObject();

var hook = EventHook.For(obj)
    .HookOnly((o, h) => o.OnTest += h);

obj.InvokeEvent();
obj.InvokeEvent();

hook.Verify(Called.AtMost(2));
```
