using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Common
{
    public class OutDocumentFilterModel
    {
        public string SearchText { get; set; }
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
    }
}
