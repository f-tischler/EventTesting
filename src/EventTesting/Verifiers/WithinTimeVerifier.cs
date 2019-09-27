using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventTesting
{
    public class WithinTimeVerifier : IVerifier
    {
        private readonly IVerifier _verifier;
        private readonly TimeSpan _span;

        public WithinTimeVerifier(IVerifier verifier, TimeSpan span)
        {
            _verifier = verifier;
            _span = span;
        }

        public void Verify(IEventHook hook)
        {
            Task.Delay(_span).Wait();

            _verifier.Verify(hook);
        }

        public bool TryVerify(IEventHook hook)
        {
            return _verifier.TryVerify(hook);
        }
    }
}
