// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
