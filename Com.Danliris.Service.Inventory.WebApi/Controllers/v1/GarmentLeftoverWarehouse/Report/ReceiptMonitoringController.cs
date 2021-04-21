using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.Report
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-receipts/monitoring")]
    [Authorize]
    public class ReceiptMonitoringController : Controller
    {
        private IIdentityService IdentityService;
        private readonly IValidateService ValidateService;
        private readonly IReceiptMonitoringService Service;
        private readonly string ApiVersion;
        //private static readonly string ApiVersion = "1.0";
        //private MaterialsRequestNoteService materialsRequestNoteService { get; }

        public ReceiptMonitoringController(IIdentityService identityService, IValidateService validateService, IReceiptMonitoringService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            ApiVersion = "1.0.0";
        }

        [HttpGet]
        public IActionResult Get(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {
                if(type == "FABRIC")
                {
                    var data = Service.GetFabricReceiptMonitoring(dateFrom, dateTo, page, size, Order, offset);

                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        data = data.Item1,
                        info = new { total = data.Item2 }
                    });
                }
                else
                {
                    var data = Service.GetFabricReceiptMonitoring(dateFrom, dateTo, page, size, Order, offset);

                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        data = data.Item1,
                        info = new { total = data.Item2 }
                    });
                }
                
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
