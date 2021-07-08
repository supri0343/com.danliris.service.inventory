using Com.Danliris.Service.Inventory.Lib.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.BalanceStock
{
    public class GarmentLeftoverWarehouseBalanceStockViewModel : BasicViewModel, IValidatableObject
    {
        public DateTimeOffset? BalanceStockDate { get; set; }
        public string TypeOfGoods { get; set; }
        public List<GarmentLeftoverWarehouseBalanceStockItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BalanceStockDate == null || BalanceStockDate == DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Balance Stok tidak boleh kosong", new List<string> { "BalanceStockDate" });
            }

            if (string.IsNullOrWhiteSpace(TypeOfGoods))
            {
                yield return new ValidationResult("Jenis Barang tidak boleh kosong", new List<string> { "TypeOfGoods" });
            }

            if (Items == null || Items.Count < 1)
            {
                yield return new ValidationResult("Items tidak boleh kosong", new List<string> { "Items" });
            } else
            {
                int errorCount = 0;
                List<Dictionary<string, string>> errorItems = new List<Dictionary<string, string>>();

                foreach (var item in Items)
                {
                    Dictionary<string, string> errorItem = new Dictionary<string, string>();

                    if (TypeOfGoods == "FABRIC")
                    {
                        if (item.PONo == null)
                        {
                            errorItem["PONo"] = "PONo tidak boleh kosong";
                            errorCount++;
                        }

                        if (item.Product == null)
                        {
                            errorItem["Product"] = "Kode Barang tidak boleh kosong";
                            errorCount++;
                        }

                        //if (item.Composition == null)
                        //{
                        //    errorItem["Composition"] = "Komposisi tidak boleh kosong";
                        //    errorCount++;
                        //}

                        //if (item.Construction == null)
                        //{
                        //    errorItem["Construction"] = "Konstruksi tidak boleh kosong";
                        //    errorCount++;
                        //}
                    } else if (TypeOfGoods == "ACCESSORIES")
                    {
                        if (item.PONo == null)
                        {
                            errorItem["PONo"] = "PONo tidak boleh kosong";
                            errorCount++;
                        }

                        if (item.Product == null)
                        {
                            errorItem["Product"] = "Kode Barang tidak boleh kosong";
                            errorCount++;
                        }

                        if (item.ProductRemark == null)
                        {
                            errorItem["ProductRemark"] = "Keterangan tidak boleh kosong";
                            errorCount++;
                        }
                    } else if (TypeOfGoods == "BARANG JADI")
                    {
                        if (item.LeftoverComodity == null)
                        {
                            errorItem["LeftoverComodity"] = "Komoditi tidak boleh kosong";
                            errorCount++;
                        }

                        if (item.RONo == null)
                        {
                            errorItem["RONo"] = "Nomor RO tidak boleh kosong";
                            errorCount++;
                        }
                    }

                    if (item.Unit == null)
                    {
                        errorItem["Unit"] = "Unit Asal tidak boleh kosong";
                        errorCount++;
                    }

                    if (item.Quantity <= 0)
                    {
                        errorItem["Quantity"] = "Jumlah tidak boleh 0";
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
