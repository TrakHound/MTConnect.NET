// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class BindingStateDescriptions
    {
        /// <summary>
        /// Default state when the collaborator is yet to bind to a task
        /// </summary>
        public const string INACTIVE = "Default state when the collaborator is yet to bind to a task";
        
        /// <summary>
        /// State when a collaborator is ready and expresses interest in all tasks that require its capability
        /// </summary>
        public const string PREPARING = "State when a collaborator is ready and expresses interest in all tasks that require its capability";
        
        /// <summary>
        /// State when a collaborator has successfully bound itself to a task
        /// </summary>
        public const string COMMITTED = "State when a collaborator has successfully bound itself to a task";


        public static string Get(BindingState value)
        {
            switch (value)
            {
                case BindingState.INACTIVE: return "Default state when the collaborator is yet to bind to a task";
                case BindingState.PREPARING: return "State when a collaborator is ready and expresses interest in all tasks that require its capability";
                case BindingState.COMMITTED: return "State when a collaborator has successfully bound itself to a task";
            }

            return null;
        }
    }
}