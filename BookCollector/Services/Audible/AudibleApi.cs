﻿using System.ComponentModel.Composition;
using NLog;

namespace BookCollector.Services.Audible
{
    [Export(typeof(AudibleApi))]
    public class AudibleApi : ApiBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public string BaseUrl 
        {
            get { return @"http://Audible.com"; }
        }

        public override bool IsAuthenticated
        {
            get { return false; }
        }

        public AudibleApi() : base("Audible")
        {
        }
    }
}