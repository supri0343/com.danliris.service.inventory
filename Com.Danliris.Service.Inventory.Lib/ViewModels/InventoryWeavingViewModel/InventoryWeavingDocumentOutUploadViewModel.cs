    using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel
{
    public class InventoryWeavingDocumentOutUploadViewModel : BasicViewModel, IValidatableObject
    {
        public DateTimeOffset date { get; set; }
        public string bonNo { get; set; }
        public string bonType { get; set; }


        public int storageId { get; set; }
        public string storageCode { get; set; }
        public string storageName { get; set; }
        public string remark { get; set; }
        public string type { get; set; }
        public ICollection<InventoryWeavingDocumentOutItemViewModel> itemsOut { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.date.Equals(DateTimeOffset.MinValue) || this.date == null)
            {
                yield return new ValidationResult("Date is required", new List<string> { "date" });
            }
            if (this.bonType == null)
            {
                yield return new ValidationResult("Destination is required", new List<string> { "destination" });
            }

            //if ( !(date >= DateTimeOffset.UtcNow || ((DateTimeOffset.UtcNow - date).TotalDays <= 1 && (DateTimeOffset.UtcNow - date).TotalDays >= 0)))
            //{
            //    yield return new ValidationResult("Tanggal Harus Lebih Besar atau Sama Dengan Hari Ini", new List<string> { "Date" });
            //}
            /*
            int Count = 0;
            string DetailErrors = "[";
            var cek = itemsOut.Where(x => x.IsSave.Equals(x.Construction) != false).Count();

            if (cek == 0)
            {
                yield return new ValidationResult("Isi Konstruksi");
            }
            else
            {

                var query = itemsOut.Where(s => s.IsSave != false);

                //if (query != null)
                //{
                    //foreach (var item in query)
                    //{

                        //var data = item.ListItems.GroupBy(x => new { x.Grade, x.Piece });
                        //foreach (var i in data)
                        //{

                            DetailErrors += "{";
                            DetailErrors += "WarehouseList : [ ";

                            foreach (var detail in query)
                            {
                                DetailErrors += "{";

                                if (detail.IsSave == true)
                                {
                                    if (detail.Quantity <= 0)
                                    {
                                        Count++;
                                        DetailErrors += "Qty: 'Qty  Harus Lebih dari 0!',";
                                    }
                                    else
                                    {
                                        if (detail.Quantity > detail.Quantity)
                                        {
                                            Count++;
                                            DetailErrors += string.Format("Qty: 'Qty Keluar Tidak boleh Lebih dari sisa saldo {0}!',", detail.Quantity);
                                        }
                                    }

                                    if (detail.QuantityPiece <= 0)
                                    {
                                        Count++;
                                        DetailErrors += "QtyPiece: 'Qty Piece Harus Lebih dari 0!',";
                                    }
                                    else
                                    {
                                        if (detail.QuantityPiece > detail.QuantityPiece)
                                        {
                                            Count++;
                                            DetailErrors += string.Format("QtyPiece: 'Qty Piece Keluar Tidak boleh Lebih dari sisa saldo {0}!',", detail.QuantityPiece);
                                        }

                                    }

                                }


                                DetailErrors += "}, ";
                            }
                            DetailErrors += "], ";
                            DetailErrors += "}, ";

                        }

                    //}
                //}
                

            DetailErrors += "]";

            if (Count > 0)
                yield return new ValidationResult(DetailErrors);*/
            
        }
    }
}
