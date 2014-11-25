using System;

namespace BookCollector.Services.Authentication
{
    public abstract class AuthenticatorBase : IAuthenticator
    {
        public abstract void Start();

        public virtual void Navigating(Uri uri) { }

        public virtual void Navigated(Uri uri) { }

        public virtual void Loaded(string html) { }
    }
}
