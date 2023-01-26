// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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


        public void InvalidDataItemAdded(string deviceUuid, IDataItem dataItem, ValidationResult result)
        {
            _logger.LogWarning($"[Agent-Validation] : Validation Failed : {deviceUuid} :  {result.Message}");
        }
    }
}
