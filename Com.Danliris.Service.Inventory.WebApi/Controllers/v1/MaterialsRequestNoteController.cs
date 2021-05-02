using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
//using System.Reflection.Metadata;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/materials-request-notes")]
    [Authorize]
    public class MaterialsRequestNoteController : BaseController<MaterialsRequestNote, MaterialsRequestNoteViewModel, IMaterialRequestNoteService>
    {
        public MaterialsRequestNoteController(IIdentityService identityService, IValidateService validateService, IMaterialRequestNoteService service) : base(identityService, validateService, service, "1.0.0")
        {
        }

        [HttpGet("pdf/{Id}")]
        public async Task<IActionResult> GetPdfById([FromRoute] int Id)
        {
            try
            {
                var model = await Service.ReadByIdAsync(Id);
                var viewModel = Service.MapToViewModel(model);

                MaterialsRequestNotePdfTemplate PdfTemplate = new MaterialsRequestNotePdfTemplate();
                MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel);

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = $"Bon Surat Permintaan Barang {viewModel.Code}.pdf"
                };
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("update/is-complete/{Id}")]
        public async Task<IActionResult> PutIsCompleted([FromRoute] int Id, [FromBody] MaterialsRequestNoteViewModel ViewModel)
        {
            try
            {
                MaterialsRequestNote Model = Service.MapToModel(ViewModel);


                await Service.UpdateIsCompleted(Id, Model);
                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                       new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                       .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        //[HttpGet("reports")]
        //public IActionResult Get(string materialsRequestNoteCode, string productionOrderId, string unitId, string productId, string status, DateTime dateFrom, DateTime dateTo, int page, int size, string Order = "{}")
        //{
        //    int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        //    string accept = Request.Headers["Accept"];

        //    try
        //    {
        //        var data = Service.GetReport(materialsRequestNoteCode, productionOrderId, unitId, productId, status, dateFrom, dateTo, page, size, Order, offset);

        //        return Ok(new
        //        {
        //            apiVersion = ApiVersion,
        //            data = data.Item1,
        //            info = new { total = data.Item2 }
        //        });

        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}
    }
}
