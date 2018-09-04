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
    public class InMailboxController : ApiController
    {
        private IntranetAppsHelper helper = new IntranetAppsHelper();
        //public IHttpActionResult GetIncomingMailbox([FromUri]GetDocumentAssignmentRequest document)
        public IHttpActionResult GetIncomingMailbox(int docId)
        {
            /* var result = new GetIncomingMailboxResponse();
             if (document.DocumentAssignmentId <= 0 && string.IsNullOrEmpty(document.SiteUrl) && string.IsNullOrEmpty(document.DocumentLibraryId) && document.ListItemId <= 0)
             {
                 result.Message = "Missing parameters to complete this operation";
                 return Ok(result);
             }
             try
             {
                 if (document.DocumentAssignmentId > 0)
                     result.Model = helper.GetIncomingMailboxById(document.DocumentAssignmentId);
                 else
                     result.Model = helper.GetIncomingMailboxByLocation(document.SiteUrl, document.DocumentLibraryId, document.ListItemId, document.DocumentStatus);
                 if (result.Model.MailboxDocumentId <= 0)
                     result.Message = "Record not found";
             }
             catch
             {
                 result.Message = "An error occured while retrieving your information.";
             }
             return Ok(result);*/
            var result = new GetIncomingMailboxResponse();
            try
            {
                    result.Model = helper.GetIncomingDocument(docId);
                
                if (result.Model.IncomingMailboxId <= 0)
                    result.Message = "Record not found";
            }
            catch
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return Ok(result);
        }

        public GetMyPendingIncomingMailboxResponse GetMyPendingInMailbox(string accountName)
        {
            var result = new GetMyPendingIncomingMailboxResponse();
            if (string.IsNullOrEmpty(accountName))
            {
                result.Message = "Missing parameters to complete this operation";
                return result;
            }
            try
            {
                result.ModelList = helper.GetMyPendingIncomingMailbox(accountName);
                if (result.ModelList.Count <= 0)
                    result.Message = "No pending records found";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return result;
        }

        

        //public IHttpActionResult PostInMailbox(MailboxDocumentModel document)
        //{
        //    var result = new PostEntityModelResponse();
        //    try
        //    {
        //        result.ItemId = helper.SaveIncomingMailbox(document);
        //        if (result.ItemId <= 0)
        //            result.Message = "No records were saved.";
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = "An error occured while saving your information.";
        //    }
        //    return Ok(result);
        //}

        public IHttpActionResult PostInMailbox(InboxDocumentModel document)
        {
            var result = new PostEntityModelResponse();
            try
            {
                result.ItemId = helper.SaveInboxDocument(document);
                if (result.ItemId <= 0)
                    result.Message = "No records were saved.";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while saving your information.";
            }
            return Ok(result);
        }

        public GetMyPendingIncomingMailboxResponse GetSearchInMailbox(string comp, string text, string from, string to)
        {
            var result = new GetMyPendingIncomingMailboxResponse();
            var tmpFilter = new OutDocumentFilterModel();
            tmpFilter.SearchText = text;
            if (!string.IsNullOrEmpty(from))
                tmpFilter.SearchDateFrom = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to))
                tmpFilter.SearchDateTo = Convert.ToDateTime(to);
            try
            {
                result.ModelList = helper.GetIncomingMailbox(tmpFilter);
                if (result.ModelList.Count <= 0)
                    result.Message = "No pending records found";
            }
            catch (Exception ex)
            {
                result.Message = "An error occured while retrieving your information.";
            }
            return result;
        }
    }
}
