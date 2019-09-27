namespace EventTesting.Verifiers
{
    public class MaximumVerifier : IVerifier
    {
        private readonly int _maximum;

        public MaximumVerifier(int maximum)
        {
            _maximum = maximum;
        }


        public void Verify(IEventHook hook)
        {
            if (TryVerify(hook)) return;

            throw new VerificationException(
                $"Event <{hook.EventName}> was expected to be raised at most <{_maximum}> time(s) but was raised <{hook.Calls}> time(s)");
        }

        public bool TryVerify(IEventHook hook)
        {
            return hook.Calls <= _maximum;
        }
    }
}
