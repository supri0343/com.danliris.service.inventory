using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentLeftoverWarehouse;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ExpenditureFabric
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-expenditures/fabric")]
    [Authorize]
    public class GarmentLeftoverWarehouseExpenditureFabricController : BaseController<GarmentLeftoverWarehouseExpenditureFabric, GarmentLeftoverWarehouseExpenditureFabricViewModel, IGarmentLeftoverWarehouseExpenditureFabricService>
    {
        public GarmentLeftoverWarehouseExpenditureFabricController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseExpenditureFabricService service) : base(identityService, validateService, service, "1.0.0")
        {
        }

        [HttpGet("pdf/{Id}")]
        public async Task<IActionResult> GetPdfById([FromRoute] int Id)
        {
            try
            {
                var model = await Service.ReadByIdAsync(Id);
                var viewModel = Service.MapToViewModel(model);
                var products = Service.getProductForPDF(model);
                GarmentLeftoverWarehouseExpenditureFabricPDFTemplate PdfTemplate = new GarmentLeftoverWarehouseExpenditureFabricPDFTemplate();
                MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, products);

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = $"Bon Keluar Fabric Gudang Sisa {viewModel.ExpenditureNo}.pdf"
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
