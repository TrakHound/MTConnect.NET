// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Clients
{
    public class SequenceRange
    {
        public SequenceRange(long from, long to)
        {
            From = from;
            To = to;
        }

        public long From { get; set; }

        public long To { get; set; }

        public long Count
        {
            get { return To - From; }
        }

        public void Reset()
        {
            From = 0;
            To = 0;
        }

        public override string ToString()
        {
            string f = "From = {0}, To = {1}, Count = {2}";
            return string.Format(f, From, To, Count); 
        }
    }
}
