// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Clients.Rest;

namespace MTConnect.Client.Http.Sample
{
    class ConsoleWriteStream : MTConnectHttpStream
    {
        public ConsoleWriteStream(string url) : base (url) { }

        protected override void ProcessResponseBody(byte[] responseBytes, string contentEncoding = null)
        {
            var response = System.Text.Encoding.UTF8.GetString(responseBytes);

            Console.WriteLine(response);
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
