using System.Collections.Generic;

namespace EventTesting
{
    public interface IEventHook
    {
        string EventName { get; }

        int Calls { get; }
    }

    public interface IEventHook<T, TEventArgs>
    {
        List<TEventArgs> CallsEventArgs { get; }
    }
}