using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report
{
    public class BalanceReportPerPieceViewModel
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
        public string Piece { get; set; }
        public string Grade { get; set; }

        public double ReceiptBalance { get; set; }
        public double ReceiptPieceQuantity { get; set; }
        public double ExpendBalance { get; set; }
        public double ExpendPieceQuantity { get; set; }
        public string Area { get; set; }
        public int Nomor { get; set; }
        public double BeginingBalance { get; set; }
        public double BeginingBalancePiece { get; set; }
        public double EndingBalance { get; set; }
        public double EndingBalancePiece { get; set; }
    }

    public class BalanceReportPerPieceAllViewModel : BasicViewModel
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
        public double QuantityPiece { get; set; }
        public string Piece { get; set; }
        public string Grade { get; set; }
        public string Type { get; set; }
        public int IdMove { get; set; }
        public int IdDoc { get; set; }


    }
}
