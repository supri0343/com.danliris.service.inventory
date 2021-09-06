using AutoMapper;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/inventory-weaving/stock-greige-recap-reports")]
    [Authorize]
    public class WeavingInventoryMovementController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IInventoryWeavingMovementService service;
        private readonly IIdentityService identityService;
        protected readonly IValidateService ValidateService;
        private readonly string ContentType = "application/vnd.openxmlformats";
        private readonly string FileName = string.Concat("Error Log - ", typeof(InventoryWeavingDocument).Name, " ", DateTime.Now.ToString("dd MMM yyyy"), ".csv");
        public WeavingInventoryMovementController(IIdentityService identityService, IValidateService validateService, IInventoryWeavingMovementService service) 
        {
            //this.mapper = mapper;
            this.service = service;
            this.identityService = identityService;
            this.ValidateService = validateService;
        }

        /*   [HttpGet]
           public IActionResult GetReport(string grade, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page = 1, int size = 25, string order = "{}")
           {
               try
               {
                   int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                   var data = service.ReadReport(grade, dateFrom, dateTo, page, size, order, offset);
                   return Ok(data);
               }
               catch (Exception ex)
               {
                   return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
               }
           } */

        [HttpGet]
        public IActionResult GetReportGrade(string grade, DateTime? dateTo, [FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string order = "{}"
          )
        {
            try
            {
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = service.ReadReportGrade(grade, dateTo, page, size, order, offset);
                return Ok(new {
                    Data = data.Item1,
                    TotalData = data.Item2 ,

                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);

            }
        }

     /*   [HttpGet("Grade")]
        public IActionResult GetGrade([FromQuery] string grade, DateTimeOffset? dateTo)
        {
            try
            {
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = service.GetGrade(grade, dateTo, offset);
                return Ok(new
                {
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);

            }
        } */


        [HttpGet("download")]
           public IActionResult GetXlsAll(string grade, DateTime? dateTo)
           {

               try
               {
                   byte[] xlsInBytes;
                   int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

                var xls = service.GenerateExcel(grade, dateTo, offset);

                   string filename = String.Format("Laporan Rekapitulas Stock Grey Per Grade - {0}.xlsx", DateTimeOffset.UtcNow.ToString("ddMMyyyy"));

                   xlsInBytes = xls.ToArray();
                   var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                   return file;

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