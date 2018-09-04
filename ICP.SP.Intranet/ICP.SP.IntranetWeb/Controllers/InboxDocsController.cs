using ICP.SP.Intranet.Helper;
using ICP.SP.Intranet.Models.Common;
using ICP.SP.IntranetWeb.Utils;
using ICP.SP.IntranetWeb.ViewModels;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICP.SP.IntranetWeb.Controllers
{

    public class InboxDocsController : Controller
    {
        private IntranetAppsServiceHelper helper = new IntranetAppsServiceHelper();
        log4net.ILog log = log4net.LogManager.GetLogger("Test");
        public async Task<ActionResult> Index()
        {
            var result = new InboxDocsViewModel();
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
                var tmp = await helper.GetMyPendingInMailbox(currentLogin);
                result.MailboxDocumentList = JsonConvert.DeserializeObject<List<InboxDocumentModel>>(tmp);
                foreach (var item in result.MailboxDocumentList)
                {
                    if (item.DocumentStatus == "Atendido")
                        item.Indicator = "4Gray";
                    else
                    {
                        if (DateTime.Today >= item.SecondReminderDate)
                        {
                            item.DocumentStatus = "Vencido";
                            item.Indicator = "1Red";
                        }
                        else if (DateTime.Today < item.SecondReminderDate && DateTime.Today >= item.FirstReminderDate)
                        {
                            item.DocumentStatus = "Por Vencer";
                            item.Indicator = "2Yellow";
                        }
                        else
                        {
                            item.DocumentStatus = "Vigente";
                            item.Indicator = "3Green";
                        }
                    }
                }
                result.MailboxDocumentList = result.MailboxDocumentList.OrderBy(x => x.Indicator).ToList();
            }
            catch { result.Message = "Ocurrió un error al obtener su información."; }
            return View(result);
        }

        public async Task<ActionResult> AssignIncoming(IncomingMailboxViewModel model)
        {
            var result = new IncomingMailboxViewModel();
           /* if (!string.IsNullOrEmpty(Request.QueryString["SPHostUrl"]))
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
                {
                    result.DBModel.DocumentDate = DateTime.Today;
                    result.DBModel.ResponseDate = DateTime.Today;
                }
            }
            else // submit form
            {
                try
                {
                    model.DBModel.AssignmentDate = DateTime.Now;
                    model.DBModel.AssignmentStatus = "Pendiente";
                    var difDays = model.DBModel.ResponseDate - DateTime.Today;
                    if (difDays.Days > 7)
                    {
                        var calcDays = Math.Ceiling(difDays.Days * 0.6);
                        model.DBModel.FirstReminderDate = DateTime.Today.AddDays(calcDays);
                        calcDays = Math.Ceiling(difDays.Days * 0.8);
                        model.DBModel.SecondReminderDate = DateTime.Today.AddDays(calcDays);
                    }
                    else
                    {
                        model.DBModel.FirstReminderDate = DateTime.Today;
                        var calcDays = Math.Ceiling(difDays.Days * 0.1);
                        model.DBModel.SecondReminderDate = DateTime.Today.AddDays(calcDays);
                    }
                    var tmpAssgnTo = JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(model.DBModel.AssignedToControl);
                    if (tmpAssgnTo.Count > 0)
                    {
                        model.DBModel.AssignedToLogin = tmpAssgnTo[0].Login;
                        model.DBModel.AssignedToName = tmpAssgnTo[0].Name;
                    }
                    var ccList = new List<string>();
                    var tmpAssgnBy = JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(model.DBModel.AssignedByControl);
                    if (tmpAssgnBy.Count > 0)
                        ccList.Add(tmpAssgnBy[0].Email);
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

                    UpdateSharePoint(model, spContext, currentCulture);
                    var tmpSave = await helper.PostInMailbox(model.DBModel);
                    var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><p>Se le ha asignado el documento <b>{2}</b> de <b>Correspondencia Interna</b> en la Intranet.<br/></p><p>Referente a: <br/><span style='font-style:italic'>{3}</span></p><p>- Para ver el documento haga  <a href='{1}'>clic aquí</a><br/>- Para ingresar al portal haga <a href='https://icpower.sharepoint.com'>clic aquí</a></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", tmpAssgnTo[0].Name, model.DBModel.DocumentUrl, model.DBModel.DocumentSubject, model.DBModel.DocumentSummary);
                    new Utils.EmailHelper().SendEmail("intranetapps@icpower.com", tmpAssgnTo[0].Email, ccList.ToArray(), "Intranet - Correspondencia Entrante", strBody);
                    result.ConfirmMessage = "Los cambios se grabaron con éxito. Se le redirigirá pronto al portal.";
                }
                catch (Exception ex) { result.ErrorMessage = "Ocurrió un error al grabar los cambios. Por favor regrese al portal e intente nuevamente."; }
            }*/
            return View(result);
        }

        private static string UpdateSharePoint(IncomingMailboxViewModel model, SharePointContext spContext, CultureInfo currentCulture)
        {
            var result = new System.Text.StringBuilder();
            if (model.DBModel.SiteUrl.ToLower() == spContext.SPHostUrl.AbsoluteUri.ToLower())
            {
                using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        ListItem spItem = clientContext.Web.Lists.GetById(new Guid(model.DBModel.DocumentLibraryId)).GetItemById(model.DBModel.ListItemId);
                        var formValues = new List<ListItemFormUpdateValue>();
                        formValues.Add(new ListItemFormUpdateValue() { FieldName = "Title", FieldValue = model.DBModel.DocumentTitle });
                        formValues.Add(new ListItemFormUpdateValue() { FieldName = "Subject", FieldValue = model.DBModel.DocumentSubject });
                        formValues.Add(new ListItemFormUpdateValue() { FieldName = "Observaciones", FieldValue = model.DBModel.DocumentSummary });
                        formValues.Add(new ListItemFormUpdateValue() { FieldName = "Remitente", FieldValue = model.DBModel.DocumentFrom });
                        formValues.Add(new ListItemFormUpdateValue() { FieldName = "FechaDoc", FieldValue = model.DBModel.DocumentDate.ToString("d", currentCulture) });
                        formValues.Add(new ListItemFormUpdateValue() { FieldName = "FechaVencimiento", FieldValue = model.DBModel.ResponseDate.ToString("d", currentCulture) });
                        formValues.Add(new ListItemFormUpdateValue() { FieldName = "EstadoDoc", FieldValue = model.DBModel.AssignmentStatus });
                        formValues.Add(new ListItemFormUpdateValue() { FieldName = "Destinatario", FieldValue = "[{'Key':'" + model.DBModel.AssignedToLogin + "'}]" });
                        var resUpdate = spItem.ValidateUpdateListItem(formValues, true, string.Empty);
                        clientContext.ExecuteQuery();

                        foreach (ListItemFormUpdateValue updValue in resUpdate)
                            if (updValue.HasException)
                                result.AppendFormat("Campo: {0}, Mensaje: {1}; ", updValue.FieldName, updValue.ErrorMessage);
                    }
                }
            }
            else
            {
                //generate destination context
                Uri destinationSiteUri = new Uri(model.DBModel.SiteUrl);

                //target realm of the tenant (This is constant per tenant)
                string targetRealm = TokenHelper.GetRealmFromTargetUrl(destinationSiteUri);
                //generate access token for destination site
                string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, destinationSiteUri.Authority, targetRealm).AccessToken;
                using (var destinationContext = TokenHelper.GetClientContextWithAccessToken(model.DBModel.SiteUrl, accessToken))
                {
                    ListItem spItem = destinationContext.Web.Lists.GetById(new Guid(model.DBModel.DocumentLibraryId)).GetItemById(model.DBModel.ListItemId);
                    var formValues = new List<ListItemFormUpdateValue>();
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Title", FieldValue = model.DBModel.DocumentTitle });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Subject", FieldValue = model.DBModel.DocumentSubject });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Observaciones", FieldValue = model.DBModel.DocumentSummary });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Remitente", FieldValue = model.DBModel.DocumentFrom });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "FechaDoc", FieldValue = model.DBModel.DocumentDate.ToString("d", currentCulture) });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "FechaVencimiento", FieldValue = model.DBModel.ResponseDate.ToString("d", currentCulture) });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "EstadoDoc", FieldValue = model.DBModel.AssignmentStatus });
                    formValues.Add(new ListItemFormUpdateValue() { FieldName = "Destinatario", FieldValue = "[{'Key':'" + model.DBModel.AssignedToLogin + "'}]" });
                    var resUpdate = spItem.ValidateUpdateListItem(formValues, true, string.Empty);
                    destinationContext.ExecuteQuery();

                    foreach (ListItemFormUpdateValue updValue in resUpdate)
                        if (updValue.HasException)
                            result.AppendFormat("Campo: {0}, Mensaje: {1}; ", updValue.FieldName, updValue.ErrorMessage);
                }
            }
            return result.ToString();
        }

        public async Task<ActionResult> IncomingHistoryDetails(MailboxDetailsViewModel inModel)
        {
            var result = new MailboxDetailsViewModel();
            try
            {
                var tmp = await helper.GetInMailboxDetails(inModel.DocumentId);

                result.Details = JsonConvert.DeserializeObject<List<MailboxDocumentDetailsModel>>(tmp);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Ocurrió un error al obtener los movimientos del documento. Por favor regrese al portal e intente nuevamente.";
            }
            return PartialView(result);
        }

        [HttpGet]
        public async Task<ActionResult> DoIncomingAssignment(int documentId)
        {
            var result = new IncomingMailboxViewModel();
            /*var tmp = await helper.GetInMailbox(new MailboxDocumentModel() { MailboxDocumentId = documentId });

            var tmpModel = JsonConvert.DeserializeObject<MailboxDocumentModel>(tmp);
            if (tmpModel.MailboxDocumentId > 0)
                result.DBModel = tmpModel;
            else
                result.ErrorMessage = "No se pudo obtener información.";
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Asignar", Value = "Asignado" });
            statusList.Add(new SelectListItem() { Text = "Atender", Value = "Atendido" });
            ViewBag.StatusList = new SelectList(statusList, "Value", "Text");
            if (tmpModel.AssignmentStatus == "Atendido")
                result.WarningMessage = "Este documento ha sido atendido.";
            else if (!tmpModel.AssignedToLogin.Contains(User.Identity.Name.ToLower()))
                result.WarningMessage = "Usted no es el responsable de este documento, puede visualizar la información pero no modificarla.";
            ViewBag.PreviousPage = Request.QueryString["SPHostUrl"];*/
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> DoIncomingAssignment(IncomingMailboxViewModel model)
        {
            var result = new IncomingMailboxViewModel();
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Asignar", Value = "Asignado" });
            statusList.Add(new SelectListItem() { Text = "Atender", Value = "Atendido" });
            ViewBag.StatusList = new SelectList(statusList, "Value", "Text");
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (!string.IsNullOrEmpty(Request.QueryString["SPLanguage"]))
                currentCulture = new CultureInfo(Request.QueryString["SPLanguage"]);
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

                        model.DBModel.AssignedByLogin = spUser.LoginName;
                        model.DBModel.AssignedByName = spUser.Title;
                        ViewBag.UserName = spUser.Title;
                    }
                }
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
                model.DBModel.AssignmentDate = DateTime.Now;

                ViewBag.TraceTxt = UpdateSharePoint(model, spContext, currentCulture);

                var tmpSave = await helper.PostInMailbox(model.DBModel);
                var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><p>Se le ha asignado el documento <b>{2}</b> de <b>Correspondencia Entrante</b> en la Intranet.<br/></p><p>Tomar en cuenta lo siguiente: <br/><span style='font-style:italic'>{3}</span></p><p>- Para ver el documento haga  <a href='{1}'>clic aquí</a><br/>- Para ingresar al portal haga <a href='https://icpower.sharepoint.com'>clic aquí</a></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", tmpAssgnTo[0].Name, model.DBModel.DocumentUrl, model.DBModel.DocumentSubject, model.DBModel.Annotations);
                if (model.DBModel.AssignmentStatus == "Atendido")
                    strBody = string.Format("<p>Estimados,</p><p>El documento <a href='{1}'>{2}</a> de <b>Correspondencia Entrante</b> ha sido atendido en la Intranet.<br/></p><p>Observaciones: <br/><span style='font-style:italic'>{3}</span></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", tmpAssgnTo[0].Name, model.DBModel.DocumentUrl, model.DBModel.DocumentSubject, model.DBModel.Annotations);
                new Utils.EmailHelper().SendEmail("intranetapps@icpower.com", tmpAssgnTo[0].Email, ccList.ToArray(), "Intranet - Correspondencia Entrante", strBody);
                result.ConfirmMessage = "Los cambios se grabaron con éxito. Se le redirigirá pronto al portal.";
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Ocurrió un error al grabar su información. Por favor vuelva a intentar.";
            }
            ViewBag.PreviousPage = Request.QueryString["SPHostUrl"];
            return View(result);
        }

        public ActionResult IncomingMailbox()
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
            //var helper = new Models.IntranetAppsDBHelper();
            var result = new IncomingListViewModel();
            try
            {
                //result.MailboxDocumentList = helper.GetMyMailboxDocuments(strLogin);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Ocurrió un error al obtener los movimientos del documento. Por favor regrese al portal e intente nuevamente.";
            }

            return View(result);
        }

        [HttpGet]
        public ActionResult NewInbox()
        {
            var companyList = new List<SelectListItem>();
            companyList.Add(new SelectListItem() { Text = "Kallpa Generación", Value = "CE Kallpa Generación" });
            companyList.Add(new SelectListItem() { Text = "Cerro del Aguila", Value = "CE Cerro del Aguila" });
            companyList.Add(new SelectListItem() { Text = "Samay I", Value = "CE Samay" });
            companyList.Add(new SelectListItem() { Text = "Hidro Chilia", Value = "CE Hidro Chilia" });
            ViewBag.CompanyList = new SelectList(companyList, "Value", "Text");
            var departmentList = new List<SelectListItem>();
            departmentList.Add(new SelectListItem() { Text = "Todos", Value = "Todos" });
            departmentList.Add(new SelectListItem() { Text = "Administración", Value = "Administración" });
            departmentList.Add(new SelectListItem() { Text = "Contraloría", Value = "Contraloría" });
            departmentList.Add(new SelectListItem() { Text = "Relaciones Comunitarias", Value = "Relaciones Comunitarias" });
            ViewBag.DepartmentList = new SelectList(departmentList, "Value", "Text");
            var result = new NewInboxViewModel();
            if (!string.IsNullOrEmpty(Request.QueryString["SPHostUrl"]))
                result.DBModel.SiteUrl = Request.QueryString["SPHostUrl"];
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(spUser, user => user.Title, user => user.LoginName, user => user.Email);
                    clientContext.ExecuteQuery();
                    result.DBModel.AssignedByLogin = spUser.LoginName;
                    result.DBModel.AssignedByName = spUser.Title;
                }
            }
            result.DBModel.ResponseDate = DateTime.Today;
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> NewInbox(NewInboxViewModel model)
        {
            var companyList = new List<SelectListItem>();
            companyList.Add(new SelectListItem() { Text = "Kallpa Generación", Value = "CE Kallpa Generación" });
            companyList.Add(new SelectListItem() { Text = "Cerro del Aguila", Value = "CE Cerro del Aguila" });
            companyList.Add(new SelectListItem() { Text = "Samay I", Value = "CE Samay" });
            companyList.Add(new SelectListItem() { Text = "Hidro Chilia", Value = "CE Hidro Chilia" });
            ViewBag.CompanyList = new SelectList(companyList, "Value", "Text");
            var departmentList = new List<SelectListItem>();
            departmentList.Add(new SelectListItem() { Text = "Todos", Value = "Todos" });
            departmentList.Add(new SelectListItem() { Text = "Administración", Value = "Administración" });
            departmentList.Add(new SelectListItem() { Text = "Contraloría", Value = "Contraloría" });
            departmentList.Add(new SelectListItem() { Text = "Relaciones Comunitarias", Value = "Relaciones Comunitarias" });
            ViewBag.DepartmentList = new SelectList(departmentList, "Value", "Text");
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(spUser, user => user.Title, user => user.LoginName, user => user.Email);
                    clientContext.ExecuteQuery();
                    model.DBModel.AssignedByLogin = spUser.LoginName;
                    model.DBModel.AssignedByName = spUser.Title;
                }
            }
            try
            {
                model.DBModel.DocumentCode = await helper.GetDocumentCode(model.DBModel.CompanyName, model.DBModel.DepartmentName, DateTime.Today.Year);
                if (string.IsNullOrEmpty(model.DBModel.DocumentCode))
                {
                    model.WarningMessage = "No se encuentra registrado un correlativo. Por favor, contacte al administrador";
                    return View(model);
                }
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
                var spHelper = new SPHelper();
                var spFolder = spHelper.CreateSPFolder(spContext, "CorEnt", model.DBModel.DocumentCode);
                var sbAttMail = new System.Text.StringBuilder();
                var sbAttModel = new System.Text.StringBuilder();
                var j = 1;
                foreach (var anAttachment in model.Attachments)
                {
                    var tmpUrl = spHelper.UploadSPFile(spContext, anAttachment, spFolder, model.DBModel.SiteUrl, "ATT" + j.ToString() + "-");
                    sbAttModel.AppendFormat("{0};", tmpUrl);
                    sbAttMail.AppendFormat("- <a href='{0}'>{1}</a><br/>", tmpUrl, tmpUrl.Substring(tmpUrl.LastIndexOf("/") + 1));
                    j++;
                }
                model.DBModel.Attachment1Url = sbAttModel.ToString();
                model.DBModel.DocumentDate = DateTime.Now;
                model.DBModel.DocumentStatus = "Asignado";
                model.DBModel.FirstReminderDate = DateTime.Today;
                model.DBModel.SecondReminderDate = DateTime.Today;
                model.DBModel.ResponseDate = DateTime.Today.AddDays(3);
                model.DBModel.AssignmentDate = DateTime.Now;
                var tmpSave = await helper.PostInMailbox(model.DBModel, string.Empty);
                var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><p>Se le ha asignado el documento <b>{1}</b> de <b>Correspondencia Entrante</b> en la Intranet.<br/></p><p>Para ver el documento escaneado haga clic en los siguientes enlaces:<br/>{2} <br/>Para ingresar al portal haga <a href='https://icpower.sharepoint.com'>clic aquí</a></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", model.DBModel.AssignedToName, model.DBModel.DocumentSubject, sbAttMail.ToString());
                new Utils.EmailHelper().SendEmail("intranetapps@icpower.com", tmpAssgnTo[0].Email, ccList.ToArray(), "Intranet - " + model.DBModel.DocumentCode + " - " + model.DBModel.DocumentSubject, strBody);
                model.ConfirmMessage = "Se ha guardado el documento " + model.DBModel.DocumentCode + ". Para continuar haga clic en Cerrar.";
            }
            catch (Exception ex)
            {
                log.Error("NewInbox - " + ex.ToString());
                model.ErrorMessage = "Ocurrió un error al guardar su información. Por favor cierre la ventana e intente de nuevo.";
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DoInbox(int documentId)
        {
            var result = new NewInboxViewModel();
            var tmp = await helper.GetInMailbox(documentId);

            var tmpModel = JsonConvert.DeserializeObject<InboxDocumentModel>(tmp);
            if (tmpModel.IncomingMailboxId > 0)
                result.DBModel = tmpModel;
            else
                result.ErrorMessage = "No se pudo obtener información.";
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Asignar", Value = "Asignado" });
            statusList.Add(new SelectListItem() { Text = "Atender", Value = "Atendido" });
            ViewBag.StatusList = new SelectList(statusList, "Value", "Text");
            if (tmpModel.DocumentStatus == "Atendido")
                result.WarningMessage = "Este documento ha sido atendido.";
            else if (!tmpModel.AssignedToLogin.Contains(User.Identity.Name.ToLower()))
                result.WarningMessage = "Usted no es el responsable de este documento, puede visualizar la información pero no modificarla.";
            ViewBag.PreviousPage = Request.QueryString["SPHostUrl"];
            if (result.DBModel.ResponseDate == new DateTime(1900, 1, 1))
                result.DBModel.ResponseDate = DateTime.Today;
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> DoInbox(NewInboxViewModel model)
        {
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Asignar", Value = "Asignado" });
            statusList.Add(new SelectListItem() { Text = "Atender", Value = "Atendido" });
            ViewBag.StatusList = new SelectList(statusList, "Value", "Text");
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (!string.IsNullOrEmpty(Request.QueryString["SPLanguage"]))
                currentCulture = new CultureInfo(Request.QueryString["SPLanguage"]);
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

                        model.DBModel.AssignedByLogin = spUser.LoginName;
                        model.DBModel.AssignedByName = spUser.Title;
                        ViewBag.UserName = spUser.Title;
                    }
                }
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
                model.DBModel.AssignmentDate = DateTime.Now;
                var difDays = model.DBModel.ResponseDate - DateTime.Today;
                if (difDays.Days > 7)
                {
                    var calcDays = Math.Ceiling(difDays.Days * 0.6);
                    model.DBModel.FirstReminderDate = DateTime.Today.AddDays(calcDays);
                    calcDays = Math.Ceiling(difDays.Days * 0.8);
                    model.DBModel.SecondReminderDate = DateTime.Today.AddDays(calcDays);
                }
                else
                {
                    model.DBModel.FirstReminderDate = DateTime.Today;
                    var calcDays = Math.Ceiling(difDays.Days * 0.1);
                    model.DBModel.SecondReminderDate = DateTime.Today.AddDays(calcDays);
                }
                var sbAttachments = new System.Text.StringBuilder();
                if (!string.IsNullOrEmpty(model.DBModel.Attachment1Url)) {
                    var arrAttachments = model.DBModel.Attachment1Url.Split(';');
                    foreach (var att in arrAttachments)
                        if (!string.IsNullOrEmpty(att))
                            sbAttachments.AppendFormat("- <a href='{0}'>{1}</a><br/>", att, att.Substring(att.LastIndexOf("/") + 1));
                }
                var tmpSave = await helper.PostInMailbox(model.DBModel, string.Empty);
                var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><p>Se le ha asignado el documento <b>{1}</b> de <b>Correspondencia Entrante</b> en la Intranet.<br/></p><p>Considerar las siguientes observaciones:<br/>{2}</p><p>Para ver el documento escaneado haga clic en los siguientes enlaces:<br/>{3} <br/>Para ingresar al portal haga <a href='https://icpower.sharepoint.com'>clic aquí</a></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", model.DBModel.AssignedToName, model.DBModel.DocumentSubject, model.DBModel.Annotations, sbAttachments.ToString());
                if (model.DBModel.DocumentStatus == "Atendido")
                    strBody = string.Format("<p>Estimados,</p><p>El documento <b>{0}</b> de <b>Correspondencia Entrante</b> ha sido atendido en la Intranet.<br/></p><p>Observaciones: <br/><span style='font-style:italic'>{1}</span></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", model.DBModel.DocumentSubject, model.DBModel.Annotations);
                new Utils.EmailHelper().SendEmail("intranetapps@icpower.com", tmpAssgnTo[0].Email, ccList.ToArray(), "Intranet - " + model.DBModel.DocumentCode + " - " + model.DBModel.DocumentSubject, strBody);
                model.ConfirmMessage = "Los cambios se grabaron con éxito. Para continuar haga clic en Cerrar.";
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Ocurrió un error al grabar su información. Por favor vuelva a intentar.";
            }
            return View(model);
        }

        public async Task<ActionResult> SearchInbox(InboxDocsViewModel model)
        {
            var result = new InboxDocsViewModel();
            if (model.SearchFilter == null)
            {
                model.SearchFilter = new OutDocumentFilterModel();
                model.SearchFilter.SearchDateFrom = DateTime.Today.AddDays(-7);
                model.SearchFilter.SearchDateTo = DateTime.Today.AddDays(1);
            }
            result.SearchFilter = model.SearchFilter;
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
                var tmp = await helper.GetSearchInMailbox(result.SearchFilter);
                result.MailboxDocumentList = JsonConvert.DeserializeObject<List<InboxDocumentModel>>(tmp);
                foreach (var item in result.MailboxDocumentList)
                {
                    if (item.DocumentStatus == "Atendido")
                    {
                        item.Indicator = "4Gray";
                    }
                    else
                    {
                        if (DateTime.Today >= item.SecondReminderDate)
                        {
                            item.DocumentStatus = "Vencido";
                            item.Indicator = "1Red";
                        }
                        else if (DateTime.Today < item.SecondReminderDate && DateTime.Today >= item.FirstReminderDate)
                        {
                            item.DocumentStatus = "Por Vencer";
                            item.Indicator = "2Yellow";
                        }
                        else
                        {
                            item.DocumentStatus = "Vigente";
                            item.Indicator = "3Green";
                        }
                    }
                }
                result.MailboxDocumentList = result.MailboxDocumentList.OrderBy(x => x.Indicator).ToList();
            }
            catch { result.Message = "Ocurrió un error al obtener su información."; }
            return View(result);
        }
    }
}