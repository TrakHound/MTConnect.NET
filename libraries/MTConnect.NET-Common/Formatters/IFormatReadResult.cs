// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public interface IFormatReadResult
    {
        Type ContentType { get; }

        bool Success { get; }

        IEnumerable<string> Messages { get; }

        IEnumerable<string> Warnings { get; }

        IEnumerable<string> Errors { get; }

        double ResponseDuration { get; }
    }
}