# Event Testing [![Build status](https://ci.appveyor.com/api/projects/status/0wckkllo1i5n8c49?svg=true)](https://ci.appveyor.com/project/f-tischler/eventtesting) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

Writing test code for event-driven APIs in C# involves a lot of boiler plate code which obfuscates tests. This library intents to alleviate this problem by provding a fluent programming model for verifying event invocations and valdidating event arguments. It also supports use cases where events may be fired asynchonously with some delay by allowing to specify a timeout for the invocation verification.

# Installation

[![NuGet: EventTesting](https://img.shields.io/nuget/v/EventTesting?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/EventTesting/)

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

## Argument Verification

To test arguments passed to event handlers, verification actions can be registered e.g. to assert that the sender is not null:

```cs
using EventTesting;

var obj = new TestObject();

var hook = EventHook.For(obj)
    .Hook((o, h) => o.OnTest += h);
    .Verify((s, e) => Assert.NotNull(s))
    .Build();

obj.InvokeEvent();
```
Here we assert that the event sender is not null.

## Examples

### Minimum raise count

```cs
using EventTesting;

var obj = new TestObject();

var hook = EventHook.For(obj)
    .HookOnly((o, h) => o.OnTest += h);

obj.InvokeEvent();
obj.InvokeEvent();

hook.Verify(Called.AtLeast(1));
```
### Maximum raise count

```cs
using EventTesting;

var obj = new TestObject();

var hook = EventHook.For(obj)
    .HookOnly((o, h) => o.OnTest += h);

obj.InvokeEvent();
obj.InvokeEvent();

hook.Verify(Called.AtMost(2));
```
## Asnychronous Events

TODO
