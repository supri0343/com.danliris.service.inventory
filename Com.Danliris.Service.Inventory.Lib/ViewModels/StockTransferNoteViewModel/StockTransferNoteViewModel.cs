using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel
{
    public class StockTransferNoteViewModel : BasicViewModel, IValidatableObject
    {
        public string Code { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceType { get; set; }
        public StorageViewModel SourceStorage { get; set; }
        public StorageViewModel TargetStorage { get; set; }
        public bool IsApproved { get; set; }
        public List<StockTransferNote_ItemViewModel> StockTransferNoteItems { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int Count = 0;

            if (string.IsNullOrWhiteSpace(this.ReferenceNo))
                yield return new ValidationResult("No. Referensi Harus Diisi", new List<string> { "ReferenceNo" });

            if (string.IsNullOrWhiteSpace(this.ReferenceType))
                yield return new ValidationResult("Jenis Referensi Harus Diisi", new List<string> { "ReferenceType" });

            if (this.SourceStorage == null || string.IsNullOrWhiteSpace(this.SourceStorage._id))
                yield return new ValidationResult("Gudang asal harus diisi", new List<string> { "SourceStorageId" });

            if (this.TargetStorage == null || string.IsNullOrWhiteSpace(this.TargetStorage._id))
                yield return new ValidationResult("Gudang tujuan harus diisi", new List<string> { "TargetStorageId" });

            if (this.StockTransferNoteItems == null || this.StockTransferNoteItems.Count.Equals(0))
            {
                yield return new ValidationResult("Item harus diisi", new List<string> { "StockTransferNoteItems" });
            }
            else
            {
                string stockTransferNoteItemError = "[";

                foreach (StockTransferNote_ItemViewModel stockTransferNoteItem in this.StockTransferNoteItems)
                {
                    stockTransferNoteItemError += "{ ";
                    if (stockTransferNoteItem.Summary == null || stockTransferNoteItem.Summary.productId == 0)
                    {
                        Count++;
                        stockTransferNoteItemError += "SummaryId: 'Barang harus diisi', ";
                    }
                    else
                    {
                        int count = StockTransferNoteItems.Count(c => string.Equals(c.Summary.productName, stockTransferNoteItem.Summary.productName));

                        if (count > 1)
                        {
                            Count++;
                            stockTransferNoteItemError += "SummaryId: 'Barang tidak boleh duplikat', ";
                        }
                    }

                    if (stockTransferNoteItem.TransferedQuantity == null || stockTransferNoteItem.TransferedQuantity <= 0)
                    {
                        Count++;
                        stockTransferNoteItemError += "TransferedQuantity: 'Quantity Transfer harus lebih besar dari 0', ";
                    }
                    else if (stockTransferNoteItem.TransferedQuantity > stockTransferNoteItem.Summary.quantity)
                    {
                        Count++;
                        stockTransferNoteItemError += "TransferedQuantity : 'Quantity Transfer harus lebih kecil dari atau sama dengan Quantity Stok', ";
                    }
                    stockTransferNoteItemError += "}, ";
                }

                stockTransferNoteItemError += "]";

                if (Count > 0)
                {
                    yield return new ValidationResult(stockTransferNoteItemError, new List<string> { "StockTransferNoteItems" });
                }
            }
        }
    }
}
