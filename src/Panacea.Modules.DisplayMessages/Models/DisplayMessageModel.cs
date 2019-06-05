using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.DisplayMessages.Models
{
    [DataContract]
    public class DisplayMessageModel
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "data")]
        public DisplayMessageData Data { get; set; }

    }
}
