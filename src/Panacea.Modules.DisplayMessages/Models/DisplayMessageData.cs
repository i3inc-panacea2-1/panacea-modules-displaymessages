using Panacea.Multilinguality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.DisplayMessages.Models
{
    [DataContract]
    public class DisplayMessageData : Translatable
    {
        [IsTranslatable]
        [DataMember(Name = "text")]
        public string Text
        {
            get => GetTranslation();
            set => SetTranslation(value);
        }

        [DataMember(Name = "services")]
        public List<string> Services { get; set; }

        [DataMember(Name = "fileUrl")]
        public string FileUrl { get; set; }

        [DataMember(Name = "allowCloseAfter")]
        public int AllowCloseAfter { get; set; }

        [DataMember(Name = "maxDisplayTime")]
        public int MaxDisplayTime { get; set; }

        [DataMember(Name = "startInFullScreen")]
        public bool StartInFullScreen { get; set; }

        [DataMember(Name = "allowClose")]
        public bool AllowClose { get; set; }
    }
}
