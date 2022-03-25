// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
