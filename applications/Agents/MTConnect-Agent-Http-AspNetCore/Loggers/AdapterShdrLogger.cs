﻿// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using MTConnect.Adapters.Shdr;

namespace MTConnect.Applications.Loggers
{
    public class AdapterShdrLogger
    {
        private readonly ILogger<AdapterShdrLogger> _logger;


        public AdapterShdrLogger(ILogger<AdapterShdrLogger> logger)
        {
            _logger = logger;
        }


        public void AdapterProtocolReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _logger.LogDebug($"[Adapter-SHDR] : ID = " + adapterClient.Id + " : " + message);
        }
    }
}
