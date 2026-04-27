// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect
{
    /// <summary>
    /// Centralises the MqttRelay agent module shutdown policy. The
    /// MqttRelay module previously called <c>_documentServer.Stop()</c>
    /// unconditionally from <c>OnStop()</c>; when configured for
    /// <c>TopicStructure=Entity</c> only <c>_entityServer</c> is
    /// constructed, so the document-server reference was <c>null</c>
    /// and shutdown raised a <see cref="NullReferenceException"/>.
    /// Encapsulating the policy here makes the null-guard unit-testable
    /// without instantiating an MTConnect agent broker, and keeps the
    /// per-server stop independent so a throw on one path cannot leave
    /// the other server running.
    /// </summary>
    internal static class MqttRelayLifecycle
    {
        /// <summary>
        /// Invokes the supplied per-server stop actions, tolerating
        /// either or both being <c>null</c>. A throw from one action
        /// does not prevent the other from running.
        /// </summary>
        /// <param name="documentStop">
        /// Optional action stopping the Document-mode server.
        /// </param>
        /// <param name="entityStop">
        /// Optional action stopping the Entity-mode server.
        /// </param>
        public static void StopServers(Action documentStop, Action entityStop)
        {
            if (documentStop != null)
            {
                try { documentStop(); } catch { }
            }

            if (entityStop != null)
            {
                try { entityStop(); } catch { }
            }
        }
    }
}
