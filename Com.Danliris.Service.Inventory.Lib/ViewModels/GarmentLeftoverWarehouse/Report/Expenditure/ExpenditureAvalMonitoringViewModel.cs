using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Expenditure
{
    public class ExpenditureAvalMonitoringViewModel
    {
        public int index { get; set; }
        public string ExpenditureNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string AvalType { get; set; }
        public string ExpenditureTo { get; set; }
        public string OtherDescription { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public string AvalReceiptNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitCode { get; set; }
        public string UomUnit { get; set; }
        public double Quantity { get; set; }
    }
}
