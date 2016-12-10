// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect
{

    public delegate void StreamResponse_Handler(string responseString);
    public delegate void Error_Handler(Error error);

    public delegate void Connection_Handler();

    public enum MTC_Stream_Status
    {
        Stopped = 0,
        Started = 1
    }
    
}
