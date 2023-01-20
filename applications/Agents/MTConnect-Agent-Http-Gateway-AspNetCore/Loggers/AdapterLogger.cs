// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Logging;
//using MTConnect.Adapters.Shdr;
using System;

namespace MTConnect.Applications.Loggers
{
    public class AdapterLogger
    {
        private readonly ILogger<AdapterLogger> _logger;


        public AdapterLogger(ILogger<AdapterLogger> logger)
        {
            _logger = logger;
        }


        public void AdapterConnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _logger.LogInformation($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        public void AdapterDisconnected(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _logger.LogInformation($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        public void AdapterConnectionError(object sender, Exception exception)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _logger.LogInformation($"[Adapter] : ID = " + adapterClient.Id + " : " + exception.Message);
        }

        public void AdapterPingSent(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _logger.LogInformation($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }

        public void AdapterPongReceived(object sender, string message)
        {
            var adapterClient = (ShdrAdapterClient)sender;
            _logger.LogInformation($"[Adapter] : ID = " + adapterClient.Id + " : " + message);
        }
    }
}
