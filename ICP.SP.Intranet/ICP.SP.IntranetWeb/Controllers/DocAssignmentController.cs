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
    public class DocAssignmentController : ApiController
    {
        private IntranetAppsHelper helper = new IntranetAppsHelper();
        public IHttpActionResult GetDocumentAssignment([FromUri]GetDocumentAssignmentRequest document)
        {
            var result = new GetDocumentAssignmentResponse();
            if (document.DocumentAssignmentId <= 0 && string.IsNullOrEmpty(document.SiteUrl) && string.IsNullOrEmpty(document.DocumentLibraryId) && document.ListItemId <= 0)
            {
                result.Message = "Missing parameters to complete this operation";
                return Ok(result);
            }
            try
            {
                if (document.DocumentAssignmentId > 0)
                    result.Model = helper.GetDocumentAssignmentById(document.DocumentAssignmentId);
                else
                    result.Model = helper.GetDocumentAssignmentByLocation(document.SiteUrl, document.DocumentLibraryId, document.ListItemId, document.DocumentStatus);
                if (result.Model.DocumentAssignmentId <= 0)
                    result.Message = "Record not found";
            }
            catch
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return Ok(result);
        }

        public IHttpActionResult GetMyPendingDocumentAssignment(string accountName)
        {
            var result = new GetMyPendingDocumentAssignmentResponse();
            if (string.IsNullOrEmpty(accountName))
            {
                result.Message = "Missing parameters to complete this operation";
                return Ok(result);
            }
            try
            {
                result.ModelList = helper.GetMyPendingDocumentAssigment(accountName);
                if (result.ModelList.Count <= 0)
                    result.Message = "No pending records found";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return Ok(result);
        }

        public IHttpActionResult PostDocAssignment(DocumentAssignmentModel document)
        {
            var result = new PostEntityModelResponse();
            try
            {
                result.ItemId = helper.SaveDocumentAssignment(document);
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
