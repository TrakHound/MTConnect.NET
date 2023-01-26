// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using System;

namespace MTConnect.Applications.Loggers
{
    public class ApplicationLogger
    {
        private readonly ILogger<ApplicationLogger> _logger;


        public ApplicationLogger(ILogger<ApplicationLogger> logger)
        {
            _logger = logger;
        }


        public void ApplicationException(Exception ex)
        {
            _logger.LogCritical($"[Application] : {ex.Message}");
        }
    }
}
