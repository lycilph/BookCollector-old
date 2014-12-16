using System;
using System.IO;
using System.Text;
using CefSharp;
using NLog;
using Caliburn.Micro;
using RestSharp.Contrib;
using LogManager = NLog.LogManager;

namespace BookCollector.Services.Browsing
{
    public class SchemeHandler : ISchemeHandler
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public bool ProcessRequestAsync(IRequest request, ISchemeHandlerResponse response, OnRequestCompletedHandler request_completed_callback)
        {
            logger.Trace("ProcessRequestAsync (url = {0})", request.Url);

            var uri = new Uri(request.Url);
            
            if (uri.Host.ToLowerInvariant() == "www.bookcollector.com")
            {
                logger.Trace("Sending response (url = {0})", request.Url);

                var sb = new StringBuilder();
                var query_string = HttpUtility.ParseQueryString(uri.Query);
                query_string.AllKeys.Apply(key => sb.AppendLine(string.Format("{0} : {1}<br>", key, query_string[key])));
                
                var html = string.Format("﻿<!DOCTYPE html> <html> <body>{0}</body </html>", sb);
                var bytes = Encoding.UTF8.GetBytes(html);

                response.ResponseStream = new MemoryStream(bytes);
                response.MimeType = "text/html";
                request_completed_callback();

                return true;
            }

            return false;
        }
    }
}
