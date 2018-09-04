using ICP.SP.Intranet.Helper;
using ICP.SP.Intranet.Models.Common;
using ICP.SP.IntranetWeb.ViewModels;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICP.SP.IntranetWeb.Controllers
{
    public class AssignedDocsController : Controller
    {
        private IntranetAppsServiceHelper helper = new IntranetAppsServiceHelper();
        public async Task<ActionResult> Index()
        {
            var result = new AssignedDocsViewModel();
            if (!string.IsNullOrEmpty(Request.QueryString["SPLanguage"]))
                result.CurrentCulture = new CultureInfo(Request.QueryString["SPLanguage"]);
            else
                result.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            ViewBag.AppUrl = Request.QueryString["SPAppWebUrl"];
            ViewBag.WebUrl = Request.QueryString["SPHostUrl"];
            var currentLogin = string.Empty;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(spUser, user => user.LoginName);
                    clientContext.ExecuteQuery();
                    currentLogin = spUser.LoginName;
                }
            }
            try
            {
                var tmp = await helper.GetMyPendingDocAssignments(currentLogin);
                result.PendDocsInfoList = JsonConvert.DeserializeObject<List<DocumentAssignmentModel>>(tmp);
                foreach (var item in result.PendDocsInfoList)
                {
                    if (DateTime.Today >= item.DueToDate.AddDays(-1 * item.SecondReminderDays))
                    {
                        item.AssignmentStatus = "Vencido";
                        item.Indicator = "1Red";
                    }
                    else if (DateTime.Today < item.DueToDate.AddDays(-1 * item.SecondReminderDays) && DateTime.Today >= item.DueToDate.AddDays(-1 * item.FirstReminderDays))
                    {
                        item.AssignmentStatus = "Por Vencer";
                        item.Indicator = "2Yellow";
                    }
                    else
                    {
                        item.AssignmentStatus = "Vigente";
                        item.Indicator = "3Green";
                    }
                }
            }
            catch { result.Message = "Ocurrió un error al obtener su información."; }
            return View(result);
        }

       
        public async Task<ActionResult> AssignDocument(AssignDocumentViewModel model)
        {
            var result = new AssignDocumentViewModel();
            if (!string.IsNullOrEmpty(Request.QueryString["SPHostUrl"]))
                result.DBModel.SiteUrl = Request.QueryString["SPHostUrl"];
            if (!string.IsNullOrEmpty(Request.QueryString["SPListId"]))
                result.DBModel.DocumentLibraryId = Request.QueryString["SPListId"];
            if (!string.IsNullOrEmpty(Request.QueryString["SPListItemId"]))
                result.DBModel.ListItemId = Convert.ToInt32(Request.QueryString["SPListItemId"]);
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (!string.IsNullOrEmpty(Request.QueryString["SPLanguage"]))
                currentCulture = new CultureInfo(Request.QueryString["SPLanguage"]);
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
                    if (!string.IsNullOrEmpty(spItem.File.LinkingUrl))
                        result.DBModel.DocumentUrl = spItem.File.LinkingUrl;
                    else
                    {
                        Uri hostUrl = new Uri(result.DBModel.SiteUrl);
                        result.DBModel.DocumentUrl = "https://" + hostUrl.Host + spItem.File.ServerRelativeUrl;
                    }
                    result.DBModel.AssignedByLogin = spUser.LoginName;
                    result.DBModel.AssignedByName = spUser.Title;

                    ViewBag.UserName = spUser.Title;
                    ViewBag.WebTitle = spWeb.Title;
                    ViewBag.PreviousPage = result.DBModel.DocumentUrl.Substring(0, result.DBModel.DocumentUrl.LastIndexOf('/'));
                    ViewBag.DocLibraryTitle = spLibrary.Title;
                }
            }
            if (string.IsNullOrEmpty(model.Operation))
            {
                result.DBModel.AssignmentStatus = "Pendiente";
                result.Operation = "Nuevo";
                var tmp = await helper.GetDocumentAssignment(result.DBModel);
                var tmpModel = JsonConvert.DeserializeObject<DocumentAssignmentModel>(tmp);
                if (tmpModel.DocumentAssignmentId > 0) // first load
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
                    result.WarningMessage = "El documento se encuentra asignado. Si continua va a sobreescribir la asignación actual";
                    result.Operation = "Sobrescribir";
                }
                else
                    result.DBModel.DueToDate = DateTime.Today;
            }
            else
            {
                if (model.DBModel.DocumentAssignmentId > 0) // cerrar actual
                {
                    model.DBModel.AssignmentStatus = "Cerrado";
                    var tmpClose = await helper.PostDocumentAssignment(model.DBModel);
                }
                model.DBModel.DocumentAssignmentId = 0; // genera nuevo registro
                model.DBModel.AssignmentDate = DateTime.Now;
                model.DBModel.AssignmentStatus = "Pendiente";
                var tmpAssgnTo = JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(model.DBModel.AssignedToControl);
                model.DBModel.AssignedToLogin = tmpAssgnTo[0].Login;
                model.DBModel.AssignedToName = tmpAssgnTo[0].Name;
                try
                {
                    UpdateSharePoint(model, spContext, currentCulture);
                    var tmpSave = await helper.PostDocumentAssignment(model.DBModel);
                    var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><p>Se le ha asignado el documento <a href='{1}'>{2}</a> en la Intranet cuyo vencimiento es el <b>{3:dd/MM/yyyy}</b>.<br/>Se le enviará un recordatorio <b>{4}</b> días antes del vencimiento y un segundo recordatorio <b>{5}</b> días antes.<br/></p><p>Atentamente,</p><p><b>Intranet IC Power</b></p>", model.DBModel.AssignedToName, model.DBModel.DocumentUrl, model.DBModel.DocumentTitle, model.DBModel.DueToDate, model.DBModel.FirstReminderDays, model.DBModel.SecondReminderDays);
                    new Utils.EmailHelper().SendEmail("intranetapps@icpower.com", tmpAssgnTo[0].Email, null, "Intranet - Documento Asignado", strBody);
                    result.ConfirmMessage = "Los cambios se grabaron con éxito. Se le redirigirá pronto al portal.";
                }
                catch (Exception ex) { result.ErrorMessage = "Ocurrió un error al grabar los cambios. Por favor regrese al portal e intente nuevamente."; }
                result.DBModel = model.DBModel;
            }
            return View(result);
        }

        private static void UpdateSharePoint(AssignDocumentViewModel model, SharePointContext spContext, CultureInfo currentCulture)
        {
            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    ListItem spItem = clientContext.Web.Lists.GetById(new Guid(model.DBModel.DocumentLibraryId)).GetItemById(model.DBModel.ListItemId);
                    var formValues = new List<ListItemFormUpdateValue>();
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Title", FieldValue = model.DBModel.DocumentTitle });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Subject", FieldValue = model.DBModel.DocumentSubject });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Observaciones", FieldValue = model.DBModel.DocumentAnnotations });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "FechaVencimiento", FieldValue = model.DBModel.DueToDate.ToString("d", currentCulture) });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Responsable", FieldValue = "[{'Key':'" + model.DBModel.AssignedToLogin + "'}]" });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "EstadoDoc", FieldValue = model.DBModel.AssignmentStatus });
                    var result = spItem.ValidateUpdateListItem(formValues, true, string.Empty);
                    clientContext.ExecuteQuery();
                }
            }
        }
    }
}
