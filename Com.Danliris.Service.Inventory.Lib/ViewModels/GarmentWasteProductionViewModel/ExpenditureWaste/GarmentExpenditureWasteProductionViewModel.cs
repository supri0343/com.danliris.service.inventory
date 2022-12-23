using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ExpenditureWaste
{
    public class GarmentExpenditureWasteProductionViewModel : BasicViewModel, IValidatableObject
    {
        public string GarmentExpenditureWasteNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string ExpenditureTo { get; set; }
        public string WasteType { get; set; }
        public string Description { get; set; }
        public string BCOutNo { get; set; }
        public string BCOutType { get; set; }
        public DateTimeOffset? BCOutDate { get; set; }
        public double ActualQuantity { get; set; }
        public List<GarmentExpenditureWasteProductionItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExpenditureDate == null || ExpenditureDate <= DateTimeOffset.MinValue)
            {
                yield return new ValidationResult("Tanggal Pengeluaran tidak boleh kosong", new List<string> { "ExpenditureDate" });
            }
            if (string.IsNullOrWhiteSpace(WasteType))
            {
                yield return new ValidationResult("Tipe Tidak Boleh Kosong", new List<string> { "WasteType" });
            }
            if (string.IsNullOrWhiteSpace(ExpenditureTo))
            {
                yield return new ValidationResult("Tujuan Tidak Boleh Kosong", new List<string> { "ExpenditureTo" });
            }
            if (string.IsNullOrWhiteSpace(Description))
            {
                yield return new ValidationResult("Keterangan Tidak Boleh Kosong", new List<string> { "Description" });
            }
            if (ActualQuantity == 0)
            {
                yield return new ValidationResult("Aktual Quantity Tidak Boleh 0", new List<string> { "ActualQuantity" });
            }
            if (Items == null || Items.Count < 1)
            {
                yield return new ValidationResult("Items tidak boleh kosong", new List<string> { "ItemsCount" });
            }
        }
    }
}
