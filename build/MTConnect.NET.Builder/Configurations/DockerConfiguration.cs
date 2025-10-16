// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Builder
{
    internal class DockerConfiguration
    {
        public string ProjectPath { get; set; }
        public string ImageName { get; set; }
        public DockerImageConfiguration[] Images { get; set; }
    }

    internal class DockerImageConfiguration
    {
        public string BaseImage { get; set; }
        public string Tag { get; set; }
        public string Runtime { get; set; }
        public string Framework { get; set; }
    }
}
