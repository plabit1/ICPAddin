using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ICP.SP.IntranetWeb.Models;

namespace ICP.SP.IntranetWeb.ViewModels
{
    public class TaskViewModel
    {
        public string Message { get; set; }
        public List<TaskInfo> TaskInfoList { get; set; }
        public System.Globalization.CultureInfo CurrentCulture { get; set; }
    }
}