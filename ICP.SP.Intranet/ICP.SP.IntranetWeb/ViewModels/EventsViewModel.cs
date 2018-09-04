using ICP.SP.IntranetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class EventsViewModel
    {
        public string Message { get; set; }
        public List<EventInfo> EventInfoList { get; set; }
        public System.Globalization.CultureInfo CurrentCulture { get; set; }
    }
}