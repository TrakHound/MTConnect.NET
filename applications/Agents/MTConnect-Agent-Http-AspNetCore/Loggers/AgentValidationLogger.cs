// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Logging;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;

namespace MTConnect.Applications.Loggers
{
    public class AgentValidationLogger
    {
        private readonly ILogger<AgentValidationLogger> _logger;


        public AgentValidationLogger(ILogger<AgentValidationLogger> logger)
        {
            _logger = logger;
        }


        public void InvalidDataItemAdded(IDataItem dataItem, ValidationResult result)
        {
            _logger.LogWarning($"[Agent-Validation] : Validation Failed :  {result.Message}");
        }
    }
}
