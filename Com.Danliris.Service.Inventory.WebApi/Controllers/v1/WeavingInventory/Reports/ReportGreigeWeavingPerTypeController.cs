using AutoMapper;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerType;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory.Reports
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/report-last-stock-greige-weaving-types")]
    [Authorize]
    public class ReportGreigeWeavingPerTypeController : Controller
    {
        protected IIdentityService IdentityService;
        protected readonly IValidateService ValidateService;
        protected readonly IReportGreigeWeavingPerTypeService Service;
        protected readonly string ApiVersion;
        public ReportGreigeWeavingPerTypeController(IIdentityService identityService, IValidateService validateService, IReportGreigeWeavingPerTypeService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            ApiVersion = "1.0.0";
        }

        [HttpGet]
        public IActionResult GetReport(DateTime? dateTo, int page = 1, int size = 25, string Order = "{}")
        {
            try
            {

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data =  Service.GetStockReport(dateTo, offset, page, size, Order);



                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
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

        [HttpGet("download")]
        public IActionResult GetExcelAll([FromHeader(Name = "x-timezone-offset")] string timezone, DateTime? dateTo)
        {
            try
            {
                //VerifyUser();
                byte[] xlsInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(timezone);
                var Result =  Service.GenerateExcel(dateTo, clientTimeZoneOffset);
                string filename = " Saldo Akhir Gudang Grey per Jenis.xlsx";

                xlsInBytes = Result.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
