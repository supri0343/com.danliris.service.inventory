using Com.Danliris.Service.Inventory.Lib.Models.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Lib.Services.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FpReturnFromBuyers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.FpReturnFromBuyerDataUtil
{
    public class FpReturnFromBuyerDataUtil
    {
        private readonly FpReturnFromBuyerService Service;
        public FpReturnFromBuyerDataUtil(FpReturnFromBuyerService service)
        {
            Service = service;
        }
        public FpReturnFromBuyerViewModel GetEmptyData()
        {
            return new FpReturnFromBuyerViewModel()
            {
                Buyer = new BuyerViewModel(),
                Storage = new StorageIntegrationViewModel()
            };
        }

        public FpReturnFromBuyerModel GetNewData()
        {
            //UnitReceiptNoteViewModel unit = UnitReceiptNoteDataUtil.GetUnitReceiptNote(client);
            ////ProductViewModel product = ProductDataUtil.GetProduct(client);
            //MachineViewModel machine = MachineDataUtil.GetMachine(client);
            //SupplierViewModel supplier = SupplierDataUtil.GetSupplier(client);

            return new FpReturnFromBuyerModel
            {
                BuyerCode = "BuyerCode",
                BuyerId = 1,
                BuyerName = "BuyerName",
                CodeProduct = "CodeProduct",
                CoverLetter = "CoverLetter",
                Date = DateTimeOffset.Now,
                Destination = "Destination",
                Details = new List<FpReturnFromBuyerDetailModel>()
                {
                    new FpReturnFromBuyerDetailModel()
                    {
                        Items = new List<FpReturnFromBuyerItemModel>()
                        {
                            new FpReturnFromBuyerItemModel()
                            {
                                ColorWay = "ColorWay",
                                DesignCode = "Designcode",
                                DesignNumber = "DesignNumber",
                                Length = 1,
                                ProductCode = "ProductCode",
                                ProductId = 1,
                                ProductName = "ProductName",
                                Remark = "Remark",
                                ReturnQuantity = 1,
                                UOM = "UOM",
                                UOMId = 1,
                                Weight = 1
                            }
                        },
                        ProductionOrderId = 1,
                        ProductionOrderNo = "ProductionOrderNo"
                    }
                },
                StorageCode = "StorageCode",
                StorageId = 1,
                StorageName = "StorageName",
                SpkNo = "SpkNo"
            };
        }

        public async Task<FpReturnFromBuyerModel> GetTestData()
        {
            var data = GetNewData();
            await this.Service.CreateAsync(data);
            return data;
        }
    }
}
