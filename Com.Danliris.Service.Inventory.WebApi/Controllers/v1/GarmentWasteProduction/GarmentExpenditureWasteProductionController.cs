using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ExpenditureWaste;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentWasteProduction;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ExpenditureWaste;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Com.Danliris.Service.Inventory.Lib.Services;
using System.IO;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentWaste;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentWasteProduction
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/waste/expenditure")]
    [Authorize]
    public class GarmentExpenditureWasteProductionController : BaseController<GarmentExpenditureWasteProductions, GarmentExpenditureWasteProductionViewModel, IGarmentExpenditureWasteProductionService>
    {
        public GarmentExpenditureWasteProductionController(IIdentityService identityService, IValidateService validateService, IGarmentExpenditureWasteProductionService service) : base(identityService, validateService, service, "1.0.0")
        {
        }

        [HttpGet("pdf/{Id}")]
        public async Task<IActionResult> GetPdfById([FromRoute] int Id)
        {
            try
            {
                var model = await Service.ReadByIdAsync(Id);
                var viewModel = Service.MapToViewModel(model);

                GarmentExpenditureWastePDFTemplate PdfTemplate = new GarmentExpenditureWastePDFTemplate();
                MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel);

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = $"Bon Keluar {viewModel.WasteType} - {viewModel.GarmentExpenditureWasteNo}.pdf"
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
