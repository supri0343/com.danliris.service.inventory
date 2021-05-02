using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.FpReturnFromBuyers
{
    public class FpReturnFromBuyerViewModel : BasicViewModel, IValidatableObject
    {
        public BuyerViewModel Buyer { get; set; }
        public string Code { get; set; }
        public string CodeProduct { get; set; }
        public string CoverLetter { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Destination { get; set; }
        public IList<FpReturnFromBuyerDetailViewModel> Details { get; set; }
        public string SpkNo { get; set; }
        public StorageIntegrationViewModel Storage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date == null || Date.Date > DateTime.Now.Date)
                yield return new ValidationResult("Tanggal harus diisi atau tidak valid", new List<string> { "Date" });

            if (string.IsNullOrWhiteSpace(Destination))
                yield return new ValidationResult("Yang menerima harus diisi", new List<string> { "Destination" });

            if (Buyer == null || Buyer.Id < 1)
                yield return new ValidationResult("Buyer tidak valid atau harus diisi", new List<string> { "Buyer" });

            if (Storage == null || Storage._id < 1)
                yield return new ValidationResult("Gudang tidak valid atau harus diisi", new List<string> { "Storage" });

            int detailErrorCount = 0;
            string detailsError = "[";
            if (Details == null || Details.Count == 0)
                yield return new ValidationResult("Detail Surat Perintah Produksi harus diisi minimal 1", new List<string> { "Detail" });
            else
            {
                foreach (var detail in Details)
                {
                    detailsError += "{";

                    if (detail.ProductionOrder == null || detail.ProductionOrder.Id < 1)
                    {
                        detailErrorCount++;
                        detailsError += "ProductionOrder: 'Surat Perintah Produksi harus diisi', ";
                    }

                    string itemsError = "[";
                    if (detail.Items == null || detail.Items.Count < 1)
                    {
                        detailErrorCount++;
                        detailsError += "Item: 'Item barang harus diisi minimal 1', ";
                    }
                    else
                    {
                        detailErrorCount++;
                        detailsError += "Items: [";

                        foreach (var item in detail.Items)
                        {
                            itemsError += "{";

                            if (item.ProductId < 1)
                                itemsError += "ProductId: 'Barang harus diisi'";

                            if (item.ReturnQuantity <= 0)
                                itemsError += "ReturnQuantity: 'Jumlah retur harus lebih dari 0'";

                            if (item.Length <= 0)
                                itemsError += "Length: 'Panjang harus lebih dari 0'";

                            if (item.Weight <= 0)
                                itemsError += "Weight: 'Berat harus lebih dari 0'";

                            itemsError += "}, ";
                        }

                        detailsError += itemsError;
                        detailsError += "], ";
                    }

                    detailsError += "},";
                }
                detailsError += "]";
            }

            if (detailErrorCount > 0)
            {
                yield return new ValidationResult(detailsError, new List<string> { "Details" });
            }
        }
    }

    public class BuyerViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class StorageIntegrationViewModel
    {
        public int _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
}
