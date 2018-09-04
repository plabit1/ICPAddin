using ICP.SP.Intranet.BusinessLogic;
using ICP.SP.Intranet.Models.Common;
using ICP.SP.Intranet.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace ICP.SP.IntranetWeb.Controllers
{
    public class OutMailboxController : ApiController
    {
        private IntranetAppsHelper helper = new IntranetAppsHelper();

        public GetMyPendingOutcomingMailboxResponse GetMyPendingOutMailbox(string comp, string text, string from, string to)
        {
            var result = new GetMyPendingOutcomingMailboxResponse();            
            try
            {
                var tmpFilter = new OutDocumentFilterModel();
                tmpFilter.SearchText = text;
                if (!string.IsNullOrEmpty(from))
                    tmpFilter.SearchDateFrom = Convert.ToDateTime(from);
                if (!string.IsNullOrEmpty(to))
                    tmpFilter.SearchDateTo = Convert.ToDateTime(to);
                result.ModelList = helper.GetMyPendingOutcomingMailbox(tmpFilter);
                if (result.ModelList.Count <= 0)
                    result.Message = "No pending records found";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return result;
        }

        public PostEntityModelResponse GetGenerateCode(string comp, string dept, int year)
        {
            var result = new PostEntityModelResponse();
           
            try
            {
                var tmp = new DocumentCodingModel();
                tmp.CompanyName = comp;
                tmp.DepartmentName = dept;
                tmp.RequestedDate = new DateTime(year, 1, 1);
                result.ItemCode = helper.GenerateCode(tmp);
                
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return result;
        }

        public IHttpActionResult PostOutMailbox(OutDocumentModel document)
        {
            var result = new PostEntityModelResponse();
            try
            {
                result.ItemId = helper.SaveOutcomingMailbox(document);
                if (result.ItemId <= 0)
                    result.Message = "No records were saved.";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while saving your information.";
            }
            return Ok(result);
        }

        public IHttpActionResult GetOutcomingMailbox(int DocumentId)
        {
            var result = new GetOutcomingMailboxResponse();
            try
            {
                result.Model = helper.GetOutcomingMailboxById(DocumentId);

                if (result.Model.DocumentId <= 0)
                    result.Message = "Record not found";
            }
            catch
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return Ok(result);
        }
    }
}
