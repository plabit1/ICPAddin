﻿using ICP.SP.Intranet.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICP.SP.Intranet.Models.Service
{
    public class GetMyPendingDocumentAssignmentResponse
    {
        public string Message { get; set; }
        public List<DocumentAssignmentModel> ModelList { get; set; }
    }
}
