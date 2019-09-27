using System;

namespace EventTesting
{
    public class InvocationVerificationBuilder<T, TEventArgs>
    {
        internal InvocationVerificationBuilder(EventHook<T, TEventArgs> hook)
        {
            _hook = hook;
        }

        public InvocationVerificationBuilder<T, TEventArgs> Verify(Action<T, TEventArgs> validationAction)
        {
            _hook.AddVerification(validationAction);

            return this;
        }

        public InvocationVerificationBuilder<T, TEventArgs> Verify(Action<TEventArgs> validationAction)
        {
            _hook.AddVerification((_, param) => validationAction(param));

            return this;
        }

        public EventHook Build()
        {
            return _hook;
        }

        private readonly EventHook<T, TEventArgs> _hook;
    }
}