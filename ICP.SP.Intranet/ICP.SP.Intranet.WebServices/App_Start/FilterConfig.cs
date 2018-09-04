using System.Web;
using System.Web.Mvc;

namespace ICP.SP.Intranet.WebServices
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
