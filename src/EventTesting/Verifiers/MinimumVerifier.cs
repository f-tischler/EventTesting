namespace EventTesting.Verifiers
{
    public class MinimumVerifier : IVerifier
    {
        private readonly int _minimum;

        public MinimumVerifier(int minimum)
        {
            _minimum = minimum;
        }


        public void Verify(IEventHook hook)
        {
            if (TryVerify(hook)) return;

            throw new VerificationException($"Event <{hook.EventName}> was expected to be raised at least <{_minimum}> time(s) but was raised <{hook.Calls}> time(s)");

        }

        public bool TryVerify(IEventHook hook)
        {
            return hook.Calls >= _minimum;
        }
    }
}
