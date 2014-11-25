﻿using System;
using BookCollector.Services.Audible;
using BookCollector.Services.Goodreads;
using BookCollector.Services.GoogleBooks;

namespace BookCollector.Services.Authentication
{
    public static class AuthenticatorFactory
    {
        public static IAuthenticator Create(IApi api, IProgress<string> progress, IAuthenticationHandler handler)
        {
            if (api is GoodreadsApi)
                return new GoodreadsAuthenticator(api as GoodreadsApi, progress, handler);

            if (api is GoogleBooksApi)
                return new GoogleBooksAuthenticator(api as GoogleBooksApi, progress, handler);

            if (api is AudibleApi)
                return new AudibleAuthenticator(api as AudibleApi, handler);

            throw new Exception("Unknown api");
        }
    }
}