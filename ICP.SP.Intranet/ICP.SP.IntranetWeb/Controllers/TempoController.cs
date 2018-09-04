using ICP.SP.Intranet.BusinessLogic;
using ICP.SP.Intranet.Models.Common;
using ICP.SP.Intranet.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ICP.SP.IntranetWeb.Controllers
{
    public class TempoController : ApiController
    {
        private IntranetAppsHelper helper = new IntranetAppsHelper();
        public IHttpActionResult PostIncomingMailbox(MailboxDocumentModel document)
        {
            var result = new PostEntityModelResponse();
            try
            {
                result.ItemId = helper.SaveIncomingMailbox(document);
                if (result.ItemId <= 0)
                    result.Message = "No records were saved.";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while saving your information.";
            }
            return Ok(result);
        }
    }
}
