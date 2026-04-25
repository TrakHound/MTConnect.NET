// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State of the binding process when Component participates in a task as a collaborator
    /// </summary>
    public enum BindingState
    {
        /// <summary>
        /// Default state when the collaborator is yet to bind to a task
        /// </summary>
        INACTIVE,
        
        /// <summary>
        /// State when a collaborator is ready and expresses interest in all tasks that require its capability
        /// </summary>
        PREPARING,
        
        /// <summary>
        /// State when a collaborator has successfully bound itself to a task
        /// </summary>
        COMMITTED
    }
}