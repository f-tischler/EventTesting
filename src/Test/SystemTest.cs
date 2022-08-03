using System;
using System.Threading.Tasks;
using EventTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class SystemTest
    {
        class TestObject
        {
            public event EventHandler OnTest;
            public event EventHandler<bool> OnCustomArgsTest;
            public event EventHandler<TestEventArgs> OnComplexCustomArgsTest;

            public void InvokeEvent()
            {
                Assert.IsNotNull(OnTest);
                OnTest.Invoke(this, EventArgs.Empty);
            }

            public void InvokeCustomArgEvent()
            {
                Assert.IsNotNull(OnCustomArgsTest);
                OnCustomArgsTest.Invoke(this, true);
            }

            public void InvokeComplexCustomArgEvent(TestEventArgs testEventArgs)
            {
                Assert.IsNotNull(OnComplexCustomArgsTest);
                OnComplexCustomArgsTest.Invoke(this, testEventArgs);
            }
        }

        private class TestEventArgs : EventArgs
        {
            public string Arg { get; }

            public TestEventArgs(string arg)
            {
                Arg = arg;
            }
        }


        [TestMethod]
        public void TestNoInvocation()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            Assert.AreEqual(0, hook.Calls);
        }

        [TestMethod]
        public void TestHookCustomEventArgsEventHandler()
        {
            var o = new TestObject();
            var hook = EventHook.For(o)
                .HookOnly<bool>((obj, m) => obj.OnCustomArgsTest += m);

            Assert.AreEqual(0, hook.Calls);
        }

        [TestMethod]
        public void TestHookCustomEventArgsEventHandlerBuild()
        {
            var o = new TestObject();
            var hook = EventHook.For(o)
                .Hook<bool>((obj, m) => obj.OnCustomArgsTest += m)
                .Build();

            Assert.AreEqual(0, hook.Calls);
        }

        [TestMethod]
        public void TestOneInvocation()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            o.InvokeEvent();

            Assert.AreEqual(1, hook.Calls);
        }

        [TestMethod]
        public void TestMultipleInvocationWithComplexCustomArgs()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook<TestEventArgs>((obj, m) => obj.OnComplexCustomArgsTest += m)
                .Build() as EventHook<TestObject, TestEventArgs>;

            o.InvokeComplexCustomArgEvent(new TestEventArgs("event #99"));
            o.InvokeComplexCustomArgEvent(new TestEventArgs("event #0"));

            Assert.AreEqual(2, hook.Calls);
            Assert.AreEqual(2, hook.CallsEventArgs.Count);
            Assert.AreEqual("event #99", hook.CallsEventArgs[0].Arg);
            Assert.AreEqual("event #0", hook.CallsEventArgs[1].Arg);
        }

        [TestMethod]
        [ExpectedException(typeof(VerificationException))]
        public void TestVerifyOnceActuallyZero()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            hook.Verify(Called.Once());
        }


        [TestMethod]
        [ExpectedException(typeof(VerificationException))]
        public void TestVerifyOnceActuallyTwice()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            o.InvokeEvent();
            o.InvokeEvent();

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void TestVerifyOnce()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            o.InvokeEvent();

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void TestVerifyOnceWithin()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            var t = Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
                o.InvokeEvent();
            });

            hook.Verify(Called.Once().Within(TimeSpan.FromSeconds(1)));

            t.Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(VerificationException))]
        public void TestVerifyTwiceActuallyZero()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            hook.Verify(Called.Twice());
        }

        [TestMethod]
        [ExpectedException(typeof(VerificationException))]
        public void TestVerifyTwiceActuallyOnce()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            o.InvokeEvent();

            hook.Verify(Called.Twice());
        }

        [TestMethod]
        public void TestVerifyParameters()
        {
            // ARRANGE
            var verificationExecuted = false;

            var o = new TestObject();
            EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Verify((sender, args) => { verificationExecuted = true; });

            // ACT
            o.InvokeEvent();

            // ASSERT
            Assert.IsTrue(verificationExecuted);
        }

        [TestMethod]
        public void TestVerifyParametersFail()
        {
            // ARRANGE
            var o = new TestObject();
            EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Verify((sender, args) => Assert.Fail());

            // ACT & ASSERT
            Assert.ThrowsException<VerificationException>(() => o.InvokeEvent());
        }

        [TestMethod]
        public void TestLastVerifyParametersFail()
        {
            // ARRANGE
            var o = new TestObject();
            EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Verify(args => {  })
                .Verify(args => Assert.Fail());

            // ACT & ASSERT
            Assert.ThrowsException<VerificationException>(() => o.InvokeEvent());
        }

        [TestMethod]
        public async Task TestWaitForCall()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            await hook.WaitForCall(() =>
            {
                o.InvokeEvent();
            });

            Assert.AreEqual(1, hook.Calls);
        }

        [TestMethod]
        public async Task TestWaitForCallExternalRaise()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            await hook.WaitForCall(() =>
            { 
                Task.Run(() =>
                {
                    Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
                    o.InvokeEvent();
                });
            });

            Assert.AreEqual(1, hook.Calls);
        }

        [TestMethod]
        public async Task TestWaitForCallAsync()
        {
            var o = new TestObject();
            var hook = EventTesting.EventHook.For(o)
                .Hook((obj, m) => obj.OnTest += m)
                .Build();

            await hook.WaitForCall(async () =>
            {
                await Task.Delay(500);
                o.InvokeEvent();
            });

            Assert.AreEqual(1, hook.Calls);
        }

        [TestMethod]
        public void TestReset()
        {
            // ARRANGE
            var o = new TestObject();
            var hook = EventHook.For(o)
                .HookOnly((obj, m) => obj.OnTest += m);

            o.InvokeEvent();

            // ACT
            hook.Reset();

            // ACT & ASSERT
            Assert.AreEqual(0, hook.Calls);
        }
    }
}
