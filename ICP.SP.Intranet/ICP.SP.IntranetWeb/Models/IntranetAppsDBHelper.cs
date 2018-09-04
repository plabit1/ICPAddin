using ICP.SP.IntranetWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.Models
{
    public class IntranetAppsDBHelper
    {
        //private IntranetAppsDbContext db = new IntranetAppsDbContext();

        #region Asign Pending Documents
        public PendingDocumentViewModel SaveDocAssignment(PendingDocumentViewModel doc)
        {
            PendingDocumentViewModel result = new PendingDocumentViewModel();
            
            /*var newDocAssgn = new DocumentAssignment();
            newDocAssgn = ToModel(doc);
            try
            {
                if (!doc.IsModified)
                    db.DocumentAssignmentList.Add(newDocAssgn);
                else
                {
                    var tmp = db.DocumentAssignmentList.FirstOrDefault(item => item.SiteUrl == doc.SiteUrl && item.DocLibrary == doc.DocLibrary && item.ListItemId == doc.ListItemId);
                    var tmpAssgnTo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(newDocAssgn.AssignedTo);
                    tmp.AssignedTo = newDocAssgn.AssignedTo;
                    tmp.AssignedToName = tmpAssgnTo[0].Login;
                    tmp.DueTo = newDocAssgn.DueTo;
                    tmp.FirstReminderDays = newDocAssgn.FirstReminderDays;
                    tmp.SecondReminderDays = newDocAssgn.SecondReminderDays;
                    tmp.DocumentURL = newDocAssgn.DocumentURL;
                    tmp.Title = newDocAssgn.Title;
                    tmp.AssignedBy = newDocAssgn.AssignedBy.Split('/')[0];
                    tmp.AssignedByName = newDocAssgn.AssignedBy.Split('/')[1];
                    db.SaveChanges();
                }
                    
                db.SaveChanges();
            }
            catch (Exception ex){ }
            result = ToViewModel(newDocAssgn);*/
            return result;
        }

        public PendingDocumentViewModel GetDocAssignment(PendingDocumentViewModel doc)
        {
            var result = new PendingDocumentViewModel();
            /*var tmp = db.DocumentAssignmentList.FirstOrDefault(item => item.SiteUrl == doc.SiteUrl && item.DocLibrary == doc.DocLibrary && item.ListItemId == doc.ListItemId);
            if (tmp != null)
            {
                if (tmp.ListItemId > 0)
                {
                    result = ToViewModel(tmp);
                }
            }*/
            return result;
        }

        public List<DocumentAssignment> GetMyAssignments(string assignedTo)
        {
            var result = new List<DocumentAssignment>();
            /*var tmp = db.DocumentAssignmentList.Where(item => item.AssignedToName == assignedTo).OrderByDescending(item => item.DueTo);
            if (tmp != null)
            {
                result = tmp.ToList();
            }*/
            return result;
        }
        #endregion

        #region Asign Incoming Documents
        public IncomingMailboxViewModel SaveIncomingMailbox(IncomingMailboxViewModel doc)
        {
            IncomingMailboxViewModel result = new IncomingMailboxViewModel();

            /*var newDocAssgn = new MailboxDocument();
            newDocAssgn = ToModel(doc);
            try
            {
                if (!doc.IsModified)
                    db.MailboxDocumentList.Add(newDocAssgn);
                else
                {
                    var tmp = db.MailboxDocumentList.FirstOrDefault(item => item.SiteUrl == doc.SiteUrl && item.DocLibrary == doc.DocLibrary && item.ListItemId == doc.ListItemId);
                    var tmpAssgnTo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(newDocAssgn.AssignedTo);
                    tmp.AssignedTo = newDocAssgn.AssignedTo;
                    tmp.AssignedToName = tmpAssgnTo[0].Name;
                    tmp.AssignedToAccount = tmpAssgnTo[0].Login;
                    tmp.DocumentDate = newDocAssgn.DocumentDate;
                    tmp.DocumentSummary = newDocAssgn.DocumentSummary;
                    tmp.DocumentTitle = newDocAssgn.DocumentTitle;
                    tmp.DocumentURL = newDocAssgn.DocumentURL;
                    tmp.Title = newDocAssgn.Title;
                    tmp.AssignedBy = newDocAssgn.AssignedBy.Split('/')[0];
                    tmp.AssignedByName = newDocAssgn.AssignedBy.Split('/')[1];
                    tmp.AssignedToCC = newDocAssgn.AssignedToCC;
                    var tmpCCName = string.Empty;
                    if (!string.IsNullOrEmpty(newDocAssgn.AssignedToCC))
                    {
                        var tmpAssgnToCC = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PeoplePickerUser>>(newDocAssgn.AssignedToCC);
                        foreach (PeoplePickerUser user in tmpAssgnToCC)
                            tmpCCName += user.Login + ";";
                    }
                    tmp.AssignedToCCName = tmpCCName;
                    tmp.DocumentStatus = newDocAssgn.DocumentStatus;
                    db.SaveChanges();
                    var tmpAction = new MailboxDetails();
                    tmpAction.ActionName = "Enviar";
                    tmpAction.AssignedBy = tmp.AssignedBy;
                    tmpAction.AssignedByName = tmp.AssignedByName;
                    tmpAction.AssignedTo = tmpAssgnTo[0].Login;
                    tmpAction.AssignedToName = tmpAssgnTo[0].Name;
                    tmpAction.AssignmentDate = tmp.AssignmentDate;
                    tmpAction.MailboxDocumentId = tmp.MailboxDocumentId;
                    tmpAction.Annotations = string.Empty;
                    db.MailboxDetailsList.Add(tmpAction);
                    db.SaveChanges();
                }

                db.SaveChanges();
            }
            catch (Exception ex) { }
            result = ToViewModel(newDocAssgn);*/
            return result;
        }

        public DoIncomingViewModel SaveIncomingMailboxUpdate(DoIncomingViewModel doc)
        {
            DoIncomingViewModel result = new DoIncomingViewModel();
/*
            var newDocAssgn = new MailboxDocument();
            newDocAssgn = ToModel(doc);
            try
            {
                var tmp = db.MailboxDocumentList.FirstOrDefault(item => item.MailboxDocumentId == doc.AssignmentId);
                if (doc.DocumentStatus == "Asignar") {
                    var tmpAssgnTo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.PeoplePickerUser>>(newDocAssgn.AssignedTo);
                    tmp.AssignedTo = newDocAssgn.AssignedTo;
                    tmp.AssignedToName = tmpAssgnTo[0].Name;
                    tmp.AssignedToAccount = tmpAssgnTo[0].Login;
                    tmp.DocumentSummary = tmp.DocumentSummary + "||" + doc.NewAnnotations;
                    tmp.AssignedBy = newDocAssgn.AssignedBy.Split('/')[0];
                    tmp.AssignedByName = newDocAssgn.AssignedBy.Split('/')[1];
                    tmp.AssignedToCC = newDocAssgn.AssignedToCC;
                    tmp.DocumentStatus = newDocAssgn.DocumentStatus;
                    var tmpCCName = string.Empty;
                    if (!string.IsNullOrEmpty(newDocAssgn.AssignedToCC))
                    {
                        var tmpAssgnToCC = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PeoplePickerUser>>(newDocAssgn.AssignedToCC);
                        foreach (PeoplePickerUser user in tmpAssgnToCC)
                            tmpCCName += user.Login + ";";
                    }
                    tmp.AssignedToCCName = tmpCCName;
                    db.SaveChanges();
                    var tmpAction = new MailboxDetails();
                    tmpAction.ActionName = doc.DocumentStatus;
                    tmpAction.AssignedBy = tmp.AssignedBy;
                    tmpAction.AssignedByName = tmp.AssignedByName;
                    tmpAction.AssignedTo = tmpAssgnTo[0].Login;
                    tmpAction.AssignedToName = tmpAssgnTo[0].Name;
                    tmpAction.AssignmentDate = tmp.AssignmentDate;
                    tmpAction.MailboxDocumentId = tmp.MailboxDocumentId;
                    tmpAction.Annotations = string.Empty;
                    db.MailboxDetailsList.Add(tmpAction);
                }
                else
                {
                    tmp.DocumentSummary = tmp.DocumentSummary + "||" + newDocAssgn.DocumentSummary;
                    tmp.DocumentStatus = "Atendido";
                    db.SaveChanges();
                    var tmpAction = new MailboxDetails();
                    tmpAction.ActionName = doc.DocumentStatus;
                    tmpAction.AssignedBy = tmp.AssignedBy;
                    tmpAction.AssignedByName = tmp.AssignedByName;
                    tmpAction.AssignmentDate = tmp.AssignmentDate;
                    tmpAction.MailboxDocumentId = tmp.MailboxDocumentId;
                    tmpAction.Annotations = doc.NewAnnotations;
                    db.MailboxDetailsList.Add(tmpAction);
                }
                db.SaveChanges();
            }
            catch (Exception ex) { }
            result = ToViewModelDo(newDocAssgn);*/
            return result;
        }

        public IncomingMailboxViewModel GetMailboxDocument(IncomingMailboxViewModel doc)
        {
            var result = new IncomingMailboxViewModel();
            /*var tmp = db.MailboxDocumentList.FirstOrDefault(item => item.SiteUrl == doc.SiteUrl && item.DocLibrary == doc.DocLibrary && item.ListItemId == doc.ListItemId);
            if (tmp != null)
            {
                if (tmp.ListItemId > 0)
                {
                    result = ToViewModel(tmp);
                }
            }*/
            return result;
        }
        public IncomingMailboxViewModel GetMailboxDocument(int docId)
        {
            var result = new IncomingMailboxViewModel();
            /*var tmp = db.MailboxDocumentList.FirstOrDefault(item => item.MailboxDocumentId == docId);
            if (tmp != null)
            {
                if (tmp.ListItemId > 0)
                {
                    result = ToViewModel(tmp);
                }
            }*/
            return result;
        }

        public List<MailboxDocument> GetMyMailboxDocuments(string assignedTo)
        {
            var result = new List<MailboxDocument>();
            /*var tmp = db.MailboxDocumentList.Where(item => item.AssignedToAccount == assignedTo).OrderByDescending(item => item.DocumentDate);
            if (tmp != null)
            {
                result = tmp.ToList();
            }*/
            return result;
        }

        public List<MailboxDocument> GetMyRelatedMailboxDocuments(string assignedTo)
        {
            var result = new List<MailboxDocument>();
            /*var tmp = db.MailboxDocumentList.Where(item => item.AssignedToAccount == assignedTo || item.AssignedToCCName.Contains(assignedTo)).OrderByDescending(item => item.DocumentDate);
            if (tmp != null)
            {
                result = tmp.ToList();
            }*/
            return result;
        }
        #endregion

        #region MailboxDetails
        public List<MailboxDetails> GetMyMailboxDetails(int documentId)
        {
            var result = new List<MailboxDetails>();
            /*var tmp = db.MailboxDetailsList.Where(item => item.MailboxDocumentId == documentId).OrderBy(item => item.AssignmentDate);
            if (tmp != null)
            {
                result = tmp.ToList();
            }*/
            return result;
        }
        #endregion

        #region Translators
        private PendingDocumentViewModel ToViewModel(DocumentAssignment doc)
        {
            var result = new PendingDocumentViewModel();
            result.AssignedBy = doc.AssignedBy;
            result.AssignedTo = doc.AssignedTo;
            result.AssignmentDate = doc.AssignmentDate;
            result.DocLibrary = doc.DocLibrary;
            result.DueTo = doc.DueTo;
            result.FirstReminderDays = doc.FirstReminderDays;
            result.ListItemId = doc.ListItemId;
            result.SecondReminderDays = doc.SecondReminderDays;
            result.SiteUrl = doc.SiteUrl;
            result.Title = doc.Title;
            result.ListItemId = doc.ListItemId;
            result.AssignmentId = doc.AssignmentId;
            result.DocumentURL = doc.DocumentURL;
            return result;
        }
        private DocumentAssignment ToModel(PendingDocumentViewModel doc)
        {
            var result = new DocumentAssignment();
            result.AssignedBy = doc.AssignedBy;
            result.AssignedTo = doc.AssignedTo;
            result.AssignmentDate = doc.AssignmentDate;
            result.DocLibrary = doc.DocLibrary;
            result.DueTo = doc.DueTo;
            result.FirstReminderDays = doc.FirstReminderDays;
            result.ListItemId = doc.ListItemId;
            result.SecondReminderDays = doc.SecondReminderDays;
            result.SiteUrl = doc.SiteUrl;
            result.Title = doc.Title;
            result.ListItemId = doc.ListItemId;
            result.AssignmentId = doc.AssignmentId;
            result.DocumentURL = doc.DocumentURL;
            return result;
        }
        private IncomingMailboxViewModel ToViewModel(MailboxDocument doc)
        {
            var result = new IncomingMailboxViewModel();
            result.DBModel.AssignedByLogin = doc.AssignedBy;
            result.DBModel.AssignedToControl = doc.AssignedTo;
            result.DBModel.AssignmentDate = doc.AssignmentDate;
            result.DBModel.DocumentLibraryId = doc.DocLibrary;
            result.DBModel.DocumentDate = doc.DocumentDate;
            result.DBModel.DocumentSubject = doc.DocumentTitle;
            result.DBModel.ListItemId = doc.ListItemId;
            result.DBModel.DocumentSummary = doc.DocumentSummary;
            result.DBModel.SiteUrl = doc.SiteUrl;
            result.DBModel.DocumentTitle = doc.Title;
            result.DBModel.ListItemId = doc.ListItemId;
            result.DBModel.MailboxDocumentId = doc.MailboxDocumentId;
            result.DBModel.DocumentUrl = doc.DocumentURL;
            result.DBModel.AssignedToCCControl = doc.AssignedToCC;
            result.DBModel.AssignmentStatus = doc.DocumentStatus;
            return result;
        }
        private MailboxDocument ToModel(IncomingMailboxViewModel doc)
        {
            var result = new MailboxDocument();
            result.AssignedBy = doc.DBModel.AssignedByLogin;
            result.AssignedTo = doc.DBModel.AssignedToCCControl;
            result.AssignmentDate = doc.DBModel.AssignmentDate;
            result.DocLibrary = doc.DBModel.DocumentLibraryId;
            result.DocumentDate = doc.DBModel.DocumentDate;
            result.DocumentTitle = doc.DBModel.DocumentSubject;
            result.ListItemId = doc.DBModel.ListItemId;
            result.DocumentSummary = doc.DBModel.DocumentSummary;
            result.SiteUrl = doc.DBModel.SiteUrl;
            result.Title = doc.DBModel.DocumentTitle;
            result.ListItemId = doc.DBModel.ListItemId;
            result.MailboxDocumentId = doc.DBModel.MailboxDocumentId;
            result.DocumentURL = doc.DBModel.DocumentUrl;
            result.AssignedToCC = doc.DBModel.AssignedToCCControl;
            result.DocumentStatus = doc.DBModel.AssignmentStatus;
            return result;
        }
        private DoIncomingViewModel ToViewModelDo(MailboxDocument doc)
        {
            var result = new DoIncomingViewModel();
            result.AssignedBy = doc.AssignedBy;
            result.AssignedTo = doc.AssignedTo;
            result.AssignmentDate = doc.AssignmentDate;
            result.DocLibrary = doc.DocLibrary;
            result.DocumentDate = doc.DocumentDate;
            result.DocumentSubject = doc.DocumentTitle;
            result.ListItemId = doc.ListItemId;
            result.DocumentSummary = doc.DocumentSummary;
            result.SiteUrl = doc.SiteUrl;
            result.Title = doc.Title;
            result.ListItemId = doc.ListItemId;
            result.AssignmentId = doc.MailboxDocumentId;
            result.DocumentURL = doc.DocumentURL;
            result.AssignedToCC = doc.AssignedToCC;
            result.DocumentStatus = doc.DocumentStatus;
            return result;
        }
        private MailboxDocument ToModel(DoIncomingViewModel doc)
        {
            var result = new MailboxDocument();
            result.AssignedBy = doc.AssignedBy;
            result.AssignedTo = doc.AssignedTo;
            result.AssignmentDate = doc.AssignmentDate;
            result.DocLibrary = doc.DocLibrary;
            result.DocumentDate = doc.DocumentDate;
            result.DocumentTitle = doc.DocumentSubject;
            result.ListItemId = doc.ListItemId;
            result.DocumentSummary = doc.DocumentSummary;
            result.SiteUrl = doc.SiteUrl;
            result.Title = doc.Title;
            result.ListItemId = doc.ListItemId;
            result.MailboxDocumentId = doc.AssignmentId;
            result.DocumentURL = doc.DocumentURL;
            result.AssignedToCC = doc.AssignedToCC;
            result.DocumentStatus = doc.DocumentStatus;
            return result;
        }
        #endregion
 
    }
}