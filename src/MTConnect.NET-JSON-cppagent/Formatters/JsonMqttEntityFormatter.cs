// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Formatters
{
    public class JsonMqttEntityFormatter : JsonHttpEntityFormatter
    {
        public override string Id => "JSON-cppagent-mqtt";
    }
}