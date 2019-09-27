using System;
using EventTesting;
using EventTesting.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class VerifiersTest
    {
        [TestClass]
        public class ExactVerifierTest
        {
            [DataTestMethod]
            [DataRow(0, 0)]
            [DataRow(1, 1)]
            [DataRow(2, 2)]
            public void TestTryVerifySuccess(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new ExactVerifier(expected);

                // ACT & ASSERT
                Assert.IsTrue(verifier.TryVerify(m.Object));
            }

            [DataTestMethod]
            [DataRow(0, 0)]
            [DataRow(1, 1)]
            [DataRow(2, 2)]
            public void TestVerifySuccess(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new ExactVerifier(expected);

                // ACT & ASSERT
                verifier.Verify(m.Object);
            }

            [DataTestMethod]
            [DataRow(0, 1)]
            [DataRow(2, 1)]
            [DataRow(3, 1)]
            public void TestTryVerifyFail(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new ExactVerifier(expected);

                // ACT & ASSERT
                Assert.IsFalse(verifier.TryVerify(m.Object));
            }

            [DataTestMethod]
            [DataRow(0, 1)]
            [DataRow(2, 1)]
            [DataRow(3, 1)]
            public void TestVerifyFail(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new ExactVerifier(expected);

                // ACT & ASSERT
                Assert.ThrowsException<VerificationException>(() => verifier.Verify(m.Object));
            }
        }

        [TestClass]
        public class MinimumVerifierTest
        {
            [DataTestMethod]
            [DataRow(0, 0)]
            [DataRow(0, 1)]
            [DataRow(1, 1)]
            [DataRow(1, 2)]
            [DataRow(1, 3)]
            public void TestTryVerifySuccess(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);
                
                var verifier = new MinimumVerifier(expected);

                // ACT & ASSERT
                Assert.IsTrue(verifier.TryVerify(m.Object));
            }

            [DataTestMethod]
            [DataRow(0, 0)]
            [DataRow(0, 1)]
            [DataRow(1, 1)]
            [DataRow(1, 2)]
            [DataRow(1, 3)]
            public void TestVerifySuccess(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new MinimumVerifier(expected);

                // ACT & ASSERT
                verifier.Verify(m.Object);
            }

            [DataTestMethod]
            [DataRow(1, 0)]
            [DataRow(2, 0)]
            [DataRow(2, 1)]
            [DataRow(3, 0)]
            public void TestTryVerifyFail(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new MinimumVerifier(expected);

                // ACT & ASSERT
                Assert.IsFalse(verifier.TryVerify(m.Object));
            }

            [DataTestMethod]
            [DataRow(1, 0)]
            [DataRow(2, 0)]
            [DataRow(2, 1)]
            [DataRow(3, 0)]
            public void TestVerifyFail(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new MinimumVerifier(expected);

                // ACT & ASSERT
                Assert.ThrowsException<VerificationException>(() => verifier.Verify(m.Object));
            }
        }

        [TestClass]
        public class MaximumVerifierTest
        {
            [DataTestMethod]
            [DataRow(1, 0)]
            [DataRow(2, 0)]
            [DataRow(2, 1)]
            [DataRow(3, 0)]
            public void TestTryVerifySuccess(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new MaximumVerifier(expected);

                // ACT & ASSERT
                Assert.IsTrue(verifier.TryVerify(m.Object));
            }

            [DataTestMethod]
            [DataRow(1, 0)]
            [DataRow(2, 0)]
            [DataRow(2, 1)]
            [DataRow(3, 0)]
            public void TestVerifySuccess(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new MaximumVerifier(expected);

                // ACT & ASSERT
                verifier.Verify(m.Object);
            }

            [DataTestMethod]
            [DataRow(0, 1)]
            [DataRow(1, 2)]
            [DataRow(1, 3)]
            public void TestTryVerifyFail(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new MaximumVerifier(expected);

                // ACT & ASSERT
                Assert.IsFalse(verifier.TryVerify(m.Object));
            }

            [DataTestMethod]
            [DataRow(0, 1)]
            [DataRow(1, 2)]
            [DataRow(1, 3)]
            public void TestVerifyFail(int expected, int actual)
            {
                // ARRANGE
                var m = new Moq.Mock<IEventHook>();

                m.SetupGet(o => o.Calls).Returns(actual);

                var verifier = new MaximumVerifier(expected);

                // ACT & ASSERT
                Assert.ThrowsException<VerificationException>(() => verifier.Verify(m.Object));
            }
        }
    }
}
