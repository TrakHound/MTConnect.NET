// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Logging;

namespace MTConnect.Applications.Loggers
{
    public class AgentValidationLogger
    {
        private readonly ILogger<AgentValidationLogger> _logger;


        public AgentValidationLogger(ILogger<AgentValidationLogger> logger)
        {
            _logger = logger;
        }


        public void InvalidDataItemAdded(Devices.DataItem dataItem, Observations.Observation observation)
        {
            _logger.LogWarning($"[Agent-Validation] : Validation Failed for {dataItem.Id} : \'{observation.Value}\' is Invalid for DataItem of Type \'{dataItem.Type}\'");
        }
    }
}
