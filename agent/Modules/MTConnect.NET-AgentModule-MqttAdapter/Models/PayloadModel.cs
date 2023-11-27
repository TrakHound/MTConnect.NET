using ProtoBuf;
using System.Collections.Generic;

namespace MTConnect
{
    [ProtoContract]
    public class PayloadModel
    {
        [ProtoMember(1)]
        public IEnumerable<DataModel> DataModels { get; set; }
    }
}
