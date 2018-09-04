using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Common
{
    public class CreateEventModel
    {
        public string AccessToken { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }        
        public string RoomEmail { get; set; }
    }
}
