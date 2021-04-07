using Com.Danliris.Service.Inventory.Lib.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel : BasicViewModel, IValidatableObject
    {
        public string FinishedGoodExpenditureNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string ExpenditureTo { get; set; }
        public BuyerViewModel Buyer { get; set; }
        public string OtherDescription { get; set; }
        public bool Consignment { get; set; }
        public string Description { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public int LocalSalesNoteId { get; set; }
        public virtual List<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExpenditureDate == null || ExpenditureDate <= DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Pengeluaran tidak boleh kosong", new List<string> { "ExpenditureDate" });
            }
            else if (ExpenditureDate > DateTimeOffset.Now)
            {
                yield return new ValidationResult("Tanggal Pengeluaran tidak boleh lebih dari ini", new List<string> { "ExpenditureDate" });
            }

            if(ExpenditureTo== "LAIN-LAIN" && string.IsNullOrWhiteSpace(OtherDescription))
            {
                yield return new ValidationResult("Keterangan Lain-lain harus diisi", new List<string> { "OtherDescription" });
            }

            if(ExpenditureTo == "JUAL LOKAL" && Buyer == null)
            {
                yield return new ValidationResult("Buyer tidak boleh kosong", new List<string> { "Buyer" });
            }

            if (ExpenditureTo == "JUAL LOKAL" && string.IsNullOrEmpty(LocalSalesNoteNo))
            {
                yield return new ValidationResult("Nomor Nota Penjualan Lokal Tidak Boleh Kosong", new List<string> { "LocalSalesNoteNo" });
            }

            if (Items == null || Items.Count < 1)
            {
                yield return new ValidationResult("Items tidak boleh kosong", new List<string> { "ItemsCount" });
            }
            else
            {
                int errorCount = 0;
                List<Dictionary<string, string>> errorItems = new List<Dictionary<string, string>>();

                foreach (var item in Items)
                {
                    Dictionary<string, string> errorItem = new Dictionary<string, string>();

                    if (item.LeftoverComodity == null)
                    {
                        errorItem["LeftoverComodity"] = "Komoditi tidak boleh kosong";
                        errorCount++;
                    }

                    if (item.Unit == null)
                    {
                        errorItem["Unit"] = "Unit tidak boleh kosong";
                        errorCount++;
                    }

                    if (string.IsNullOrEmpty(item.RONo))
                    {
                        errorItem["RONo"] = "No RO tidak boleh kosong";
                        errorCount++;
                    }

                    if (item.ExpenditureQuantity <= 0)
                    {
                        errorItem["ExpenditureQuantity"] = "Jumlah Pengeluaran harus lebih dari 0";
                        errorCount++;
                    }
                    else if (item.ExpenditureQuantity > item.StockQuantity)
                    {
                        errorItem["ExpenditureQuantity"] = $"Jumlah Pengeluaran tidak boleh lebih dari {item.StockQuantity}";
                        errorCount++;
                    }

                    errorItems.Add(errorItem);
                }

                if (errorCount > 0)
                {
                    yield return new ValidationResult(JsonConvert.SerializeObject(errorItems), new List<string> { "Items" });
                }
            }
        }
    }
}
