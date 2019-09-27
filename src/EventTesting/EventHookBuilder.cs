using System;
using System.Linq;
using System.Reflection;

namespace EventTesting
{
    public class EventHookBuilder<T>
    {
        public EventHookBuilder(T target)
        {
            _target = target;
        }

        public InvocationVerificationBuilder<T, EventArgs> Hook(Action<T, EventHandler> subscribeAction)
        {
            var hook = new EventHook<T, EventArgs>();

            var eventName = AddHandler(subscribeAction, hook.HandleEvent);

            hook.SetEventName(eventName);

            return new InvocationVerificationBuilder<T, EventArgs>(hook);
        }

        public InvocationVerificationBuilder<T, TEventArgs> Hook<TEventArgs>(
            Action<T, EventHandler<TEventArgs>> subscribeAction)
        {
            var hook = new EventHook<T, TEventArgs>();

            var eventName = AddHandler(subscribeAction, hook.HandleEvent);

            hook.SetEventName(eventName);

            return new InvocationVerificationBuilder<T, TEventArgs>(hook);
        }

        public EventHook HookOnly(Action<T, EventHandler> subscribeAction)
        {
            return Hook(subscribeAction).Build();
        }

        public EventHook HookOnly<TEventArgs>(Action<T, EventHandler<TEventArgs>> subscribeAction)
        {
            return Hook(subscribeAction).Build();
        }

        private string AddHandler<TEventHandler>(
            Action<T, TEventHandler> subscribeAction,
            TEventHandler handler) where TEventHandler : class
        {
            subscribeAction(_target, handler);

            return GetEventName(handler as Delegate);
        }

        private string GetEventName(Delegate handler)
        {
            try
            {
                var type = _target.GetType();

                var name = GetEventName(handler, type);

                while (name is null)
                {
                    type = type.BaseType;
                    name = GetEventName(handler, type);
                }


                return name;
            }
            catch (Exception)
            {
                throw new Exception("Failed to find event name. Did you use the provided object to subscribe?");
            }
        }

        private string GetEventName(Delegate handler, Type type)
        {
            return type
                .GetEvents()
                // get non-null fields
                .Select(e => (e,
                    type.GetField(e.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)))
                .Where(t => t.Item2 != null)
                // get non-null instance values
                .Select(t => (t.Item1, t.Item2.GetValue(_target) as Delegate))
                .Where(t => t.Item2 != null)
                // get invocation lists
                .Select(t => (t.Item1, t.Item2.GetInvocationList()))
                // get first event which contains the specified handler
                .FirstOrDefault(t => t.Item2.Contains(handler))
                // select the name of the event
                .Item1?.Name;
        }

        private readonly T _target;
    }
}