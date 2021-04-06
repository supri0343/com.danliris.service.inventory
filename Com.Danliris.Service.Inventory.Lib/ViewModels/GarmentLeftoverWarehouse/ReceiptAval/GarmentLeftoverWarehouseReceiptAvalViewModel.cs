using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalViewModels
{
    public class GarmentLeftoverWarehouseReceiptAvalViewModel : BasicViewModel, IValidatableObject
    {
        public string AvalReceiptNo { get; set; }

        public UnitViewModel UnitFrom { get; set; }
        public DateTimeOffset? ReceiptDate { get; set; }
        public string AvalType { get; set; }
        public string Remark { get; set; }
        public double TotalAval { get; set; }
        public List<GarmentLeftoverWarehouseReceiptAvalItemViewModel> Items { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UnitFrom == null)
            {
                yield return new ValidationResult("Unit Asal tidak boleh kosong", new List<string> { "UnitFrom" });
            }

            if (ReceiptDate == null || ReceiptDate <= DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Penerimaan tidak boleh kosong", new List<string> { "ReceiptDate" });
            }
            else if (ReceiptDate > DateTimeOffset.Now)
            {
                yield return new ValidationResult("Tanggal Penerimaan tidak boleh lebih dari ini", new List<string> { "ReceiptDate" });
            }
            if (AvalType=="AVAL FABRIC" && TotalAval <= 0)
            {
                yield return new ValidationResult("Total Aval Fabric (KG) harus lebih dari 0", new List<string> { "TotalAval" });
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
                    if(AvalType == "AVAL FABRIC")
                    {
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
                    }
                    else if(AvalType == "AVAL BAHAN PENOLONG")
                    {
                        if (item.Product == null)
                        {
                            errorItem["Product"] = "Barang tidak boleh kosong";
                            errorCount++;
                        }
                        if (item.Uom == null)
                        {
                            errorItem["Uom"] = "Satuan tidak boleh kosong";
                            errorCount++;
                        }
                        if (item.Quantity <=0)
                        {
                            errorItem["Quantity"] = "Jumlah harus lebih dari 0";
                            errorCount++;
                        }
                    }else if(AvalType == "AVAL KOMPONEN")
                    {
                        if (item.AvalComponentNo == null)
                        {
                            errorItem["AvalComponentNo"] = "No Aval komponen tidak boleh kosong";
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
