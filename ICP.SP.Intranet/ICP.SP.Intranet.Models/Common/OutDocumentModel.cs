using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Common
{
    public class OutDocumentModel
    {
        public string SiteUrl { get; set; }
        public string DocumentTitle { get; set; }
        public string RequestedByLogin { get; set; }
        public string RequestedByName { get; set; }
        public string RequestedByEmail { get; set; }
        public DateTime RequestDate { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string RequestStatus { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DeliveryExpectedDate { get; set; }
        public string Indicator { get; set; }
        public int DocumentId { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentFilename { get; set; }
        public string ReceiptFilename { get; set; }
        public string DocumentFolder { get; set; }
        public string DestinationCompany { get; set; }
    }
}
