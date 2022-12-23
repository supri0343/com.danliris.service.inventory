using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentWasteProduction;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentWaste;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentWasteProduction
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/waste/receipt")]
    [Authorize]
    public class GarmentReceiptWasteProductionController : BaseController<GarmentReceiptWasteProductions, GarmentReceiptWasteProductionViewModel, IGarmentReceiptWasteProductionService>
    {
        public GarmentReceiptWasteProductionController(IIdentityService identityService, IValidateService validateService, IGarmentReceiptWasteProductionService service) : base(identityService, validateService, service, "1.0.0")
        {
        }

        [HttpGet("pdf/{Id}")]
        public async Task<IActionResult> GetPdfById([FromRoute] int Id)
        {
            try
            {
                var model = await Service.ReadByIdAsync(Id);
                var viewModel = Service.MapToViewModel(model);

                GarmentReceiptWastePDFTemplate PdfTemplate = new GarmentReceiptWastePDFTemplate();
                MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel);

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = $"Bon Terima {viewModel.WasteType} - {viewModel.GarmentReceiptWasteNo}.pdf"
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
    }
}
