using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICP.SP.Intranet.DataAccess;
using ICP.SP.Intranet.Models.Common;


namespace ICP.SP.Intranet.BusinessLogic
{
    public class IntranetAppsHelper
    {
        private IntranetAppsDbEntities context = new IntranetAppsDbEntities();
        #region DocumentAssignment
        public DocumentAssignmentModel GetDocumentAssignmentById(int id)
        {
            var tmp = context.DocumentAssignment.FirstOrDefault(x => x.DocumentAssignmentId == id);
            var result = new DocumentAssignmentModel();
            if(tmp != null)
            {
                result = ToModel(tmp);
            }
            return result;
        }

        public DocumentAssignmentModel GetDocumentAssignmentByLocation(string siteUrl, string docLibraryId, int itemId, string status)
        {
            var tmp = context.DocumentAssignment.Where(x => x.SiteUrl == siteUrl && x.DocumentLibraryId == docLibraryId && x.ListItemId == itemId && (string.IsNullOrEmpty(status) || x.AssignmentStatus == status)).OrderByDescending(x => x.AssignmentDate);
            var result = new DocumentAssignmentModel();
            if (tmp != null)
            {
                if (tmp.Count() > 0)
                    result = ToModel(tmp.First());
            }
            return result;
        }

        public List<DocumentAssignmentModel> GetMyPendingDocumentAssigment(string accountName)
        {
            var result = new List<DocumentAssignmentModel>();
            var tmp = context.DocumentAssignment.Where(x => x.AssignedToLogin == accountName && x.AssignmentStatus == "Pendiente").OrderByDescending(x=> x.DueToDate);
            if (tmp != null)
                result = tmp.ToList().ConvertAll(ToModel);
            return result;
        }

        

        public int SaveDocumentAssignment(DocumentAssignmentModel model)
        {
            var result = 0;
            var tmp = ToEntity(model);
            if (model.DocumentAssignmentId > 0)
            {
                tmp = context.DocumentAssignment.FirstOrDefault(x => x.DocumentAssignmentId == model.DocumentAssignmentId);
                if (model.AssignmentStatus == "Cerrado")
                {
                    tmp.AssignmentStatus = model.AssignmentStatus;
                }
                else
                {
                    tmp.AssignmentStatus = model.AssignmentStatus;
                    tmp.DueToDate = model.DueToDate;
                    tmp.FirstReminderDays = model.FirstReminderDays;
                    tmp.SecondReminderDays = model.SecondReminderDays;
                }
            }
            else
            {
                context.DocumentAssignment.Add(tmp);
            }
            context.SaveChanges();
            result = tmp.DocumentAssignmentId;
            return result;
        }

        private DocumentAssignmentModel ToModel(DocumentAssignment doc)
        {
            var result = new DocumentAssignmentModel();
            result.AssignedByLogin = doc.AssignedByLogin;
            result.AssignedByName = doc.AssignedByName;
            result.AssignedToControl = doc.AssignedToControl;
            result.AssignedToLogin = doc.AssignedToLogin;
            result.AssignedToName = doc.AssignedToName;
            result.AssignmentDate = doc.AssignmentDate.HasValue? doc.AssignmentDate.Value : DateTime.MinValue;
            result.AssignmentStatus = doc.AssignmentStatus;
            result.DocumentAssignmentId = doc.DocumentAssignmentId;
            result.DocumentLibraryId = doc.DocumentLibraryId;
            result.DocumentTitle = doc.DocumentTitle;
            result.DocumentUrl = doc.DocumentUrl;
            result.DueToDate = doc.DueToDate.HasValue? doc.DueToDate.Value:DateTime.MinValue;
            result.FirstReminderDays = doc.FirstReminderDays.HasValue ? doc.FirstReminderDays.Value : 0;
            result.ListItemId = doc.ListItemId.HasValue ? doc.ListItemId.Value : 0;
            result.SecondReminderDays = doc.SecondReminderDays.HasValue ? doc.SecondReminderDays.Value : 0;
            result.SiteUrl = doc.SiteUrl;
            result.DocumentAnnotations = doc.DocumentAnnotations;
            result.DocumentSubject = doc.DocumentSubject;
            return result;
        }

        private DocumentAssignment ToEntity(DocumentAssignmentModel doc)
        {
            var result = new DocumentAssignment();
            result.AssignedByLogin = doc.AssignedByLogin;
            result.AssignedByName = doc.AssignedByName;
            result.AssignedToControl = doc.AssignedToControl;
            result.AssignedToLogin = doc.AssignedToLogin;
            result.AssignedToName = doc.AssignedToName;
            result.AssignmentDate = doc.AssignmentDate;
            result.AssignmentStatus = doc.AssignmentStatus;
            result.DocumentAssignmentId = doc.DocumentAssignmentId;
            result.DocumentLibraryId = doc.DocumentLibraryId;
            result.DocumentTitle = doc.DocumentTitle;
            result.DocumentUrl = doc.DocumentUrl;
            result.DueToDate = doc.DueToDate;
            result.FirstReminderDays = doc.FirstReminderDays;
            result.ListItemId = doc.ListItemId;
            result.SecondReminderDays = doc.SecondReminderDays;
            result.SiteUrl = doc.SiteUrl;
            result.DocumentAnnotations = doc.DocumentAnnotations;
            result.DocumentSubject = doc.DocumentSubject;
            return result;
        }
        #endregion

        #region IncomingMailbox
        public MailboxDocumentModel GetIncomingMailboxById(int id)
        {
            var tmp = context.IncomingMailbox.FirstOrDefault(x => x.IncomingMailboxId == id);
            var result = new MailboxDocumentModel();
            if (tmp != null)
            {
                result = ToModelIncoming(tmp);
            }
            return result;
        }

        public MailboxDocumentModel GetIncomingMailboxByLocation(string siteUrl, string docLibraryId, int itemId, string status)
        {
            var tmp = context.IncomingMailbox.Where(x => x.SiteUrl == siteUrl && x.DocumentLibraryId == docLibraryId && x.ListItemId == itemId && (string.IsNullOrEmpty(status) || x.AssignmentStatus == status)).OrderByDescending(x => x.AssignmentDate); 
            var result = new MailboxDocumentModel();
            if (tmp != null)
            {
                if (tmp.Count() > 0)
                    result = ToModelIncoming(tmp.First());
            }
            return result;
        }

        /*public List<MailboxDocumentModel> GetMyPendingIncomingMailbox(string accountName)
        {
            var result = new List<MailboxDocumentModel>();
            var tmp = context.IncomingMailbox.Where(x => x.AssignedToLogin == accountName || x.AssignedToCCLogin.Contains(accountName)).OrderByDescending(x => x.DocumentDate);
            if (tmp != null)
                result = tmp.ToList().ConvertAll(ToModelIncoming);
            return result;
        }*/

        public List<InboxDocumentModel> GetMyPendingIncomingMailbox(string accountName)
        {
            var result = new List<InboxDocumentModel>();
            var tmp = context.InboxDocument.Where(x => x.AssignedToLogin == accountName || x.AssignedToCCLogin.Contains(accountName)).OrderByDescending(x => x.DocumentDate);
            if (tmp != null)
                result = tmp.ToList().ConvertAll(ToModel);
            return result;
        }

        public List<InboxDocumentModel> GetIncomingMailbox(OutDocumentFilterModel filter)
        {
            var result = new List<InboxDocumentModel>();
            var tmp = context.InboxDocument.Where(m => (string.IsNullOrEmpty(filter.SearchText) || m.DocumentCode.ToLower().Contains(filter.SearchText.ToLower()) || m.CompanyName.ToLower().Contains(filter.SearchText.ToLower()) || m.DocumentSubject.ToLower().Contains(filter.SearchText.ToLower()) || m.FromCompany.ToLower().Contains(filter.SearchText.ToLower())) && (!filter.SearchDateFrom.HasValue || m.DocumentDate >= filter.SearchDateFrom.Value) && (!filter.SearchDateTo.HasValue || m.DocumentDate <= filter.SearchDateTo.Value)).OrderByDescending(x => x.DocumentDate).Take(50);
            if (tmp != null)
                result = tmp.ToList().ConvertAll(ToModel);
            return result;
        }

        public int SaveIncomingMailbox(MailboxDocumentModel model)
        {
            var result = 0;
            var tmp = ToEntityIncoming(model);
            var tmpDetail = new IncomingMailboxDetail();
            tmpDetail.AssignedByLogin = model.AssignedByLogin;
            tmpDetail.AssignedByName = model.AssignedByName;
            tmpDetail.AssignedToLogin = model.AssignedToLogin;
            tmpDetail.AssignedToName = model.AssignedToName;
            tmpDetail.AssignedToCCLogin = model.AssignedToCCLogin;
            tmpDetail.AssignedToCCName = model.AssignedToCCName;
            tmpDetail.AssignmentDate = model.AssignmentDate;
            tmpDetail.AssignmentStatus = model.AssignmentStatus;
            tmpDetail.DocumentAnnotation = model.Annotations;
            tmpDetail.AssignmentDueTo = model.ResponseDate;
            if (model.MailboxDocumentId > 0)
            {
                tmp = context.IncomingMailbox.FirstOrDefault(x => x.IncomingMailboxId == model.MailboxDocumentId);
                if (model.AssignmentStatus == "Asignado")
                {
                    tmp.AssignedByLogin = model.AssignedByLogin;
                    tmp.AssignedByName = model.AssignedByName;
                    tmp.AssignedToCCControl = model.AssignedToCCControl;
                    tmp.AssignedToCCLogin = model.AssignedToCCLogin;
                    tmp.AssignedToCCName = model.AssignedToCCName;
                    tmp.AssignedToControl = model.AssignedToControl;
                    tmp.AssignedToLogin = model.AssignedToLogin;
                    tmp.AssignedToName = model.AssignedToName;
                    tmp.AssignmentDate = model.AssignmentDate;
                    tmp.ResponseDate = model.ResponseDate;
                }
                tmp.DocumentAnnotation = model.Annotations;
                tmp.AssignmentStatus = model.AssignmentStatus;
            }
            else
            {
                context.IncomingMailbox.Add(tmp);
            }
            context.SaveChanges();
            tmpDetail.IncomingMailboxId = tmp.IncomingMailboxId;
            context.IncomingMailboxDetail.Add(tmpDetail);
            context.SaveChanges();
            result = tmp.IncomingMailboxId;
            return result;
        }

        private MailboxDocumentModel ToModelIncoming(IncomingMailbox doc)
        {
            var result = new MailboxDocumentModel();
            result.Annotations = doc.DocumentAnnotation;
            result.AssignedByLogin = doc.AssignedByLogin;
            result.AssignedByName = doc.AssignedByName;
            result.AssignedToControl = doc.AssignedToControl;
            result.AssignedToLogin = doc.AssignedToLogin;
            result.AssignedToName = doc.AssignedToName;
            result.AssignedToCCName = doc.AssignedToCCName;
            result.AssignedToCCControl = doc.AssignedToCCControl;
            result.AssignedToCCLogin = doc.AssignedToCCLogin;
            result.AssignmentDate = doc.AssignmentDate.HasValue ? doc.AssignmentDate.Value : DateTime.MinValue;
            result.AssignmentStatus = doc.AssignmentStatus;
            result.MailboxDocumentId = doc.IncomingMailboxId;
            result.DocumentFrom = doc.DocumentFrom;
            result.DocumentLibraryId = doc.DocumentLibraryId;
            result.DocumentTitle = doc.DocumentTitle;
            result.DocumentUrl = doc.DocumentUrl;
            result.DocumentDate = doc.DocumentDate.HasValue ? doc.DocumentDate.Value : DateTime.MinValue;
            result.DocumentSubject = doc.DocumentSubject;
            result.ListItemId = doc.ListItemId.HasValue ? doc.ListItemId.Value : 0;
            result.DocumentSummary = doc.DocumentSummary;
            result.SiteUrl = doc.SiteUrl;
            result.ResponseDate = doc.ResponseDate.HasValue ? doc.ResponseDate.Value : DateTime.MinValue;
            result.FirstReminderDate = doc.FirstReminderDate.HasValue ? doc.FirstReminderDate.Value : DateTime.MinValue;
            result.SecondReminderDate = doc.SecondReminderDate.HasValue ? doc.SecondReminderDate.Value : DateTime.MinValue;
            return result;
        }

        private IncomingMailbox ToEntityIncoming(MailboxDocumentModel doc)
        {
            var result = new IncomingMailbox();
            result.DocumentAnnotation = doc.Annotations;
            result.AssignedByLogin = doc.AssignedByLogin;
            result.AssignedByName = doc.AssignedByName;
            result.AssignedToControl = doc.AssignedToControl;
            result.AssignedToLogin = doc.AssignedToLogin;
            result.AssignedToName = doc.AssignedToName;
            result.AssignedToCCName = doc.AssignedToCCName;
            result.AssignedToCCControl = doc.AssignedToCCControl;
            result.AssignedToCCLogin = doc.AssignedToCCLogin;
            result.AssignmentDate = doc.AssignmentDate;
            result.AssignmentStatus = doc.AssignmentStatus;
            result.IncomingMailboxId = doc.MailboxDocumentId;
            result.DocumentFrom = doc.DocumentFrom;
            result.DocumentLibraryId = doc.DocumentLibraryId;
            result.DocumentTitle = doc.DocumentTitle;
            result.DocumentUrl = doc.DocumentUrl;
            result.DocumentDate = doc.DocumentDate;
            result.DocumentSubject = doc.DocumentSubject;
            result.ListItemId = doc.ListItemId;
            result.DocumentSummary = doc.DocumentSummary;
            result.SiteUrl = doc.SiteUrl;
            result.ResponseDate = doc.ResponseDate;
            result.FirstReminderDate = doc.FirstReminderDate;
            result.SecondReminderDate = doc.SecondReminderDate;
            return result;
        }
        #endregion

        #region IncomingMailboxDetail
        public List<MailboxDocumentDetailsModel> GetIncomingMailboxDetailsById(int id)
        {
            var tmp = context.InboxDocumentDetail.Where(x => x.IncomingDocumentId == id).OrderBy(x=> x.AssignmentDate);
            var result = new List<MailboxDocumentDetailsModel>();
            if (tmp != null)
            {
                result = tmp.ToList().ConvertAll(ToModel);
            }
            return result;
        }

        private MailboxDocumentDetailsModel ToModel(InboxDocumentDetail doc)
        {
            var result = new MailboxDocumentDetailsModel();
            result.Annotations = doc.DocumentAnnotation;
            result.AssignedByLogin = doc.AssignedByLogin;
            result.AssignedByName = doc.AssignedByName;
            result.AssignedToLogin = doc.AssignedToLogin;
            result.AssignedToName = doc.AssignedToName;
            result.AssignedToCCName = doc.AssignedToCCName;
            result.AssignedToCCLogin = doc.AssignedToCCLogin;
            result.AssignmentDate = doc.AssignmentDate.HasValue ? doc.AssignmentDate.Value : DateTime.MinValue;
            result.AssignmentStatus = doc.AssignmentStatus;
            result.MailboxDocumentId = doc.IncomingDocumentId;
            result.MailboxDocumentDetailId = doc.IncomingDocumentDetailId;
            result.DueToDate = doc.AssignmentDueTo.HasValue ? doc.AssignmentDueTo.Value : DateTime.MinValue;
            return result;
        }
        #endregion

        #region DocumentCoding
        public string GenerateCode(DocumentCodingModel model)
        {
            var tmp = context.CodingParameters.Where(x => x.Company == model.CompanyName && x.Department == model.DepartmentName && x.Year == model.RequestedDate.Year);
            var initials = string.Empty;
            var counter = 1;
            var curYear = DateTime.Today;
            if(tmp.Count() > 0)
            {
                var item = tmp.FirstOrDefault();
                initials = item.Prefix;
                counter = 1 + item.Code.Value;
                curYear = model.RequestedDate;
                item.Code = counter;
                context.SaveChanges();
            }
            return string.Format("{0}{1:0000}/{2:yy}", initials, counter, curYear);
        }

        public List<OutDocumentModel> GetMyPendingOutcomingMailbox(OutDocumentFilterModel filter)
        {
            var result = new List<OutDocumentModel>();
            var tmp = context.OutcomingMailbox.Where(m => (string.IsNullOrEmpty(filter.SearchText) || m.DocumentCode.ToLower().Contains(filter.SearchText.ToLower()) || m.RequestedByName.ToLower().Contains(filter.SearchText.ToLower()) || m.DocumentSubject.ToLower().Contains(filter.SearchText.ToLower()) || m.DestinationCompany.ToLower().Contains(filter.SearchText.ToLower()))).OrderByDescending(x => x.RequestDate).Take(50);
            if (tmp != null)
                result = tmp.ToList().ConvertAll(ToModel);
            return result;
        }

        private OutDocumentModel ToModel(OutcomingMailbox doc)
        {
            var result = new OutDocumentModel();
            result.DocumentId = doc.OutcomingMailboxId;
            result.SiteUrl = doc.SiteUrl;
            result.DocumentCode = doc.DocumentCode;
            result.RequestDate = doc.RequestDate.HasValue ? doc.RequestDate.Value : DateTime.MinValue;
            result.RequestedByLogin = doc.RequestedByLogin;
            result.RequestedByName = doc.RequestedByName;
            result.DocumentTitle = doc.DocumentSubject;
            result.CompanyName = doc.CompanyName;
            result.DepartmentName = doc.DepartmentName;
            result.SentDate = doc.SentDate.HasValue ? doc.SentDate.Value : DateTime.MinValue; ;
            result.DeliveryDate = doc.DeliveryDate.HasValue ? doc.DeliveryDate.Value : DateTime.MinValue; 
            result.DeliveryExpectedDate = doc.ExpectedDeliveryDate.HasValue ? doc.ExpectedDeliveryDate.Value : DateTime.MinValue; 
            result.RequestStatus = doc.DocumentStatus;
            result.DocumentFolder = doc.DocumentFolder;
            result.DocumentFilename = doc.DocumentFilename;
            result.ReceiptFilename = doc.ReceiptFilename;
            result.DestinationCompany = doc.DestinationCompany;
            return result;
        }

        private OutcomingMailbox ToEntity(OutDocumentModel doc)
        {
            var result = new OutcomingMailbox();
            result.OutcomingMailboxId = doc.DocumentId;
            result.SiteUrl = doc.SiteUrl;
            result.DocumentCode = doc.DocumentCode;
            result.RequestDate = doc.RequestDate;
            result.RequestedByLogin = doc.RequestedByLogin;
            result.RequestedByName = doc.RequestedByName;
            result.DocumentSubject = doc.DocumentTitle;
            result.CompanyName = doc.CompanyName;
            result.DepartmentName = doc.DepartmentName;
            result.SentDate = doc.SentDate;
            result.DeliveryDate = doc.DeliveryDate;
            result.ExpectedDeliveryDate = doc.DeliveryExpectedDate;
            result.DocumentStatus = doc.RequestStatus;
            result.DocumentFolder = doc.DocumentFolder;
            result.DocumentFilename = doc.DocumentFilename;
            result.ReceiptFilename = doc.ReceiptFilename;
            result.DestinationCompany = doc.DestinationCompany;
            return result;
        }

        public int SaveOutcomingMailbox(OutDocumentModel model)
        {
            var result = 0;
            var tmp = ToEntity(model);
            
            if (model.DocumentId > 0)
            {
                tmp = context.OutcomingMailbox.FirstOrDefault(x => x.OutcomingMailboxId == model.DocumentId);
                if (model.RequestStatus == "Generado")
                {
                    tmp.SentDate = DateTime.Today;
                    tmp.ExpectedDeliveryDate = DateTime.Today.AddDays(7);
                    tmp.DocumentFilename = model.DocumentFilename;
                    tmp.DocumentStatus = "Enviado";
                    tmp.DocumentSubject = model.DocumentTitle;
                    tmp.DestinationCompany = model.DestinationCompany;
                }
               else
                {
                    tmp.DeliveryDate = DateTime.Today;
                    tmp.ReceiptFilename = model.ReceiptFilename;
                    tmp.DocumentStatus = "Recibido";
                }
            }
            else
            {
                context.OutcomingMailbox.Add(tmp);
            }
            context.SaveChanges();
            
            result = tmp.OutcomingMailboxId;
            return result;
        }

        public OutDocumentModel GetOutcomingMailboxById(int id)
        {
            var tmp = context.OutcomingMailbox.FirstOrDefault(x => x.OutcomingMailboxId == id);
            var result = new OutDocumentModel();
            if (tmp != null)
            {
                result = ToModel(tmp);
            }
            return result;
        }
        #endregion

        #region InboxDocument
        public int SaveInboxDocument(InboxDocumentModel model)
        {
            var result = 0;
            var tmp = ToEntity(model);
            var tmpDetail = new InboxDocumentDetail();
            tmpDetail.AssignedByLogin = model.AssignedByLogin;
            tmpDetail.AssignedByName = model.AssignedByName;
            tmpDetail.AssignedToLogin = model.AssignedToLogin;
            tmpDetail.AssignedToName = model.AssignedToName;
            tmpDetail.AssignedToCCLogin = model.AssignedToCCLogin;
            tmpDetail.AssignedToCCName = model.AssignedToCCName;
            tmpDetail.AssignmentDate = model.AssignmentDate;
            tmpDetail.AssignmentStatus = model.DocumentStatus;
            tmpDetail.DocumentAnnotation = model.Annotations;
            tmpDetail.AssignmentDueTo = model.ResponseDate;
            if (model.IncomingMailboxId > 0)
            {
                tmp = context.InboxDocument.FirstOrDefault(x => x.IncomingMailboxId== model.IncomingMailboxId);
                if (model.DocumentStatus == "Asignado")
                {
                    tmp.AssignedByLogin = model.AssignedByLogin;
                    tmp.AssignedByName = model.AssignedByName;
                    tmp.AssignedToCCControl = model.AssignedToCCControl;
                    tmp.AssignedToCCLogin = model.AssignedToCCLogin;
                    tmp.AssignedToCCName = model.AssignedToCCName;
                    tmp.AssignedToControl = model.AssignedToControl;
                    tmp.AssignedToLogin = model.AssignedToLogin;
                    tmp.AssignedToName = model.AssignedToName;
                    tmp.AssignmentDate = model.AssignmentDate;
                    tmp.ResponseDate = model.ResponseDate;
                    tmp.FirstReminderDate = model.FirstReminderDate;
                    tmp.SecondReminderDate = model.SecondReminderDate;
                }
                tmp.Annotations = model.Annotations;
                tmp.DocumentStatus = model.DocumentStatus;
            }
            else
            {
                context.InboxDocument.Add(tmp);
            }
            context.SaveChanges();
            tmpDetail.IncomingDocumentId = tmp.IncomingMailboxId;
            context.InboxDocumentDetail.Add(tmpDetail);
            context.SaveChanges();
            result = tmp.IncomingMailboxId;
            return result;
        }

        public InboxDocumentModel GetIncomingDocument(int docId)
        {
            var tmp = context.InboxDocument.FirstOrDefault(x => x.IncomingMailboxId == docId);
            var result = new InboxDocumentModel();
            if (tmp != null)
            {
                result = ToModel(tmp);
            }
            return result;
        }

        private InboxDocument ToEntity(InboxDocumentModel model)
        {
            var result = new InboxDocument();
            result.Annotations = model.Annotations;
            result.AssignedByLogin = model.AssignedByLogin;
            result.AssignedByName = model.AssignedByName;
            result.AssignedToCCControl = model.AssignedToCCControl;
            result.AssignedToCCLogin = model.AssignedToCCLogin;
            result.AssignedToCCName = model.AssignedToCCName;
            result.AssignedToControl = model.AssignedToControl;
            result.AssignedToLogin = model.AssignedToLogin;
            result.AssignedToName = model.AssignedToName;
            result.AssignmentDate = model.AssignmentDate;
            result.AttachmentUrl = model.Attachment1Url;
            result.CompanyName = model.CompanyName;
            result.DepartmentName = model.DepartmentName;
            result.DocumentCode = model.DocumentCode;
            result.DocumentDate = model.DocumentDate;
            result.DocumentStatus = model.DocumentStatus;
            result.DocumentSubject = model.DocumentSubject;
            result.FirstReminderDate = model.FirstReminderDate;
            result.FromCompany = model.FromCompany;
            result.FromContact = model.FromContact;
            result.IncomingMailboxId = model.IncomingMailboxId;
            result.ResponseDate = model.ResponseDate;
            result.SecondReminderDate = model.SecondReminderDate;
            result.SiteUrl = model.SiteUrl;
            return result;
        }

        private InboxDocumentModel ToModel(InboxDocument model)
        {
            var result = new InboxDocumentModel();
            result.Annotations = model.Annotations;
            result.AssignedByLogin = model.AssignedByLogin;
            result.AssignedByName = model.AssignedByName;
            result.AssignedToCCControl = model.AssignedToCCControl;
            result.AssignedToCCLogin = model.AssignedToCCLogin;
            result.AssignedToCCName = model.AssignedToCCName;
            result.AssignedToControl = model.AssignedToControl;
            result.AssignedToLogin = model.AssignedToLogin;
            result.AssignedToName = model.AssignedToName;
            result.AssignmentDate = model.AssignmentDate.HasValue ? model.AssignmentDate.Value:DateTime.MinValue;
            result.Attachment1Url = model.AttachmentUrl;
            result.CompanyName = model.CompanyName;
            result.DepartmentName = model.DepartmentName;
            result.DocumentCode = model.DocumentCode;
            result.DocumentDate = model.DocumentDate.HasValue ? model.DocumentDate.Value : DateTime.MinValue;
            result.DocumentStatus = model.DocumentStatus;
            result.DocumentSubject = model.DocumentSubject;
            result.FirstReminderDate = model.FirstReminderDate.HasValue ? model.FirstReminderDate.Value : DateTime.MinValue;
            result.FromCompany = model.FromCompany;
            result.FromContact = model.FromContact;
            result.IncomingMailboxId = model.IncomingMailboxId;
            result.ResponseDate = model.ResponseDate.HasValue ? model.ResponseDate.Value : DateTime.MinValue;
            result.SecondReminderDate = model.SecondReminderDate.HasValue ? model.SecondReminderDate.Value : DateTime.MinValue;
            result.SiteUrl = model.SiteUrl;
            return result;
        }
        #endregion
    }
}
