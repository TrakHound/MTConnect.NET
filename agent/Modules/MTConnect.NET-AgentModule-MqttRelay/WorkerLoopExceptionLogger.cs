// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace MTConnect
{
    /// <summary>
    /// Encodes the MqttRelay Worker outer-catch logging policy: skip
    /// orderly-shutdown cancelation signals
    /// (<see cref="OperationCanceledException"/> and its
    /// <see cref="System.Threading.Tasks.TaskCanceledException"/>
    /// subclass), and log everything else with type and message so the
    /// operator diagnoses the defect from log scrapes. Centralising the
    /// policy keeps Module.Worker's outer catch a single delegating
    /// call and lets the policy be unit-tested in isolation.
    /// </summary>
    internal static class WorkerLoopExceptionLogger
    {
        /// <summary>
        /// Routes <paramref name="exception"/> to <paramref name="onLog"/>
        /// unless the exception is a cancelation signal
        /// (<see cref="OperationCanceledException"/> or its
        /// <see cref="TaskCanceledException"/> subclass), in which case
        /// the policy is silence (orderly shutdown).
        /// </summary>
        public static void Log(Exception exception, Action<string> onLog)
        {
            if (exception == null) return;
            if (onLog == null) return;
            if (exception is OperationCanceledException) return;

            onLog($"MQTT Relay Worker unexpected error : {exception.GetType().Name} : {exception.Message}");
        }
    }
}
