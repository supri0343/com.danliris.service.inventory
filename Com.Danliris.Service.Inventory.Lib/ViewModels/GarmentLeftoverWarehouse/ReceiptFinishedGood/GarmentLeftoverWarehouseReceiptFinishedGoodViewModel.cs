using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodViewModel
{
    public class GarmentLeftoverWarehouseReceiptFinishedGoodViewModel : BasicViewModel, IValidatableObject
    {
        public string FinishedGoodReceiptNo { get; set; }
        public UnitViewModel UnitFrom { get; set; }
        public string Invoice { get; set; }
        public string ContractNo { get; set; }
        public double Carton { get; set; }
        public string ExpenditureDesc { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? ReceiptDate { get; set; }
        public List<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UnitFrom == null)
            {
                yield return new ValidationResult("Unit Asal tidak boleh kosong", new List<string> { "UnitFrom" });
            }

            
            //else if (Id == 0)
            //{
            //    IGarmentLeftoverWarehouseReceiptFinishedGoodService service = (IGarmentLeftoverWarehouseReceiptFinishedGoodService)validationContext.GetService(typeof(IGarmentLeftoverWarehouseReceiptFinishedGoodService));
            //    var existingByExpenditureGood = service.Read(1, 1, "{}", new List<string>(), null, JsonConvert.SerializeObject(new { ExpenditureGoodNo }));
            //    if (existingByExpenditureGood.Count > 0)
            //    {
            //        yield return new ValidationResult("No Bon Pengeluaran Barang Jadi sudah dibuat Penerimaan Gudang Sisa", new List<string> { "ExpenditureGoodNo" });
            //    }
            //}

            if (ReceiptDate == null || ReceiptDate <= DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Penerimaan tidak boleh kosong", new List<string> { "ReceiptDate" });
            }
            else if (ReceiptDate > DateTimeOffset.Now)
            {
                yield return new ValidationResult("Tanggal Penerimaan tidak boleh lebih dari ini", new List<string> { "ReceiptDate" });
            }

            if (Items == null || Items.Count < 1)
            {
                yield return new ValidationResult("Items tidak boleh kosong", new List<string> { "ItemsCount" });
            }
            else if (Items.Find(a => a.LeftoverComodity == null) != null)
            {
                yield return new ValidationResult("Ada komoditi yang belum diisi", new List<string> { "LeftoverComodity" });
            }
            else if (Items.Find(a => string.IsNullOrWhiteSpace(a.ExpenditureGoodNo)) != null)
            {
                yield return new ValidationResult("Ada No Bon Pengeluaran yang belum diisi", new List<string> { "ExpenditureGoodNo" });
            }
            else
            {
                int errorCount = 0;
                List<Dictionary<string, string>> errorItems = new List<Dictionary<string, string>>();

                foreach (var item in Items)
                {
                    Dictionary<string, string> errorItem = new Dictionary<string, string>();

                    if (string.IsNullOrWhiteSpace(item.ExpenditureGoodNo))
                    {
                        errorItem["ExpenditureGoodNo"] = "No Bon Pengeluaran tidak boleh kosong";
                        errorCount++;
                    }
                    //if (item.Size == null)
                    //{
                    //    errorItem["Size"] = "Produk tidak boleh kosong";
                    //    errorCount++;
                    //}

                    //if (item.Uom == null)
                    //{
                    //    errorItem["Uom"] = "Satuan tidak boleh kosong";
                    //    errorCount++;
                    //}

                    if (item.LeftoverComodity == null)
                    {
                        errorItem["LeftoverComodity"] = "Komoditi tidak boleh kosong";
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
