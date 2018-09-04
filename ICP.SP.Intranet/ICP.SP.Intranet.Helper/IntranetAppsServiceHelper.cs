using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICP.SP.Intranet.Models.Service;
using System.Net.Http;
using System.Net;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Configuration;
using System.Web;
using ICP.SP.Intranet.Models.Common;

namespace ICP.SP.Intranet.Helper
{
    public class IntranetAppsServiceHelper
    {
        private string baseEndpoint = ConfigurationManager.AppSettings["ServiceApiServer"];
        public async Task<string> GetMyPendingDocAssignments(string accountName)
        {
            string actionEndpoint = "docassignment/MyPendingDocumentAssignment";
            string queryParameter = string.Format("?accountName={0}", HttpUtility.UrlEncode(accountName));
            GetMyPendingDocumentAssignmentResponse serviceResult = new GetMyPendingDocumentAssignmentResponse();
            List<DocumentAssignmentModel> result = new List<DocumentAssignmentModel>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<GetMyPendingDocumentAssignmentResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ModelList;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetDocumentAssignment(DocumentAssignmentModel document)
        {
            string actionEndpoint = "docassignment/DocumentAssignment";
            string queryParameter = string.Format("?DocumentAssignmentId={0}&SiteUrl={1}&DocumentLibraryId={2}&ListItemId={3}&DocumentStatus={4}", document.DocumentAssignmentId, document.SiteUrl, document.DocumentLibraryId, document.ListItemId, document.AssignmentStatus);
            GetDocumentAssignmentResponse serviceResult = new GetDocumentAssignmentResponse();
            DocumentAssignmentModel result = new DocumentAssignmentModel();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<GetDocumentAssignmentResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.Model;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> PostDocumentAssignment(DocumentAssignmentModel document)
        {
            var result = 0;
            string actionEndpoint = "docassignment/DocAssignment";
            PostEntityModelResponse serviceResult = new PostEntityModelResponse();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.PostAsJsonAsync(request.RequestUri, document))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<PostEntityModelResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ItemId;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetMyPendingInMailbox(string accountName)
        {
            string actionEndpoint = "inmailbox/MyPendingInMailbox";
            string queryParameter = string.Format("?accountName={0}", HttpUtility.UrlEncode(accountName));
            GetMyPendingIncomingMailboxResponse serviceResult = new GetMyPendingIncomingMailboxResponse();
            List<InboxDocumentModel> result = new List<InboxDocumentModel>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<GetMyPendingIncomingMailboxResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ModelList;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetSearchInMailbox(OutDocumentFilterModel document)
        {
            string actionEndpoint = "inmailbox/SearchInMailbox";
            string queryParameter = string.Format("?comp={0}&text={1}&from={2}&to={3}", string.Empty, HttpUtility.UrlEncode(document.SearchText), document.SearchDateFrom != null ? document.SearchDateFrom.Value.ToString("yyyy-MM-dd") : string.Empty, document.SearchDateTo != null ? document.SearchDateTo.Value.ToString("yyyy-MM-dd") : string.Empty);
            GetMyPendingIncomingMailboxResponse serviceResult = new GetMyPendingIncomingMailboxResponse();
            List<InboxDocumentModel> result = new List<InboxDocumentModel>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<GetMyPendingIncomingMailboxResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ModelList;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetInMailbox(int documentId)
        {
            string actionEndpoint = "inmailbox/IncomingMailbox";
            string queryParameter = string.Format("?docId={0}", documentId);
            GetIncomingMailboxResponse serviceResult = new GetIncomingMailboxResponse();
            InboxDocumentModel result = new InboxDocumentModel();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<GetIncomingMailboxResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.Model;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> PostInMailbox(MailboxDocumentModel document)
        {
            var result = 0;
            string actionEndpoint = "inmailbox/InMailbox";
            PostEntityModelResponse serviceResult = new PostEntityModelResponse();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.PostAsJsonAsync(request.RequestUri, document))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<PostEntityModelResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ItemId;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> PostInMailbox(InboxDocumentModel document, string temp)
        {
            var result = 0;
            string actionEndpoint = "inmailbox/InMailbox";
            PostEntityModelResponse serviceResult = new PostEntityModelResponse();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.PostAsJsonAsync(request.RequestUri, document))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<PostEntityModelResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ItemId;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetInMailboxDetails(int id)
        {
            string actionEndpoint = "inmailboxdetails/InMailboxDetails";
            string queryParameter = string.Format("?id={0}", id);
            GetIncomingMailboxDetailsResponse serviceResult = new GetIncomingMailboxDetailsResponse();
            List<MailboxDocumentDetailsModel> result = new List<MailboxDocumentDetailsModel>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<GetIncomingMailboxDetailsResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ModelList;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> PostDocumentCoding(DocumentCodingModel document)
        {
            var result = string.Empty;
            string actionEndpoint = "documentcoding/documentcoding";
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.PostAsJsonAsync(request.RequestUri, document))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            result = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetMyPendingOutMailbox(OutDocumentFilterModel document)
        {
            string actionEndpoint = "outmailbox/MyPendingOutMailbox";
            string queryParameter = string.Format("?comp={0}&text={1}&from={2}&to={3}", string.Empty, HttpUtility.UrlEncode(document.SearchText), document.SearchDateFrom != null ? document.SearchDateFrom.Value.ToString("yyyy-MM-dd") : string.Empty, document.SearchDateTo != null ? document.SearchDateTo.Value.ToString("yyyy-MM-dd") : string.Empty);
            GetMyPendingOutcomingMailboxResponse serviceResult = new GetMyPendingOutcomingMailboxResponse();
            List<OutDocumentModel> result = new List<OutDocumentModel>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<GetMyPendingOutcomingMailboxResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ModelList;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> PostOutMailbox(OutDocumentModel document)
        {
            var result = string.Empty;
            string actionEndpoint = "outmailbox/OutMailbox";
            PostEntityModelResponse serviceResult = new PostEntityModelResponse();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.PostAsJsonAsync(request.RequestUri, document))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<PostEntityModelResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ItemId.ToString();
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetDocumentCode(string companyName, string departmentName, int year)
        {
            var result = string.Empty;
            string actionEndpoint = "outmailbox/GenerateCode";
            string queryParameter = string.Format("?comp={0}&dept={1}&year={2}", HttpUtility.UrlEncode(companyName), HttpUtility.UrlEncode(departmentName), year);
            PostEntityModelResponse serviceResult = new PostEntityModelResponse();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<PostEntityModelResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.ItemCode;
                        }
                    }
                }
            }
            return result;
        }

        public async Task<string> GetOutMailbox(OutDocumentModel document)
        {
            string actionEndpoint = "outmailbox/OutcomingMailbox";
            string queryParameter = string.Format("?DocumentId={0}", document.DocumentId);
            GetOutcomingMailboxResponse serviceResult = new GetOutcomingMailboxResponse();
            OutDocumentModel result = new OutDocumentModel();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, baseEndpoint + actionEndpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            serviceResult = JsonConvert.DeserializeObject<GetOutcomingMailboxResponse>(response.Content.ReadAsStringAsync().Result);
                            result = serviceResult.Model;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}
