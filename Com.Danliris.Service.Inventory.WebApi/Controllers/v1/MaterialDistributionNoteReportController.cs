using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using System.IO;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates;
using Com.Danliris.Service.Inventory.Lib.Services;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/material-distribution-note-report")]
    [Authorize]
    public class MaterialDistributionNoteReportController : Controller
    {
        private IIdentityService IdentityService;
        private readonly IValidateService ValidateService;
        private readonly IMaterialDistributionService Service;
        private readonly string ApiVersion;

        public MaterialDistributionNoteReportController(IIdentityService identityService, IValidateService validateService, IMaterialDistributionService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            ApiVersion = "1.0.0";
        }

        [HttpGet]
        public IActionResult Get(string unitId, string unitName, string type, DateTime date, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            string pdf = "application/pdf";

            try
            {
                if (accept.IndexOf(pdf) == -1) // Not PDF
                {
                    var data = Service.GetReport(unitId, type, date, page, size, Order, offset);

                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        data = data.Item1,
                        info = new { total = data.Item2, page = page, size = size }
                    });
                }
                else
                {
                    var Data = Service.GetPdfReport(unitId, unitName, type, date, offset);

                    if (Data.Count.Equals(0))
                    {
                        Dictionary<string, object> Result =
                            new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, "Error PDF")
                            .Fail("Tidak ada data");
                        return BadRequest(Result);
                    }
                    else if (Data.Any(p => p.IsDisposition == true && p.IsApproved == false))
                    {
                        Dictionary<string, object> Result =
                            new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, "Error PDF")
                            .Fail("Ada data disposisi yang belum diapprove");
                        return BadRequest(Result);
                    }
                    else
                    {
                        MaterialDistributionNoteReportPdfTemplate PdfTemplate = new MaterialDistributionNoteReportPdfTemplate();
                        MemoryStream stream = PdfTemplate.GeneratePdfTemplate(Data, date, unitName);

                        return new FileStreamResult(stream, pdf)
                        {
                            FileDownloadName = $"Bon Surat Permintaan Barang {date.ToString("dd-MM-yyyy")}.pdf"
                        };
                    }
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