using ICP.SP.IntranetWeb.Models;
using ICP.SP.IntranetWeb.ViewModels;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
    public class OutlookCalendarController : Controller
    {
        [Authorize]
        public async Task<ActionResult> Index()
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
                var tmpResult = await graphService.GetMyOutlookCalendar(authResult.AccessToken);
                objResult.EventInfoList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EventInfo>>(tmpResult);
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
            return View(objResult);
        }
    }
}
