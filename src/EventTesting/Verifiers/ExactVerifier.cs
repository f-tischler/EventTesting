namespace EventTesting.Verifiers
{
    public class ExactVerifier : IVerifier
    {
        private readonly int _expected;

        public ExactVerifier(int expected)
        {
            _expected = expected;
        }


        public void Verify(IEventHook hook)
        {
            if (TryVerify(hook)) return;

            throw new VerificationException(
                $"Event <{hook.EventName}> was expected to be raised exactly <{_expected}> time(s) but was raised <{hook.Calls}> time(s)");
        }

        public bool TryVerify(IEventHook hook)
        {
            return hook.Calls == _expected;
        }
    }
}
