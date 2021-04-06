using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using Com.Danliris.Service.Inventory.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.MaterialDistributionNoteDataUtil
{
    public class MaterialDistributionNoteDataUtil
    {
        private readonly NewMaterialDistributionNoteService Service;
        private readonly NewMaterialRequestNoteService mrnService;
        public MaterialDistributionNoteDataUtil(NewMaterialDistributionNoteService service, NewMaterialRequestNoteService newMaterialRequestNoteService)
        {
            Service = service;
            mrnService = newMaterialRequestNoteService;
        }

        public MaterialDistributionNoteViewModel GetEmptyData()
        {
            MaterialDistributionNoteViewModel Data = new MaterialDistributionNoteViewModel();

            Data.Type = string.Empty;
            Data.Unit = new UnitViewModel()
            {
                Id = "1"
            };
            Data.MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel> {
                    new MaterialDistributionNoteItemViewModel {
                        MaterialDistributionNoteDetails = new List<MaterialDistributionNoteDetailViewModel>() { new MaterialDistributionNoteDetailViewModel()
                        {
                            Product = new ProductViewModel(),
                            ProductionOrder = new ProductionOrderViewModel(),
                            Supplier = new SupplierViewModel()
                        }
                    }
                }
            };

            return Data;
        }

        public MaterialDistributionNote GetNewData()
        {

            MaterialDistributionNote TestData = new MaterialDistributionNote
            {
                UnitId = "1",
                UnitCode = "code",
                UnitName = "name",
                Type = "PRODUKSI",
                IsApproved = false,
                IsDisposition = false,
                MaterialDistributionNoteItems = new List<MaterialDistributionNoteItem> {
                    new MaterialDistributionNoteItem()
                    {
                        MaterialRequestNoteCode = "code",
                        MaterialRequestNoteCreatedDateUtc = DateTime.UtcNow,
                        MaterialDistributionNoteDetails = new List<MaterialDistributionNoteDetail>()
                        {
                            new MaterialDistributionNoteDetail()
                            {
                                DistributedLength = 1,
                                Grade = "a",
                                ProductCode = "code",
                                ProductId = "1",
                                MaterialsRequestNoteItemId = 1,
                                MaterialRequestNoteItemLength = 1,
                                ProductionOrderId = "1",
                                ProductionOrderNo = "no",
                                ProductName = "name",
                                SupplierCode = "code",
                                SupplierId = "1",
                                SupplierName = "name",
                                ReceivedLength = 1,
                                Quantity = 1
                            }
                        }
                    }
                }
            };

            return TestData;
        }


        public async Task<MaterialDistributionNote> GetTestData()
        {
            MaterialRequestNoteDataUtil.MaterialRequestNoteDataUtil materialRequestNoteDataUtil = new MaterialRequestNoteDataUtil.MaterialRequestNoteDataUtil(mrnService);
            var mrn = await materialRequestNoteDataUtil.GetTestData();

            MaterialDistributionNote Data = GetNewData();
            foreach(var item in Data.MaterialDistributionNoteItems)
            {
                item.MaterialRequestNoteId = mrn.Id;
                item.MaterialRequestNoteCreatedDateUtc = mrn._CreatedUtc;
                item.MaterialRequestNoteCode = mrn.Code;
                foreach(var detail in item.MaterialDistributionNoteDetails)
                {
                    detail.MaterialsRequestNoteItemId = mrn.Id;
                }
            }
            await this.Service.CreateAsync(Data);
            return Data;
        }


    }
}
