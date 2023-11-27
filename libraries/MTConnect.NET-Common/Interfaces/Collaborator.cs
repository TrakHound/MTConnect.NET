// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    public class Collaborator
    {
        public string CollaboratorId { get; set; }

        public CollaboratorType CollaboratorType { get; set; }

        public bool Optional { get; set; }

        public Priority Priority { get; set; }
    }
}
