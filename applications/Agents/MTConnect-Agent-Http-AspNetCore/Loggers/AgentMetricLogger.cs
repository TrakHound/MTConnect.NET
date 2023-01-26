// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
