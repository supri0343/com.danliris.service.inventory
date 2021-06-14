using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report
{
    public class ReportGreigeWeavingPerMonthViewModel
    {
        public string MaterialName { get; set; }
        public string WovenType { get; set; }
        public string Yarn1 { get; set; }
        public string Yarn2 { get; set; }
        public string YarnType1 { get; set; }
        public string YarnType2 { get; set; }
        public string YarnOrigin1 { get; set; }
        public string YarnOrigin2 { get; set; }
        public string Width { get; set; }
        public string Construction { get; set; }

        public double ReceiptProduction { get; set; }
        public double ReceiptRecheking { get; set; }
        public double ReceiptFinishing { get; set; }
        public double ReceiptPrinting { get; set; }
        public double ReceiptPacking { get; set; }
        public double ReceiptOther { get; set; }
        public double ExpendPacking { get; set; }
        public double ExpendFinishing { get; set; }
        public double ExpendPrinting { get; set; }
        public double ExpendInspecting { get; set; }
        public double ExpendDirty { get; set; }
        public double ExpendOther { get; set; }
        public string Area { get; set; }
        public int Nomor { get; set; }
        public double BeginingBalance { get; set; }
        public double EndingBalance { get; set; }

    }

    public class ReportGreigeWeavingAllDataViewModel : BasicViewModel
    {
        public string MaterialName { get; set; }
        public string WovenType { get; set; }
        public string Yarn1 { get; set; }
        public string Yarn2 { get; set; }
        public string YarnType1 { get; set; }
        public string YarnType2 { get; set; }
        public string YarnOrigin1 { get; set; }
        public string YarnOrigin2 { get; set; }
        public string Width { get; set; }
        public string Construction { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Area { get; set; }
        public double Quantity { get; set; }
        public string Type { get; set; }
        public int IdMove { get; set; }
        public int IdDoc { get; set; }


    }
}
