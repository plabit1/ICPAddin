using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICP.SP.IntranetWeb.ViewModels;
using Microsoft.SharePoint.Client;
using ICP.SP.Intranet.Helper;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ICP.SP.Intranet.Models.Common;

namespace ICP.SP.IntranetWeb.Controllers
{
    public class MailboxController : Controller
    {
        // GET: Mailbox
        public ActionResult Index()
        {
            return View();
        }

        private IntranetAppsServiceHelper helper = new IntranetAppsServiceHelper();

        public async Task<ActionResult> Incoming(IncomingMailboxViewModel model)
        {
            var result = new IncomingMailboxViewModel();
            if (!string.IsNullOrEmpty(Request.QueryString["SPHostUrl"]))
                result.DBModel.SiteUrl = Request.QueryString["SPHostUrl"];
            if (!string.IsNullOrEmpty(Request.QueryString["SPListId"]))
                result.DBModel.DocumentLibraryId = Request.QueryString["SPListId"];
            if (!string.IsNullOrEmpty(Request.QueryString["SPListItemId"]))
                result.DBModel.ListItemId = Convert.ToInt32(Request.QueryString["SPListItemId"]);
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    ListItem spItem = clientContext.Web.Lists.GetById(new Guid(result.DBModel.DocumentLibraryId)).GetItemById(result.DBModel.ListItemId);
                    clientContext.Load(spItem, item => item.Client_Title, item => item.File);
                    var spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(spUser, user => user.Title, user => user.LoginName);
                    var spWeb = clientContext.Web;
                    clientContext.Load(spWeb, web => web.Title);
                    var spLibrary = clientContext.Web.Lists.GetById(new Guid(result.DBModel.DocumentLibraryId));
                    clientContext.Load(spLibrary, library => library.Title);
                    clientContext.ExecuteQuery();
                    result.DBModel.DocumentTitle = string.IsNullOrEmpty(spItem.File.Title) ? spItem.File.Name : spItem.File.Title;
                    result.DBModel.DocumentUrl = spItem.File.LinkingUrl;
                    result.DBModel.AssignedByLogin = spUser.LoginName;
                    result.DBModel.AssignedByName = spUser.Title;
                    ViewBag.UserName = spUser.Title;
                    ViewBag.WebTitle = spWeb.Title;
                    ViewBag.PreviousPage = clientContext.Url;
                    ViewBag.DocLibraryTitle = spLibrary.Title;
                }
            }
            if (string.IsNullOrEmpty(model.Operation))
            {
                result.DBModel.AssignmentStatus = "Pendiente";
                result.Operation = "Nuevo";
                var tmp = await helper.GetInMailbox(result.DBModel);
                var tmpModel = JsonConvert.DeserializeObject<MailboxDocumentModel>(tmp);
                if (tmpModel.MailboxDocumentId > 0) // first load
                {
                    var tmpTitle = result.DBModel.DocumentTitle;
                    var tmpURL = result.DBModel.DocumentUrl;
                    var tmpAssignedBy = result.DBModel.AssignedByLogin;
                    var tmpAssignedByName = result.DBModel.AssignedByName;
                    result.DBModel = tmpModel;
                    result.DBModel.DocumentTitle = tmpTitle != tmpModel.DocumentTitle ? tmpTitle : tmpModel.DocumentTitle;
                    result.DBModel.DocumentUrl = tmpURL != tmpModel.DocumentUrl ? tmpURL : tmpModel.DocumentUrl;
                    result.DBModel.AssignedByLogin = tmpAssignedBy != tmpModel.AssignedByLogin ? tmpAssignedBy : tmpModel.AssignedByLogin;
                    result.DBModel.AssignedByName = tmpAssignedBy != tmpModel.AssignedByName ? tmpAssignedBy : tmpModel.AssignedByName;
                    result.WarningMessage = "El documento está en proceso. No puede realizar cambios.";
                    result.Operation = "Ver";
                }
                else
                    result.DBModel.DocumentDate = DateTime.Today;
            }
            else // submit form
            {
                try
                {
                    model.DBModel.AssignmentDate = DateTime.Now;
                    model.DBModel.AssignmentStatus = "Pendiente";
                    var tmpAssgnTo = JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(model.DBModel.AssignedToControl);

                   
                    model.DBModel.AssignedToLogin = tmpAssgnTo[0].Login;
                    model.DBModel.AssignedToName = tmpAssgnTo[0].Name;
                    var ccList = new List<string>();
                    var tmpCCLoginStr = string.Empty;
                    var tmpCCNameStr = string.Empty;
                    if (!string.IsNullOrEmpty(model.DBModel.AssignedToCCControl))
                    {
                        var tmpAssgnToCC = JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(model.DBModel.AssignedToCCControl);
                        for (var i = 0; i < tmpAssgnToCC.Count; i++)
                        {
                            ccList.Add(tmpAssgnToCC[i].Email);
                            tmpCCLoginStr += tmpAssgnToCC[i].Login + "/";
                            tmpCCNameStr += tmpAssgnToCC[i].Name + ", ";
                        }
                    }
                    model.DBModel.AssignedToCCLogin = tmpCCLoginStr;
                    model.DBModel.AssignedToCCName = tmpCCNameStr;
                    var tmpSave = await helper.PostInMailbox(model.DBModel);
                    var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><p>Se le ha asignado el documento <a href='{1}'>{2}</a> de <b>Correspondencia Interna</b> en la Intranet.<br/></p><p>Referente a: <br/><span style='font-style:italic'>{3}</span></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", tmpAssgnTo[0].Name, result.DBModel.DocumentUrl, result.DBModel.DocumentSubject, result.DBModel.DocumentSummary);
                    new Utils.EmailHelper().SendEmail("intranetapps@icpower.com", tmpAssgnTo[0].Email, ccList.ToArray(), "Intranet - Correspondencia Entrante", strBody);
                    result.ConfirmMessage = "Los cambios se grabaron con éxito. Se le redirigirá pronto al portal.";
                }
                catch (Exception ex) { result.ErrorMessage = "Ocurrió un error al grabar los cambios. Por favor regrese al portal e intente nuevamente."; }
            }
            return View(result);
        }

        public ActionResult MailboxDetails(MailboxDetailsViewModel inModel)
        {
            var helper = new Models.IntranetAppsDBHelper();
            var result = new MailboxDetailsViewModel();
            try
            {
                result.Details = helper.GetMyMailboxDetails(inModel.DocumentId);
            }
            catch(Exception ex)
            {
                result.ErrorMessage = "Ocurrió un error al obtener los movimientos del documento. Por favor regrese al portal e intente nuevamente.";
            }
            
            return PartialView(result);
        }

        [HttpGet]
        public ActionResult DoIncoming(int documentId)
        {
            var result = new DoIncomingViewModel();
            //var helper = new Models.IntranetAppsDBHelper();
            //var tmp = helper.GetMailboxDocument(documentId);
            //result.AssignedBy = tmp.AssignedBy;
            //result.AssignedTo = tmp.AssignedTo;
            //result.AssignmentDate = tmp.AssignmentDate;
            //result.DocLibrary = tmp.DocLibrary;
            //result.DocumentDate = tmp.DocumentDate;
            //result.DocumentSubject = tmp.DocumentSubject;
            //result.ListItemId = tmp.ListItemId;
            //result.DocumentSummary = tmp.DocumentSummary;
            //result.SiteUrl = tmp.SiteUrl;
            //result.Title = tmp.Title;
            //result.ListItemId = tmp.ListItemId;
            //result.AssignmentId = tmp.AssignmentId;
            //result.DocumentURL = tmp.DocumentURL;
            //result.AssignedToCC = tmp.AssignedToCC;
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Asignar", Value = "Asignar" });
            statusList.Add(new SelectListItem() { Text = "Atender", Value="Atender" });
            ViewBag.StatusList = new SelectList(statusList, "Value","Text");
            return View(result);
        }
        [HttpPost]
        public ActionResult DoIncoming(DoIncomingViewModel document)
        {
            var result = new DoIncomingViewModel();
            
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Asignar", Value = "Asignar" });
            statusList.Add(new SelectListItem() { Text = "Atender", Value = "Atender" });
            ViewBag.StatusList = new SelectList(statusList, "Value", "Text");
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        var spUser = clientContext.Web.CurrentUser;
                        clientContext.Load(spUser, user => user.Title, user => user.LoginName);
                        clientContext.ExecuteQuery();

                        document.AssignedBy = string.Format("{0}/{1}", spUser.LoginName, spUser.Title);
                        ViewBag.UserName = spUser.Title;
                    }
                }

                var helper = new Models.IntranetAppsDBHelper();
                helper.SaveIncomingMailboxUpdate(document);
                
                var tmpAssgnTo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(document.AssignedTo);
                var tmpAssgnToCC = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(document.AssignedToCC);
                var ccList = new List<string>();
                for (var i = 0; i < tmpAssgnToCC.Count; i++)
                    ccList.Add(tmpAssgnToCC[i].Email);
                var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><p>Se le ha asignado el documento <a href='{1}'>{2}</a> de <b>Correspondencia Interna</b> en la Intranet.<br/></p><p>Referente a: <br/><span style='font-style:italic'>{3}</span></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", tmpAssgnTo[0].Name, document.DocumentURL, document.DocumentSubject, document.NewAnnotations);
                new Utils.EmailHelper().SendEmail("intranetapps@icpower.com", tmpAssgnTo[0].Email, ccList.ToArray(), "Intranet - Correspondencia Entrante", strBody);
                result.ConfirmMessage = "Los cambios se grabaron con éxito. Se le redirigirá pronto al portal.";
            }
            catch (Exception ex) { result.ErrorMessage = "Ocurrió un error al grabar los cambios. Por favor regrese al portal e intente nuevamente."; }
            ViewBag.PreviousPage = "https://icpower.sharepoint.com";
            return View(result);
        }

        public ActionResult IncomingList()
        {
            var strLogin = string.Empty;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(spUser, user => user.Title, user => user.LoginName);
                    clientContext.ExecuteQuery();
                    strLogin = spUser.LoginName;
                    ViewBag.UserName = spUser.Title;
                }
            }
            var helper = new Models.IntranetAppsDBHelper();
            var result = new IncomingListViewModel();
            try
            {
                result.MailboxDocumentList = helper.GetMyMailboxDocuments(strLogin);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Ocurrió un error al obtener los movimientos del documento. Por favor regrese al portal e intente nuevamente.";
            }

            return View(result);
        }
    }
}