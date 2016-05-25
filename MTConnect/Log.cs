// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect
{
    public class Log
    {
        public static bool Enabled { get; set; }

        internal static void Write(string s)
        {
            if (Enabled)
            {
                Console.WriteLine(s);
            }
        }
    }
}
