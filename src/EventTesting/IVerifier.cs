namespace EventTesting
{
    public interface IVerifier
    {
        void Verify(IEventHook hook);

        bool TryVerify(IEventHook hook);
    }
}