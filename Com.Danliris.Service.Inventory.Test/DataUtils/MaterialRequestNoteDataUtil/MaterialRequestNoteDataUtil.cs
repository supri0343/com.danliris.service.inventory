using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil
{
    public class MaterialRequestNoteDataUtil
    {
        private readonly NewMaterialRequestNoteService Service;

        public MaterialRequestNoteDataUtil(NewMaterialRequestNoteService service)
        {
            Service = service;
        }

        public MaterialsRequestNoteViewModel GetEmptyData()
        {
            MaterialsRequestNoteViewModel Data = new MaterialsRequestNoteViewModel();

            Data.RequestType = string.Empty;
            Data.Unit = new UnitViewModel();
            Data.Remark = string.Empty;
            Data.MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel> { new MaterialsRequestNote_ItemViewModel() {
                Product = new ProductViewModel()
                {

                },
                ProductionOrder = new ProductionOrderViewModel()
                {
                    OrderQuantity = 0,
                    OrderType = new OrderTypeViewModel()
                    {

                    }
                }
                
            } };

            return Data;
        }

        public MaterialsRequestNote GetNewData()
        {

            MaterialsRequestNote TestData = new MaterialsRequestNote
            {
                UnitId = "1",
                UnitCode = "a",
                UnitName = "name",
                Remark = "",
                RequestType = "AWAL",
                IsDistributed = false,
                IsCompleted = false,
                MaterialsRequestNote_Items = new List<MaterialsRequestNote_Item> { new MaterialsRequestNote_Item()
                {
                    Grade="a",
                    Length = 1,
                    OrderQuantity = 1,
                    OrderTypeCode = "code",
                    OrderTypeId = "1",
                    OrderTypeName = "name",
                    ProductCode = "code",
                    ProductionOrderNo = "c",
                    Remark = "a",
                    ProductId = "1",
                    ProductionOrderId = "1",
                    ProductionOrderIsCompleted = true,
                    ProductName = "name"
                }}
            };

            return TestData;
        }


        public async Task<MaterialsRequestNote> GetTestData()
        {
            MaterialsRequestNote Data = GetNewData();
            await this.Service.CreateAsync(Data);
            return Data;
        }

    }
}
