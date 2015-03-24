using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using BookCollector.Api.Goodreads;
using BookCollector.Controllers;

namespace BookCollector.Api.ImportProvider
{
    [Export(typeof(IImportProvider))]
    public class GoodreadsImporter : IImportProvider
    {
        private readonly IDataController data_controller;

        public string Name { get { return "Goodreads"; } }

        [ImportingConstructor]
        public GoodreadsImporter(IDataController data_controller)
        {
            this.data_controller = data_controller;
        }

        public async Task<List<ImportedBook>> Execute(IProgress<string> status)
        {
            var credentials = await Authenticate();
            var books = await GetBooksAsync(credentials);
            return books;
        }

        private async Task<GoodreadsCredential> Authenticate()
        {
            var credentials = data_controller.GetApiCredential<GoodreadsCredential>(Name);
            if (credentials != null)
                return credentials;

            return new GoodreadsCredential();
        }

        public Task<List<ImportedBook>> GetBooksAsync(GoodreadsCredential credential)
        {
            return null;
        }

        /*
            progress.Report("Requesting authorization token");
            var authorization_response = await Task.Factory.StartNew(() => api.RequestAuthorizationToken(callback_uri.ToString()));
            logger.Trace("Response url: " + authorization_response.Url);

            var tcs = browser.Show();
            await browser.Load(authorization_response.Url, s => Predicate(s, tcs));
            await tcs.Task;

            progress.Report("Requesting access token");
            var access_response = await Task.Factory.StartNew(() => api.RequestAccessToken(authorization_response));
            credentials = new GoodReadsCredentials(access_response);

            progress.Report("Requesting user id");
            credentials.UserId = await Task.Factory.StartNew(() => api.GetUserId(credentials));

            progress.Report("Authorization done!");
            application_settings.AddCredentials(profile.Id, api.Name, credentials);

            return credentials;
        }*/
    }
}
