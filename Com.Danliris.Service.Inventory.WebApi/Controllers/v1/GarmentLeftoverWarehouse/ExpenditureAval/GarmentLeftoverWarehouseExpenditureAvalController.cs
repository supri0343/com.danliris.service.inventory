using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ExpenditureAval
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-expenditures/avals")]
    [Authorize]
    public class GarmentLeftoverWarehouseExpenditureAvalController : BaseController<GarmentLeftoverWarehouseExpenditureAval, GarmentLeftoverWarehouseExpenditureAvalViewModel, IGarmentLeftoverWarehouseExpenditureAvalService>
    {
        public GarmentLeftoverWarehouseExpenditureAvalController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseExpenditureAvalService service) : base(identityService, validateService, service, "1.0.0")
        {
        }
    }
}
