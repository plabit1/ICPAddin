using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICP.SP.Intranet.Models.Common;

namespace ICP.SP.Intranet.Models.Service
{
    public class GetIncomingMailboxResponse
    {
        public string Message { get; set; }
        public InboxDocumentModel Model { get; set; }
    }
}
