using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventTesting
{
    public class EventHook : IEventHook
    {
        public int Calls { get; protected set; }

        public string EventName { get; protected set; }

        public void Verify(IVerifier verifier)
        {
            verifier.Verify(this);
        }

        public async Task WaitForCall(Action invocationAction)
        {
            var currentCalls = Calls;

            invocationAction();

            while (Calls == currentCalls)
                await Task.Delay(TimeSpan.FromMilliseconds(50));
        }

        public async Task WaitForCall(Func<Task> invocationAction)
        {
            var currentCalls = Calls;

            await invocationAction();

            while (Calls == currentCalls)
                await Task.Delay(TimeSpan.FromMilliseconds(50));
        }

        public virtual void Reset()
        {
            Calls = 0;
        }

        public static EventHookBuilder<T> For<T>(T target)
        {
            return new EventHookBuilder<T>(target);
        }
    }

    public class EventHook<T, TEventArgs> : EventHook, IEventHook<T, TEventArgs>
    {
        public List<TEventArgs> CallsEventArgs { get; protected set; } = new List<TEventArgs>();

        public override void Reset()
        {
            CallsEventArgs.Clear();
            base.Reset();
        }

        internal void SetEventName(string eventName)
        {
            EventName = eventName;
        }

        internal void AddVerification(Action<T, TEventArgs> validator)
        {
            _verification.Add(validator);
        }

        internal void HandleEvent(object o, TEventArgs e)
        {
            ++Calls;
            CallsEventArgs.Add(e);

            var i = 0;

            try
            {
                foreach (var validator in _verification)
                {
                    ++i;

                    validator((T) o, e);
                }
            }
            catch (Exception ex)
            {
                throw new VerificationException($"Verification nr. <{i}> for event <{EventName}> failed at call nr. <{Calls}>", ex);
            }
        }

        private readonly List<Action<T, TEventArgs>> _verification = new List<Action<T, TEventArgs>>();
    }
}