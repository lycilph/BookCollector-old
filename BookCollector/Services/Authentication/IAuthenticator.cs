using System;

namespace BookCollector.Services.Authentication
{
    public interface IAuthenticator
    {
        void Start();
        void Navigating(Uri uri);
        void Navigated(Uri uri);
        void Loaded(string html);
    }
}