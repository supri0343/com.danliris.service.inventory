
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseExpenditureFinishedGoodControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-expenditures/finished-goods")]
    [Authorize]
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodController : BaseController<GarmentLeftoverWarehouseExpenditureFinishedGood, GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel, IGarmentLeftoverWarehouseExpenditureFinishedGoodService>
    {
        public GarmentLeftoverWarehouseExpenditureFinishedGoodController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseExpenditureFinishedGoodService service) : base(identityService, validateService, service, "1.0.0")
        {
        }
    }
}
