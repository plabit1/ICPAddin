/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
//using Resources;

namespace ICP.SP.IntranetWeb.Models
{
    public class GraphService
    {
        public async Task<string> GetMyPlannerTasks(string accessToken)
        {
            string endpoint = "https://graph.microsoft.com/beta/tasks";
            string queryParameter = "?$filter=assignedTo eq 'me'&$orderby=title";
            List<TaskInfo> lstMyTasks = new List<TaskInfo>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                            foreach (var jObj in json.GetValue("value"))
                                if (jObj.HasValues)
                                {
                                    TaskInfo aTask = new TaskInfo();
                                    aTask.Title = jObj["title"].ToString();
                                    aTask.StartDate = string.IsNullOrEmpty(jObj["startDateTime"].ToString()) ? DateTime.MinValue: Convert.ToDateTime(jObj["startDateTime"].ToString());
                                    aTask.DueDate = string.IsNullOrEmpty(jObj["dueDateTime"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(jObj["dueDateTime"].ToString());
                                    aTask.PercentComplete = string.IsNullOrEmpty(jObj["percentComplete"].ToString()) ? 0 : Convert.ToInt32(jObj["percentComplete"].ToString());
                                    if (aTask.PercentComplete < 100)
                                    {
                                        if (aTask.DueDate == DateTime.MinValue)
                                            aTask.Indicator = "2Yellow";
                                        else
                                        {
                                            if (aTask.DueDate <= DateTime.Today)
                                                aTask.Indicator = "1Red";
                                            else
                                            {
                                                if (aTask.DueDate > DateTime.Today.AddDays(5))
                                                    aTask.Indicator = "3Green";
                                                else
                                                    aTask.Indicator = "2Yellow";
                                            }
                                        }
                                    }
                                    else
                                        aTask.Indicator = "4Gray";
                                    lstMyTasks.Add(aTask);
                                }
                        }
                        return JsonConvert.SerializeObject(lstMyTasks);
                    }
                }
            }
        }

        public async Task<string> GetMyOutlookTasks(string accessToken)
        {
            string endpoint = "https://outlook.office.com/api/v2.0/me/tasks";
            string queryParameter = "?filter=Status neq 'Completed'";
            List<TaskInfo> lstMyTasks = new List<TaskInfo>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                            foreach (var jObj in json.GetValue("value"))
                                if (jObj.HasValues)
                                {
                                    TaskInfo aTask = new TaskInfo();
                                    aTask.Title = jObj["Subject"].ToString();
                                    aTask.StartDate = string.IsNullOrEmpty(jObj["StartDateTime"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(jObj["StartDateTime"]["DateTime"].ToString());
                                    aTask.DueDate = string.IsNullOrEmpty(jObj["DueDateTime"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(jObj["DueDateTime"]["DateTime"].ToString());
                                    //aTask.PercentComplete = string.IsNullOrEmpty(jObj["percentComplete"].ToString()) ? 0 : Convert.ToInt32(jObj["percentComplete"].ToString());
                                    aTask.PercentComplete = string.IsNullOrEmpty(jObj["CompletedDateTime"].ToString()) ? 0:100;
                                    if (aTask.PercentComplete < 100)
                                    {
                                        if (aTask.DueDate == DateTime.MinValue)
                                            aTask.Indicator = "2Yellow";
                                        else
                                        {
                                            if (aTask.DueDate <= DateTime.Today)
                                                aTask.Indicator = "1Red";
                                            else
                                            {
                                                if (aTask.DueDate > DateTime.Today.AddDays(5))
                                                    aTask.Indicator = "3Green";
                                                else
                                                    aTask.Indicator = "2Yellow";
                                            }
                                        }
                                    }
                                    else
                                        aTask.Indicator = "4Gray";
                                    lstMyTasks.Add(aTask);
                                }
                        }
                        return JsonConvert.SerializeObject(lstMyTasks);
                    }
                }
            }
        }

        public async Task<string> GetMyOutlookCalendar(string accessToken)
        {
            string endpoint = "https://graph.microsoft.com/v1.0/me/calendar/calendarView";
            string queryParameter = string.Format("?startDateTime={0:yyyy-MM-ddT00:00:00.0000000}&endDateTime={1:yyyy-MM-ddT00:00:00.0000000}", DateTime.Today, DateTime.Today.AddDays(2));
            List<EventInfo> lstMyEvents = new List<EventInfo>();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint + queryParameter))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                            foreach (var jObj in json.GetValue("value"))
                                if (jObj.HasValues)
                                {
                                    EventInfo anEvent = new EventInfo();
                                    anEvent.Title = jObj["subject"].ToString();
                                    anEvent.StartDate = string.IsNullOrEmpty(jObj["start"].ToString()) ? DateTime.MinValue : TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(jObj["start"]["dateTime"].ToString()), TimeZoneInfo.FindSystemTimeZoneById(jObj["originalStartTimeZone"].ToString()));
                                    anEvent.EndDate = string.IsNullOrEmpty(jObj["end"].ToString()) ? DateTime.MinValue : TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(jObj["end"]["dateTime"].ToString()), TimeZoneInfo.FindSystemTimeZoneById(jObj["originalEndTimeZone"].ToString()));
                                    anEvent.Location = string.IsNullOrEmpty(jObj["location"].ToString()) ? string.Empty : jObj["location"]["displayName"].ToString();
                                    anEvent.IsAllDay = string.IsNullOrEmpty(jObj["isAllDay"].ToString()) ? false : Convert.ToBoolean(jObj["isAllDay"].ToString());
                                    lstMyEvents.Add(anEvent);
                                }
                        }
                        
                        return JsonConvert.SerializeObject(lstMyEvents);
                    }
                }
            }
        }

        public async Task<string> GetRoomSuggestedTime(Intranet.Models.Common.RoomManagementModel model)
        {
            string endpoint = "https://graph.microsoft.com/v1.0/me/findMeetingTimes";
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", model.AccessToken);
                    request.Headers.Add("Prefer", "outlook.timezone=\"SA Pacific Standard Time\"");
                    request.Content = new StringContent("{ \"attendees\": [{ \"type\": \"resource\", \"emailAddress\": { \"name\": \"Sala Prueba\", \"address\": \"salaprueba@kg.com.pe\" } }],\"locationConstraint\": { \"isRequired\": \"true\", \"suggestLocation\": \"false\",\"locations\": [ { \"locationEmailAddress\": \"salaprueba@kg.com.pe\" }]}, \"timeConstraint\": {\"activityDomain\":\"unrestricted\",  \"timeslots\": [{ \"start\": { \"dateTime\": \"" + model.StartDateTime + "\", \"timeZone\": \"SA Pacific Standard Time\" }, \"end\": { \"dateTime\": \"" + model.EndDateTime + "\", \"timeZone\": \"SA Pacific Standard Time\" } } ] }, \"meetingDuration\": \"" + model.Duration + "\", \"returnSuggestionReasons\": \"true\"}", Encoding.UTF8, "application/json");
                    //request.Content = new StringContent("{ \"attendees\": [{ \"type\": \"resource\", \"emailAddress\": { \"name\": \"Sala Prueba\", \"address\": \"salaprueba@kg.com.pe\" } }, { \"type\": \"resource\", \"emailAddress\": { \"name\": \"Piso 6 - Recepcion\", \"address\": \"piso6_Recepcion@kg.com.pe\" } },   { \"type\": \"resource\", \"emailAddress\": { \"name\": \"Piso 6 - Sala Reuniones\", \"address\": \"piso6_SalaReuniones@kg.com.pe\" } } ],\"locationConstraint\": { \"isRequired\": \"true\", \"suggestLocation\": \"false\",\"locations\": [{ \"locationEmailAddress\": \"piso6_Recepcion@kg.com.pe\" }, { \"locationEmailAddress\": \"salaprueba@kg.com.pe\" },{ \"locationEmailAddress\": \"piso6_SalaReuniones@kg.com.pe\" }]}, \"timeConstraint\": {\"activityDomain\":\"unrestricted\",  \"timeslots\": [{ \"start\": { \"dateTime\": \"" + model.StartDateTime + "\", \"timeZone\": \"SA Pacific Standard Time\" }, \"end\": { \"dateTime\": \"" + model.EndDateTime + "\", \"timeZone\": \"SA Pacific Standard Time\" } } ] }, \"meetingDuration\": \"" + model.Duration + "\", \"returnSuggestionReasons\": \"true\"}", Encoding.UTF8, "application/json");
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                            var suggestions = json.GetValue("meetingTimeSuggestions");
                            var roomItem = suggestions.First;
                            if(roomItem != null)
                            {
                                result = roomItem.Last.First.First["locationEmailAddress"].ToString();
                            }
                        }

                        return JsonConvert.SerializeObject(result);
                    }
                }
            }
        }

        public async Task<string> CreateEvent(Intranet.Models.Common.CreateEventModel model)
        {
            string endpoint = "https://graph.microsoft.com/v1.0/me/events";
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", model.AccessToken);
                    request.Headers.Add("Prefer", "outlook.timezone=\"SA Pacific Standard Time\"");
                    request.Content = new StringContent("{ \"subject\": \"Nuevo evento\",\"start\": { \"dateTime\": \"" + model.StartDateTime + "\", \"timeZone\": \"SA Pacific Standard Time\" }, \"end\": { \"dateTime\": \"" + model.EndDateTime + "\", \"timeZone\": \"SA Pacific Standard Time\" } , \"location\": {\"displayName\":\"Sala Prueba\", \"locationEmailAddress\":\"" + model.RoomEmail + "\"}, \"attendees\": [ { \"type\": \"resource\", \"emailAddress\": { \"address\": \"" + model.RoomEmail + "\"}} ]}", Encoding.UTF8, "application/json");

                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                            result = json.GetValue("webLink").ToString();
                        }

                        return JsonConvert.SerializeObject(result);
                    }
                }
            }
        }
    }
}