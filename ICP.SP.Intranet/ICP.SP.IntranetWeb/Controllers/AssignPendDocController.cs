using ICP.SP.IntranetWeb.ViewModels;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICP.SP.IntranetWeb.Controllers
{
    public class AssignPendDocController : Controller
    {
        // GET: AssignPendDoc
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Assign(PendingDocumentViewModel pendingDoc)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["SPHostUrl"]))
                pendingDoc.SiteUrl = Request.QueryString["SPHostUrl"];
            if (!string.IsNullOrEmpty(Request.QueryString["SPListId"]))
                pendingDoc.DocLibrary = Request.QueryString["SPListId"];
            if (!string.IsNullOrEmpty(Request.QueryString["SPListItemId"]))
                pendingDoc.ListItemId = Convert.ToInt32(Request.QueryString["SPListItemId"]);
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    ListItem spItem = clientContext.Web.Lists.GetById(new Guid(pendingDoc.DocLibrary)).GetItemById(pendingDoc.ListItemId);
                    clientContext.Load(spItem, item => item.Client_Title, item => item.File);
                    var spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(spUser, user => user.Title, user => user.LoginName);
                    var spWeb = clientContext.Web;
                    clientContext.Load(spWeb, web => web.Title);
                    var spLibrary = clientContext.Web.Lists.GetById(new Guid(pendingDoc.DocLibrary));
                    clientContext.Load(spLibrary, library => library.Title);
                    clientContext.ExecuteQuery();
                    pendingDoc.Title = string.IsNullOrEmpty(spItem.File.Title) ? spItem.File.Name : spItem.File.Title;
                    pendingDoc.DocumentURL = spItem.File.LinkingUrl;

                    pendingDoc.AssignedBy = string.Format("{0}/{1}", spUser.LoginName, spUser.Title);
                    ViewBag.UserName = spUser.Title;
                    ViewBag.WebTitle = spWeb.Title;
                    ViewBag.PreviousPage = clientContext.Url;
                    ViewBag.DocLibraryTitle = spLibrary.Title;
                }
            }
            pendingDoc.AssignmentDate = DateTime.Now;
            var helper = new Models.IntranetAppsDBHelper();
            if (pendingDoc.AssignmentId == 0) // first page load
            {
                var tmp = helper.GetDocAssignment(pendingDoc);
                if (tmp.AssignmentId == 0)
                {
                    pendingDoc.DueTo = DateTime.Today;
                    pendingDoc.IsModified = false;
                    pendingDoc = helper.SaveDocAssignment(pendingDoc);
                }
                else
                {
                    var tmpTitle = pendingDoc.Title;
                    var tmpURL = pendingDoc.DocumentURL;
                    var tmpAssignedBy = pendingDoc.AssignedBy;
                    pendingDoc = tmp;
                    pendingDoc.Title = tmpTitle != tmp.Title ? tmpTitle : tmp.Title;
                    pendingDoc.DocumentURL = tmpURL != tmp.DocumentURL ? tmpURL : tmp.DocumentURL;
                    pendingDoc.AssignedBy = tmpAssignedBy != tmp.AssignedBy ? tmpAssignedBy : tmp.AssignedBy;
                    pendingDoc.WarningMessage = "El documento se encuentra asignado. Si continua va a sobreescribir la asignación actual";
                }
            }
            else // submit form
            {
                pendingDoc.IsModified = true;
                try
                {
                    pendingDoc = helper.SaveDocAssignment(pendingDoc);
                    var tmpAssgnTo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(pendingDoc.AssignedTo);
                    var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><br/><p>Se le ha asignado el documento <a href='{1}'>{2}</a> en la Intranet cuyo vencimiento es el <b>{3:dd/MM/yyyy}</b>.<br/></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", tmpAssgnTo[0].Name, pendingDoc.DocumentURL, pendingDoc.Title, pendingDoc.DueTo);
                    new Utils.EmailHelper().SendEmail("intranetapps@icpower.com", tmpAssgnTo[0].Email, null, "Intranet - Documento Asignado",strBody);
                    pendingDoc.ConfirmMessage = "Los cambios se grabaron con éxito. Se le redirigirá pronto al portal.";
                }
                catch (Exception ex) { pendingDoc.ErrorMessage = "Ocurrió un error al grabar los cambios. Por favor regrese al portal e intente nuevamente."; }
            }

            return View(pendingDoc);
        }

        
    }
}