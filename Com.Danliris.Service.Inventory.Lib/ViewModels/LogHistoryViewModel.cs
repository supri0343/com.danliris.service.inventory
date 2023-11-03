using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels
{
    public class LogHistoryViewModel
    {
        public string Division { get; set; }
        public string Activity { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }

    public class LogHistoryViewModelToUI
    {
        public string Division { get; set; }
        public string Activity { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Date { get; set; }
        public string Time { get; set; }
        public string CreatedBy { get; set; }
    }
}
