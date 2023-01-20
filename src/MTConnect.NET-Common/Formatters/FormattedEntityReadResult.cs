// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public struct FormattedEntityReadResult<T>
    {
        public T Entity { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public IEnumerable<string> Warnings { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public long ResponseDuration { get; set; }


        public FormattedEntityReadResult(T entity, bool success = true, IEnumerable<string> messages = null, IEnumerable<string> warnings = null, IEnumerable<string> errors = null)
        {
            Entity = entity;
            Success = success;
            Messages = messages;
            Warnings = warnings;
            Errors = errors;
            ResponseDuration = 0;
        }


        public static FormattedEntityReadResult<T> Successful(T entity, string message = null)
        {
            var messages = new List<string>();
            if (!string.IsNullOrEmpty(message)) messages = new List<string> { message };

            return new FormattedEntityReadResult<T>(entity, true, messages);
        }

        public static FormattedEntityReadResult<T> Successful(T entity, IEnumerable<string> messages)
        {
            return new FormattedEntityReadResult<T>(entity, true, messages);
        }


        public static FormattedEntityReadResult<T> Warning(T entity, string warning = null)
        {
            var warnings = new List<string>();
            if (!string.IsNullOrEmpty(warning)) warnings = new List<string> { warning };

            return new FormattedEntityReadResult<T>(entity, true, null, warnings);
        }

        public static FormattedEntityReadResult<T> Warning(T entity, IEnumerable<string> warnings)
        {
            return new FormattedEntityReadResult<T>(entity, true, null, warnings);
        }


        public static FormattedEntityReadResult<T> Error(T entity, string error = null)
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(error)) errors = new List<string> { error };

            return new FormattedEntityReadResult<T>(entity, false, null, null, errors);
        }

        public static FormattedEntityReadResult<T> Error(T entity, IEnumerable<string> errors)
        {
            return new FormattedEntityReadResult<T>(entity, false, null, null, errors);
        }

        public static FormattedEntityReadResult<T> Error(IEnumerable<string> errors = null)
        {
            return new FormattedEntityReadResult<T>(default, false, null, null, errors);
        }
    }
}
