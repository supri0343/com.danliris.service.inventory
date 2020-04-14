using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodViewModel;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-receipts/finished-goods")]
    [Authorize]
    public class GarmentLeftoverWarehouseReceiptFinishedGoodController : BaseController<GarmentLeftoverWarehouseReceiptFinishedGood, GarmentLeftoverWarehouseReceiptFinishedGoodViewModel, IGarmentLeftoverWarehouseReceiptFinishedGoodService>
    {
        public GarmentLeftoverWarehouseReceiptFinishedGoodController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseReceiptFinishedGoodService service) : base(identityService, validateService, service, "1.0.0")
        {
        }
    }
}
