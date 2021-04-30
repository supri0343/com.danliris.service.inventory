using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.BalanceStock
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse/balance-stocks")]
    [Authorize]
    public class GarmentLeftoverWarehouseBalanceStockController : BaseController<GarmentLeftoverWarehouseBalanceStock, GarmentLeftoverWarehouseBalanceStockViewModel, IGarmentLeftoverWarehouseBalanceStockService>
    {
        public GarmentLeftoverWarehouseBalanceStockController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseBalanceStockService service) : base(identityService, validateService, service, "1.0.0")
        {
        }
    }
}
