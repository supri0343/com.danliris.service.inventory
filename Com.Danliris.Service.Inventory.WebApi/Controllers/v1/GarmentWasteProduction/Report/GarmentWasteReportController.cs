using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping;
using Com.Danliris.Service.Inventory.Lib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentWasteProduction.Report;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentWasteProduction.Report
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/waste/report")]
    [Authorize]
    public class GarmentWasteReportController  : Controller
    {
        protected IIdentityService IdentityService;
        protected readonly IValidateService ValidateService;
        protected readonly IGarmentWasteReportService Service;
        protected const string ApiVersion = "1.0.0";

        public GarmentWasteReportController(IIdentityService identityService, IValidateService validateService, IGarmentWasteReportService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetWasteReport(DateTime dateFrom, DateTime dateTo, string type)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = await Service.GetReport(dateFrom, dateTo,type);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("recap")]
        public async Task<IActionResult> GetWasteReportRecap(DateTime dateFrom, DateTime dateTo, string type,string transactionType)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = await Service.GetRecap(dateFrom, dateTo, type, transactionType);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
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
