using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentReceiptSubconWasteProduction.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentReceiptSubconWasteProductionViewModel.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentReceiptSubconWasteProduction;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentReceiptSubconWaste;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentReceiptSubconWasteProduction
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-receipt-subcon/waste/receipt")]
    [Authorize]
    public class GarmentSubconReceiptWasteProductionController : BaseController<GarmentSubconReceiptWasteProductions, GarmentSubconReceiptWasteProductionViewModel, IGarmentSubconReceiptWasteProductionService>
    {
        public GarmentSubconReceiptWasteProductionController(IIdentityService identityService, IValidateService validateService, IGarmentSubconReceiptWasteProductionService service) : base(identityService, validateService, service, "1.0.0")
        {
        }

        [HttpGet("pdf/{Id}")]
        public async Task<IActionResult> GetPdfById([FromRoute] int Id)
        {
            try
            {
                var model = await Service.ReadByIdAsync(Id);
                var viewModel = Service.MapToViewModel(model);

                GarmentSubconReceiptWastePDFTemplate PdfTemplate = new GarmentSubconReceiptWastePDFTemplate();
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
