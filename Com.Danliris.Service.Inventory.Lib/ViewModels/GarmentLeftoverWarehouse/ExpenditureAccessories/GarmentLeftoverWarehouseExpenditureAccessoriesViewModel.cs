using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class GarmentLeftoverWarehouseExpenditureAccessoriesViewModel : BasicViewModel, IValidatableObject
    {
        public string ExpenditureNo { get; set; }
        public DateTimeOffset? ExpenditureDate { get; set; }
        public string ExpenditureDestination { get; set; }
        public UnitViewModel UnitExpenditure { get; set; }
        public BuyerViewModel Buyer { get; set; }
        public string EtcRemark { get; set; }
        public string Remark { get; set; }
        public string LocalSalesNoteNo { get; set; }
        public int LocalSalesNoteId { get; set; }
        public bool IsUsed { get; set; }

        public List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel> Items { get; set; }

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


            if (string.IsNullOrWhiteSpace(ExpenditureDestination))
            {
                yield return new ValidationResult("Tujuan Pengeluaran tidak boleh kosong", new List<string> { "ExpenditureDestination" });
            }
            else
            {
                if (ExpenditureDestination == "UNIT" && UnitExpenditure == null)
                {
                    yield return new ValidationResult("Unit tidak boleh kosong", new List<string> { "UnitExpenditure" });
                }

                if (ExpenditureDestination == "JUAL LOKAL" && Buyer == null)
                {
                    yield return new ValidationResult("Buyer tidak boleh kosong", new List<string> { "Buyer" });
                }

                if (ExpenditureDestination == "JUAL LOKAL" && string.IsNullOrEmpty(LocalSalesNoteNo))
                {
                    yield return new ValidationResult("Nomor Nota Penjualan Lokal Tidak Boleh Kosong", new List<string> { "LocalSalesNoteNo" });
                }

                if (ExpenditureDestination == "LAIN-LAIN" && string.IsNullOrWhiteSpace(EtcRemark))
                {
                    yield return new ValidationResult("Keterangan Lain-lain tidak boleh kosong", new List<string> { "EtcRemark" });
                }
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

                    if (item.Product == null)
                    {
                        errorItem["Product"] = "Produk tidak boleh kosong";
                        errorCount++;
                    }

                    if (item.PONo == null)
                    {
                        errorItem["PONo"] = "PONo tidak boleh kosong";
                        errorCount++;
                    }

                    if (item.Uom == null || item.StockId == 0)
                    {
                        errorItem["Uom"] = "Satuan tidak boleh kosong";
                        errorCount++;
                    }
                    else if (Items.Count(i => i.StockId == item.StockId) > 1)
                    {
                        errorItem["Uom"] = "Satuan tidak boleh sama";
                        errorCount++;
                    }

                    if (item.Quantity <= 0)
                    {
                        errorItem["Quantity"] = "Jumlah tidak boleh 0";
                        errorCount++;
                    }
                    else if (item.StockId > 0)
                    {
                        IGarmentLeftoverWarehouseExpenditureAccessoriesService service = (IGarmentLeftoverWarehouseExpenditureAccessoriesService)validationContext.GetService(typeof(IGarmentLeftoverWarehouseExpenditureAccessoriesService));

                        var stockQuantity = service.CheckStockQuantity(Id, item.StockId);
                        if (item.Quantity > stockQuantity)
                        {
                            errorItem["Quantity"] = "Jumlah tidak boleh lebih dari " + stockQuantity;
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
