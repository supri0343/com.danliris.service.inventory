using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.FpRegradingResultDataUtil
{
    public class FpRegradingResultDataUtil
    {
        private readonly NewFpRegradingResultDocsService Service;
        public FpRegradingResultDataUtil(NewFpRegradingResultDocsService service)
        {
            Service = service;
        }
        public FpRegradingResultDocsViewModel GetEmptyData()
        {
            FpRegradingResultDocsViewModel Data = new FpRegradingResultDocsViewModel();

            Data.Bon = new FpRegradingResultDocsViewModel.noBon();
            Data.Machine = new FpRegradingResultDocsViewModel.machine();
            Data.Product = new FpRegradingResultDocsViewModel.product();
            Data.Supplier = new FpRegradingResultDocsViewModel.supplier();
            Data.Details = new List<FpRegradingResultDetailsDocsViewModel> { new FpRegradingResultDetailsDocsViewModel() };
            Data.Date = null;
            Data.Shift = string.Empty;
            Data.TotalLength = 0;
            Data.OriginalGrade = string.Empty;
            Data.Remark = string.Empty;

            return Data;
        }

        public FpRegradingResultDocs GetNewData()
        {
            //UnitReceiptNoteViewModel unit = UnitReceiptNoteDataUtil.GetUnitReceiptNote(client);
            ////ProductViewModel product = ProductDataUtil.GetProduct(client);
            //MachineViewModel machine = MachineDataUtil.GetMachine(client);
            //SupplierViewModel supplier = SupplierDataUtil.GetSupplier(client);

            FpRegradingResultDocs TestData = new FpRegradingResultDocs
            {
                Date = DateTimeOffset.UtcNow,
                NoBon = "no",
                NoBonId = "1",

                MachineId = "1",
                MachineCode = "code",
                MachineName = "name",

                ProductId = "1",
                ProductCode = "code",
                ProductName = "name",

                SupplierId = "1",
                SupplierCode = "code",
                SupplierName = "name",
                Operator = "operator test",
                Shift = "shift 1 test",
                TotalLength = 100,
                OriginalGrade = "test grade",
                Remark = "test remark",
                UnitName = "PRINTING",
                _CreatedUtc = new DateTime(2019,8,11,1,1,1),

                Details = new List<FpRegradingResultDocsDetails> { new FpRegradingResultDocsDetails(){
                    Grade = "a",
                    Length = 1,
                    ProductCode = "code",
                    ProductId = "1",
                    ProductName = "name",
                    Quantity = 1,
                    Remark = "remar",
                    Retur = "retur",
                      _CreatedUtc = new DateTime(2019,8,11,1,1,1),
                } }

            };

            return TestData;
        }

        public async Task<FpRegradingResultDocs> GetTestData()
        {
            FpRegradingResultDocs Data = GetNewData();
            await this.Service.CreateAsync(Data);
            return Data;
        }
    }
}
