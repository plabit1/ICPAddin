using ICP.SP.Intranet.Helper;
using ICP.SP.IntranetWeb.Models;
using ICP.SP.IntranetWeb.ViewModels;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ICP.SP.IntranetWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            User spUser = null;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    ViewBag.UserName = spUser.Title;
                }
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> About()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Message = "Welcome " + User.Identity.Name;
            }
            else
            {
                ViewBag.Message = "We are Anonymous.  We are Legion.";
            }
            
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> About(DateTime fecha, string horasFI, string minutosFI, string ampm, string horasD, string minutosD, string todoDia)
        {
            var objResult = new EventsViewModel();
            string clientID = ConfigurationManager.AppSettings["ida:ClientID"];
            string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
            string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
            string clientSecret = ConfigurationManager.AppSettings["ida:AppKey"];
            string graphResourceID = ConfigurationManager.AppSettings["ida:GraphResourceID"];
            string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            if (Request.QueryString["SPLanguage"] != null)
                objResult.CurrentCulture = new CultureInfo(Request.QueryString["SPLanguage"]);
            else
                objResult.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            AuthenticationContext authContext = new AuthenticationContext(authority, new ADALTokenCache(signInUserId));
            try
            {
                var authResult = await authContext.AcquireTokenSilentAsync(graphResourceID, new ClientCredential(clientID, clientSecret), new UserIdentifier(userObjectId, UserIdentifierType.UniqueId));
                GraphService graphService = new GraphService();
                var objRoom = new Intranet.Models.Common.RoomManagementModel();
                objRoom.AccessToken = authResult.AccessToken;
                if (ampm == "pm")
                {
                    horasFI = (12 + int.Parse(horasFI)).ToString();
                }
                objRoom.StartDateTime = string.Format("{0:yyyy-MM-dd}T{1}:{2}:00", fecha, horasFI, minutosFI);
                int horaFin = int.Parse(horasFI) + int.Parse(horasD);
                int minutosFin = int.Parse(minutosFI) + int.Parse(minutosD);
                if (minutosFin == 60)
                {
                    minutosFin = 0;
                    horaFin++;
                }
                objRoom.EndDateTime = string.Format("{0:yyyy-MM-dd}T{1:00}:{2:00}:00", fecha, horaFin, minutosFin);
                objRoom.Duration = "PT";
                if (horasD != "0")
                    objRoom.Duration = string.Format("{0}{1}H", objRoom.Duration, horasD);
                if (minutosD != "00")
                    objRoom.Duration = string.Format("{0}{1}M", objRoom.Duration, minutosD);
                var tmpResult = await graphService.GetRoomSuggestedTime(objRoom);
                var roomAddress = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(tmpResult);
                if (!string.IsNullOrEmpty(roomAddress))
                {
                    var objMeeting = new Intranet.Models.Common.CreateEventModel();
                    objMeeting.AccessToken = objRoom.AccessToken;
                    objMeeting.EndDateTime = objRoom.EndDateTime;
                    objMeeting.RoomEmail = roomAddress;
                    objMeeting.StartDateTime = objRoom.StartDateTime;
                    tmpResult = await graphService.CreateEvent(objMeeting);
                    ViewBag.MeetingUrl = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(tmpResult).Replace("&amp;","&");
                }
            }
            catch (AdalException exception)
            {
                objResult.Message = "No se puede obtener sus datos de sesión. Cierre la ventana y vuelva a ingresar";
                //handle token acquisition failure
                if (exception.ErrorCode == AdalError.FailedToAcquireTokenSilently)
                {
                    authContext.TokenCache.Clear();
                    //handle token acquisition failure
                }
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
