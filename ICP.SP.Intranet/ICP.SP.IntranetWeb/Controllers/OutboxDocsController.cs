using DocumentFormat.OpenXml.Packaging;
using ICP.SP.Intranet.Helper;
using ICP.SP.Intranet.Models.Common;
using ICP.SP.IntranetWeb.Utils;
using ICP.SP.IntranetWeb.ViewModels;
using System.IO;
using System.Text.RegularExpressions;
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
    public class OutboxDocsController : Controller
    {
        private IntranetAppsServiceHelper helper = new IntranetAppsServiceHelper();
        log4net.ILog log = log4net.LogManager.GetLogger("Test");

        // GET: OutboxDocs
        public async Task<ActionResult> Index(OutboxDocsViewModel model)
        {
            var result = new OutboxDocsViewModel();
            try
            {
                
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
            
                if (result.SearchFilter != null)
                {
                    model.SearchFilter.SearchDateFrom = null;
                    model.SearchFilter.SearchDateTo = null;
                    var tmp = await helper.GetMyPendingOutMailbox(result.SearchFilter);
                    result.MailboxDocumentList = JsonConvert.DeserializeObject<List<OutDocumentModel>>(tmp);
                    foreach (var item in result.MailboxDocumentList)
                    {
                        if (item.RequestStatus == "Recibido")
                        {
                            item.Indicator = "4Gray";
                        }
                        else
                        {
                            if (item.RequestStatus == "Generado")
                            {
                                item.Indicator = "3Green";
                            }
                            else
                            {
                                if (DateTime.Today >= item.DeliveryExpectedDate)
                                {
                                    item.Indicator = "1Red";
                                }
                                else if (DateTime.Today < item.DeliveryExpectedDate)
                                {
                                    item.Indicator = "2Yellow";
                                }
                            }
                        }
                    }
                    result.MailboxDocumentList = result.MailboxDocumentList.OrderBy(x => x.Indicator).ToList();
                }
            }
            catch (Exception ex) { result.Message = ex.ToString(); }
            return View(result);
        }

        [HttpGet]
        public ActionResult GenerateCode(string siteUrl)
        {
            var result = new OutcomingMailboxViewModel();
            try
            {
                log.Info("GenerateCode");
                result.DBModel.SiteUrl = siteUrl;
                result.DBModel.RequestDate = DateTime.Today;
                if (!string.IsNullOrEmpty(Request.QueryString["SPHostUrl"]))
                    result.DBModel.SiteUrl = Request.QueryString["SPHostUrl"];
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        var spUser = clientContext.Web.CurrentUser;
                        clientContext.Load(spUser, user => user.Title, user => user.Groups);
                        clientContext.ExecuteQuery();
                        result.DBModel.RequestedByName = spUser.Title;
                        ViewBag.UserName = spUser.Title;

                        var spGroups = spUser.Groups;
                        foreach (var group in spGroups)
                            if (group.Title == "Correspondencia Saliente Adm")
                            {
                                ViewBag.CanSave = true;
                                break;
                            }
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.ToString());
            }
            var companyList = new List<SelectListItem>();
            companyList.Add(new SelectListItem() { Text = "Kallpa Generación", Value = "Kallpa Generación" });
            companyList.Add(new SelectListItem() { Text = "Cerro del Aguila", Value = "Cerro del Aguila" });
            companyList.Add(new SelectListItem() { Text = "Samay I", Value = "Samay" });
            companyList.Add(new SelectListItem() { Text = "Hidro Chilia", Value = "Hidro Chilia" });
            ViewBag.CompanyList = new SelectList(companyList, "Value", "Text");
            var departmentList = new List<SelectListItem>();
            departmentList.Add(new SelectListItem() { Text = "Todos", Value = "Todos" });
            departmentList.Add(new SelectListItem() { Text = "Administración", Value = "Administración" });
            departmentList.Add(new SelectListItem() { Text = "Contraloría", Value = "Contraloría" });
            departmentList.Add(new SelectListItem() { Text = "Relaciones Comunitarias", Value = "Relaciones Comunitarias" });
            ViewBag.DepartmentList = new SelectList(departmentList, "Value", "Text");
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> GenerateCode(OutcomingMailboxViewModel model)
        {
            log.Info(Request.Url);
            log.Info(HttpContext == null);

            log.Info("GenerateCode");
            var companyList = new List<SelectListItem>();
            companyList.Add(new SelectListItem() { Text = "Kallpa Generación", Value = "Kallpa Generación" });
            companyList.Add(new SelectListItem() { Text = "Cerro del Aguila", Value = "Cerro del Aguila" });
            companyList.Add(new SelectListItem() { Text = "Samay I", Value = "Samay" });
            companyList.Add(new SelectListItem() { Text = "Hidro Chilia", Value = "Hidro Chilia" });
            ViewBag.CompanyList = new SelectList(companyList, "Value", "Text");
            var departmentList = new List<SelectListItem>();
            departmentList.Add(new SelectListItem() { Text = "Todos", Value = "Todos" });
            departmentList.Add(new SelectListItem() { Text = "Administración", Value = "Administración" });
            departmentList.Add(new SelectListItem() { Text = "Contraloría", Value = "Contraloría" });
            departmentList.Add(new SelectListItem() { Text = "Relaciones Comunitarias", Value = "Relaciones Comunitarias" });
            ViewBag.DepartmentList = new SelectList(departmentList, "Value", "Text");
            var ccMail = new List<string>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        var spGroup = clientContext.Web.SiteGroups.GetByName("Correspondencia Saliente Adm");
                        clientContext.Load(spGroup, gr => gr.Users);
                        clientContext.ExecuteQuery();
                        foreach (var spUser in spGroup.Users)
                            if (!string.IsNullOrEmpty(spUser.Email))
                                ccMail.Add(spUser.Email);
                    }
                }
                var tmpAssgnTo = JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(model.DBModel.RequestedByLogin);
                model.DBModel.RequestedByLogin = tmpAssgnTo[0].Login;
                model.DBModel.RequestedByName = tmpAssgnTo[0].Name;
                model.DBModel.RequestedByEmail = tmpAssgnTo[0].Email;
                model.DBModel.DocumentCode = await helper.GetDocumentCode(model.DBModel.CompanyName, model.DBModel.DepartmentName, DateTime.Today.Year);
                if (string.IsNullOrEmpty(model.DBModel.DocumentCode))
                {
                    model.WarningMessage = "No se encuentra registrado un correlativo. Por favor, contacte al administrador";
                    return View(model);
                }
                var spHelper = new SPHelper();
                model.DBModel.DocumentFolder = spHelper.CreateSPFolder(spContext, "CorSal", model.DBModel.DocumentCode);
                model.DBModel.RequestDate = DateTime.Now;
                model.DBModel.SentDate = new DateTime(1900, 1, 1);
                model.DBModel.DeliveryExpectedDate = new DateTime(1900, 1, 1);
                model.DBModel.DeliveryDate = new DateTime(1900, 1, 1);
                model.DBModel.RequestStatus = "Generado";
                var tmpSave = await helper.PostOutMailbox(model.DBModel);
                var tmpModel = JsonConvert.DeserializeObject<string>(tmpSave);
                model.DBModel.DocumentId = Convert.ToInt32(tmpModel);
                var strBody = string.Format("<p>Estimado <b>{0}</b>,</p><p>Se ha generado el código <b>{1}</b> de <b>Correspondencia Saliente</b> en la Intranet.<br/></p><p>Atentamente,<br/></p><p><b>Intranet IC Power</b></p>", model.DBModel.RequestedByName, model.DBModel.DocumentCode, model.DBModel.DocumentTitle, model.DBModel.CompanyName);

                new Utils.EmailHelper().SendEmail("intranetapps@inkiaenergy.com", model.DBModel.RequestedByEmail, ccMail.ToArray(), "Intranet - " + model.DBModel.DocumentCode, strBody);
                model.ConfirmMessage = string.Format("Se ha generado el código: <b>{0}</b>.", model.DBModel.DocumentCode);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ViewOutDoc(int documentId)
        {
            var result = new OutcomingMailboxViewModel();
            try
            {
                var tmp = await helper.GetOutMailbox(new OutDocumentModel() { DocumentId = documentId });
                var tmpModel = JsonConvert.DeserializeObject<OutDocumentModel>(tmp);
                if (tmpModel.DocumentId > 0)
                {
                    result.DBModel = tmpModel;
                    if (result.DBModel.RequestStatus == "Recibido")
                        result.WarningMessage = "El documento se encuentra recibido, no puede realizar modificaciones";
                }
                else
                    result.ErrorMessage = "No se pudo obtener información.";
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        var spUser = clientContext.Web.CurrentUser;
                        clientContext.Load(spUser, user => user.Groups);
                        clientContext.ExecuteQuery();

                        var spGroups = spUser.Groups;
                        foreach(var group in spGroups)
                            if(group.Title == "Correspondencia Saliente Adm")
                            {
                                ViewBag.CanSave = true;
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> ViewOutDoc(OutcomingMailboxViewModel model)
        {
            log.Info("- OutboxDocs - ViewOutDoc:");
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                if (model.TempFile != null)
                {
                    log.Info("Tiene archivo adjunto");
                    var prefix = model.DBModel.RequestStatus == "Generado" ? "DOC-" : "REC-";
                    var spHelper = new SPHelper();
                    if (model.DBModel.RequestStatus == "Generado")
                    {
                        model.DBModel.DocumentFilename = spHelper.UploadSPFile(spContext, model.TempFile, model.DBModel.DocumentFolder, model.DBModel.SiteUrl, prefix);
                        log.Info(model.DBModel.DocumentFilename);
                    }
                    else
                    {
                        model.DBModel.ReceiptFilename = spHelper.UploadSPFile(spContext, model.TempFile, model.DBModel.DocumentFolder, model.DBModel.SiteUrl, prefix);
                        log.Info(model.DBModel.ReceiptFilename);
                    }
                }
                var tmpSave = await helper.PostOutMailbox(model.DBModel);
                model.ConfirmMessage = "Se ha guardado su información.";
                log.Info("Se ha guardado: " + tmpSave.ToString());
            }
            catch(Exception ex)
            {
                model.ErrorMessage = "Ocurrió un error al guardar su información.";
                
                log.Error(ex.ToString());
            }
            return View(model);
        }

        public async Task<ActionResult> DownloadOutboxFile(int documentId)
        {
            var tmp = await helper.GetOutMailbox(new OutDocumentModel() { DocumentId = documentId });
            var tmpModel = JsonConvert.DeserializeObject<OutDocumentModel>(tmp);
            string document = Server.MapPath("~/templates/corsaliente-plantilla.docx");
            byte[] byteArray = System.IO.File.ReadAllBytes(document);
            string docText = null;
            MemoryStream mem = new MemoryStream();
            mem.Write(byteArray, 0, (int)byteArray.Length);
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(mem, true))
            {
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }
                Regex regexText = new Regex("DocCode");
                docText = regexText.Replace(docText, tmpModel.DocumentCode);
                regexText = new Regex("DocSubject");
                docText = regexText.Replace(docText, tmpModel.DocumentTitle);

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            mem.Position = 0;
            string contentType = MimeMapping.GetMimeMapping(document);
            return File(mem, contentType, tmpModel.DocumentCode.Replace("/", "-") + ".docx");
        }
    }
}