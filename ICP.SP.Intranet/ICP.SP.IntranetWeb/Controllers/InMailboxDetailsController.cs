using ICP.SP.Intranet.BusinessLogic;
using ICP.SP.Intranet.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ICP.SP.IntranetWeb.Controllers
{
    public class InMailboxDetailsController : ApiController
    {
        private IntranetAppsHelper helper = new IntranetAppsHelper();
        public IHttpActionResult GetInMailboxDetails(int id)
        {
            var result = new GetIncomingMailboxDetailsResponse();
            if (id <= 0)
            {
                result.Message = "Missing parameters to complete this operation";
                return Ok(result);
            }
            try
            {
                result.ModelList = helper.GetIncomingMailboxDetailsById(id);
                if (result.ModelList.Count <= 0)
                    result.Message = "No pending records found";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return Ok(result);
        }
    }
}
