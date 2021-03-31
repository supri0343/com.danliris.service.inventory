using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ReceiptAccessories
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-receipts/accessories")]
    [Authorize]
    public class GarmentLeftoverWarehouseReceiptAccessoriesController : BaseController<GarmentLeftoverWarehouseExpenditureAccessory, GarmentLeftoverWarehouseReceiptAccessoriesViewModel, IGarmentLeftoverWarehouseReceiptAccessoriesService>
    {

        public GarmentLeftoverWarehouseReceiptAccessoriesController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseReceiptAccessoriesService service, string apiVersion) : base(identityService, validateService, service, apiVersion)
        {
        }
    }
}
