namespace EventTesting
{
    public interface IEventHook
    {
        string EventName { get; }

        int Calls { get; }
    }
}