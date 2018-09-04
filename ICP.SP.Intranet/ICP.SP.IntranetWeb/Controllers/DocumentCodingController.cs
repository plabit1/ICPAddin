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
    public class DocumentCodingController : ApiController
    {
        private IntranetAppsHelper helper = new IntranetAppsHelper();

        [HttpPost]
        public IHttpActionResult PostDocumentCoding(DocumentCodingModel request)
        {
            var result = new PostEntityModelResponse();
            try
            {
                result.ItemCode = helper.GenerateCode(request);
                if (string.IsNullOrEmpty(result.ItemCode))
                    result.Message = "No code was generated.";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while saving your information.";
            }
            return Ok(result);
        }
    }
}
