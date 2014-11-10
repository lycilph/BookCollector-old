using System;

namespace BookCollector.Services.Authentication
{
    public interface IAuthenticator
    {
        void Start();
        void Handle(Uri uri);
    }
}