using System;
using System.Threading.Tasks;
using BookCollector.Services.Browsing;

namespace BookCollector.Apis.Audible
{
    public static class AudibleImportHelper
    {
        public static async Task<string> GetSignInName()
        {
            var value = await BrowserController.EvaluateOffscreen("document.getElementById('anon_header_v2_signin') == null");
            var is_signed_in = Convert.ToBoolean(value.Result);

            if (!is_signed_in)
                return string.Empty;

            var result = await BrowserController.EvaluateOffscreen("document.getElementById('mast-member-acct-name').getAttribute('alt')");
            var str = (string)result.Result;
            return str.Replace("Hi,", "").Replace("!", "").Trim();
        }
    }
}
