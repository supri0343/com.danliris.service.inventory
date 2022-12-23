using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalViewModels;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ReceiptWaste
{
    public class GarmentReceiptWasteProductionViewModel : BasicViewModel, IValidatableObject
    {
        public string GarmentReceiptWasteNo { get; set; }
        public string SourceName { get; set; }
        public string DestinationName { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }
        public string WasteType { get; set; }
        public string Remark { get; set; }
        public double TotalAval { get; set; }
        public bool IsUsed { get; set; }

        public List<GarmentReceiptWasteProductionItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(WasteType))
            {
                yield return new ValidationResult("Tipe Tidak Boleh Kosong", new List<string> { "WasteType" });
            }
            if (TotalAval == 0)
            {
                yield return new ValidationResult("Jumlah Tidak Boleh Kosong", new List<string> { "TotalAval" });
            }
            if (ReceiptDate == null || ReceiptDate <= DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Penerimaan tidak boleh kosong", new List<string> { "ReceiptDate" });
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
                    if (string.IsNullOrWhiteSpace(item.RONo))
                    {
                        errorItem["RONo"] = "Nomor RO tidak boleh kosong";
                        errorCount++;
                    }
                    else if (item.RONo == "error")
                    {
                        errorItem["RONo"] = "Item harus dipilih";
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
