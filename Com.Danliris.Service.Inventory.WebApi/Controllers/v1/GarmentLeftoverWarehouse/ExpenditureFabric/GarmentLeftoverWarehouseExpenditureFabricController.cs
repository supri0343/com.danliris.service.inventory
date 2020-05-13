using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ExpenditureFabric
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-expenditures/fabric")]
    [Authorize]
    public class GarmentLeftoverWarehouseExpenditureFabricController : BaseController<GarmentLeftoverWarehouseExpenditureFabric, GarmentLeftoverWarehouseExpenditureFabricViewModel, IGarmentLeftoverWarehouseExpenditureFabricService>
    {
        public GarmentLeftoverWarehouseExpenditureFabricController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseExpenditureFabricService service) : base(identityService, validateService, service, "1.0.0")
        {
        }
    }
}
