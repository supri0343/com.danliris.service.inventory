using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ReceiptAccessories
{
    public class GarmentLeftoverWarehouseReceiptAccessoriesViewModel : BasicViewModel, IValidatableObject
    {
        public string InvoiceNoReceive { get; set; }
        public string RequestUnitCode { get; set; }
        public string RequestUnitName { get; set; }
        public int RequestUnitId { get; set; }
        public UnitViewModel RequestUnit { get; set; }
        public string UENNo { get; set; }
        public long UENid { get; set; }
        //public int UENId { get; set; }
        public string StorageFromCode { get; set; }
        public string StorageFromId { get; set; }
        public string StorageFromName { get; set; }
        public StorageViewModel Storage { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public DateTimeOffset StorageReceiveDate { get; set; }
        public string Remark { get; set; }
        public List<GarmentLeftoverWarehouseReceiptAccessoriesItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RequestUnit == null)
            {
                yield return new ValidationResult("Unit Asal tidak boleh kosong", new List<string> { "UnitFrom" });
            }

            if (string.IsNullOrWhiteSpace(UENNo))
            {
                yield return new ValidationResult("No Bon Pengeluaran Unit tidak boleh kosong", new List<string> { "UENNo" });
            }
            else if (Id == 0)
            {
                IGarmentLeftoverWarehouseReceiptAccessoriesService service = (IGarmentLeftoverWarehouseReceiptAccessoriesService)validationContext.GetService(typeof(IGarmentLeftoverWarehouseReceiptAccessoriesService));
                var existingByUENNo = service.Read(1, 1, "{}", new List<string>(), null, JsonConvert.SerializeObject(new { UENNo }));
                if (existingByUENNo.Count > 0)
                {
                    yield return new ValidationResult("No Bon Pengeluaran Unit sudah dibuat Penerimaan Gudang Sisa", new List<string> { "UENNo" });
                }
            }

            if (StorageReceiveDate == null || StorageReceiveDate <= DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Penerimaan tidak boleh kosong", new List<string> { "ReceiptDate" });
            }
            else if (StorageReceiveDate > DateTimeOffset.Now)
            {
                yield return new ValidationResult("Tanggal Penerimaan tidak boleh lebih dari ini", new List<string> { "ReceiptDate" });
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

                    if (item.Product == null)
                    {
                        errorItem["Product"] = "Produk tidak boleh kosong";
                        errorCount++;
                    }

                    if (item.Uom == null)
                    {
                        errorItem["Uom"] = "Satuan tidak boleh kosong";
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
