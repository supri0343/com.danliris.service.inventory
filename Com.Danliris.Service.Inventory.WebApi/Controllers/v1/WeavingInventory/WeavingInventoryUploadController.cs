using AutoMapper;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.InventoryWeavingDocumentUploadService;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/inventory-weaving")]
    [Authorize]
    public class WeavingInventoryUploadController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IInventoryWeavingDocumentUploadService service;
        private readonly IIdentityService identityService;
        protected readonly IValidateService ValidateService;
        private readonly string ContentType = "application/vnd.openxmlformats";
        private readonly string FileName = string.Concat("Error Log - ", typeof(InventoryWeavingDocument).Name, " ", DateTime.Now.ToString("dd MMM yyyy"), ".csv");

       // IIdentityService identityService, IValidateService validateService, ICOAService service, IMapper mapper
        public WeavingInventoryUploadController(IIdentityService identityService, IValidateService validateService, IInventoryWeavingDocumentUploadService service, IMapper mapper  ) //: base(facade, ApiVersion)
        {
            this.mapper = mapper;
            this.service = service;
            this.identityService = identityService;
            this.ValidateService = validateService;
        }
        protected void VerifyUser()
        {
            identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }
        [HttpGet]
        public IActionResult Get([FromQuery] string keyword = null, [FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string order = "{}",
            [FromQuery] string filter = "{}")
        {
            try
            {

                var data = service.Read(page, size, order, keyword ,filter);
                
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);

            }



        }
        [HttpPost("upload")]
        public async Task<IActionResult> PostCSVFileAsync(string source, DateTime date)
        // public async Task<IActionResult> PostCSVFileAsync(double source, double destination,  DateTime date)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
                identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                if (Request.Form.Files.Count > 0)
                {
                    //VerifyUser();
                    var UploadedFile = Request.Form.Files[0];
                    StreamReader Reader = new StreamReader(UploadedFile.OpenReadStream());
                    List<string> FileHeader = new List<string>(Reader.ReadLine().Split(","));
                    var ValidHeader = service.CsvHeader.SequenceEqual(FileHeader, StringComparer.OrdinalIgnoreCase);

                    if (ValidHeader)
                    {
                        Reader.DiscardBufferedData();
                        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        Reader.BaseStream.Position = 0;
                        CsvReader Csv = new CsvReader(Reader);
                        Csv.Configuration.IgnoreQuotes = false;
                        Csv.Configuration.Delimiter = ",";
                        Csv.Configuration.RegisterClassMap<InventoryWeavingDocumentMap>();
                        Csv.Configuration.HeaderValidated = null;

                        List<InventoryWeavingDocumentCsvViewModel> Data = Csv.GetRecords<InventoryWeavingDocumentCsvViewModel>().ToList();

                        InventoryWeavingDocumentViewModel Data1 = await service.MapToViewModel(Data, date, source);
                        //InventoryWeavingDocumentViewModel Data1 = await service.MapToViewModel(Data, source);

                        ValidateService.Validate(Data1);

                       

                        Tuple<bool, List<object>> Validated = service.UploadValidate(ref Data, Request.Form.ToList());

                        Reader.Close();

                        if (Validated.Item1)
                        {
                            var CheckNota = service.checkNota(Data);

                            if (CheckNota == 0)
                            {

                                InventoryWeavingDocument data = await service.MapToModel(Data1);

                                await service.UploadData(data, identityService.Username);


                                Dictionary<string, object> Result =
                                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                                    .Ok();
                                return Created(HttpContext.Request.Path, Result);


                            }

                            else
                            {
                                Dictionary<string, object> Result = new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, "Nota Sudah Pernah di Input").Fail();

                                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
                            }

                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                                using (CsvWriter csvWriter = new CsvWriter(streamWriter))
                                {
                                    csvWriter.WriteRecords(Validated.Item2);
                                }

                                return File(memoryStream.ToArray(), ContentType, FileName);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, object> Result =
                           new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, General.CSV_ERROR_MESSAGE)
                           .Fail();

                        return NotFound(Result);
                    }
                }
                else
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.NO_FILE_ERROR_MESSAGE)
                            .Fail();
                    return BadRequest(Result);
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

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {

                var data = service.ReadById(id);
                return Ok(new
                {
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("monitoring")]
        public IActionResult GetInputWeaving(string bonType, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page = 1, int size = 25, string order = "{}")
        {
            try
            {
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = service.ReadInputWeaving(bonType, dateFrom, dateTo, page, size, order, offset);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("xls")]
        public IActionResult GetExcelAll([FromHeader(Name = "x-timezone-offset")] string timezone, string from, DateTimeOffset? dateFrom,  DateTimeOffset? dateTo)
        {
            try
            {
                //VerifyUser();
                byte[] xlsInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(timezone);
                var Result = service.GenerateExcel(from, dateFrom, dateTo, clientTimeZoneOffset);
                string filename = "Monitoring Penerimaan Gudang Weaving.xlsx";

                xlsInBytes = Result.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut("{Id}")]
        public virtual async Task<IActionResult> Put([FromRoute] int id, [FromBody] InventoryWeavingDocumentDetailViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);

                if (id != viewModel.Id)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                        .Fail();
                    return BadRequest(Result);
                }

                InventoryWeavingDocument model = await service.MapToModelUpdate(viewModel);

                await service.UpdateAsync(id, model);

                return NoContent();
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
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
