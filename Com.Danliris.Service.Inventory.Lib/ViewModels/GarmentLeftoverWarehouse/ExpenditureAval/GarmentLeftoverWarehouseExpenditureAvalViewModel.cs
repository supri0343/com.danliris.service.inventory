using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class GarmentLeftoverWarehouseExpenditureAvalViewModel : BasicViewModel, IValidatableObject
    {
        public string AvalExpenditureNo { get; set; }
        public DateTimeOffset? ExpenditureDate { get; set; }
        public string ExpenditureTo { get; set; }
        public string AvalType { get; set; }
        public BuyerViewModel Buyer { get; set; }
        public string OtherDescription { get; set; }
        public string Description { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public int LocalSalesNoteId { get; set; }
        public virtual List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExpenditureDate == null || ExpenditureDate == DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Pengeluaran tidak boleh kosong", new List<string> { "ExpenditureDate" });
            }
            else if (ExpenditureDate > DateTimeOffset.Now)
            {
                yield return new ValidationResult("Tanggal Pengeluaran tidak boleh lebih dari hari ini", new List<string> { "ExpenditureDate" });
            }

            if (ExpenditureTo == "LAIN-LAIN" && string.IsNullOrWhiteSpace(OtherDescription))
            {
                yield return new ValidationResult("Keterangan Lain-lain harus diisi", new List<string> { "OtherDescription" });
            }

            if (ExpenditureTo == "JUAL LOKAL" && Buyer == null)
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

                    if (item.Unit == null)
                    {
                        errorItem["Unit"] = "Unit tidak boleh kosong";
                        errorCount++;
                    }


                    if(AvalType=="AVAL FABRIC" || AvalType == "AVAL KOMPONEN")
                    {
                        if (string.IsNullOrEmpty(item.AvalReceiptNo))
                        {
                            errorItem["AvalReceiptNo"] = "No Bon tidak boleh kosong";
                            errorCount++;
                        }

                        if (item.ActualQuantity <= 0)
                        {
                            errorItem["ActualQuantity"] = "Jumlah Pengeluaran harus lebih dari 0";
                            errorCount++;
                        }
                    }
                    else
                    {
                        if (item.Quantity <= 0)
                        {
                            errorItem["Quantity"] = "Jumlah Pengeluaran harus lebih dari 0";
                            errorCount++;
                        }
                        else if (item.Quantity > item.StockQuantity)
                        {
                            errorItem["Quantity"] = $"Jumlah Pengeluaran tidak boleh lebih dari {item.StockQuantity}";
                            errorCount++;
                        }

                        if (item.Uom == null)
                        {
                            errorItem["Uom"] = "Satuan tidak boleh kosong";
                            errorCount++;
                        }

                        if (item.Product == null)
                        {
                            errorItem["Product"] = "Barang tidak boleh kosong";
                            errorCount++;
                        }
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
