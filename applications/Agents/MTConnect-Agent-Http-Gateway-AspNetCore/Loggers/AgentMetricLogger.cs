// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Extensions.Logging;

namespace MTConnect.Applications.Loggers
{
    public class AgentMetricLogger
    {
        private readonly ILogger<AgentMetricLogger> _logger;


        public AgentMetricLogger(ILogger<AgentMetricLogger> logger)
        {
            _logger = logger;
        }


        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
