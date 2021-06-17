using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentLeftoverWarehouse;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-expenditures/accessories")]
    [Authorize]
    public class GarmentLeftoverWarehouseExpenditureAccessoriesController : BaseController<GarmentLeftoverWarehouseExpenditureAccessories, GarmentLeftoverWarehouseExpenditureAccessoriesViewModel, IGarmentLeftoverWarehouseExpenditureAccessoriesService>
    {
        public GarmentLeftoverWarehouseExpenditureAccessoriesController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseExpenditureAccessoriesService service) : base(identityService, validateService, service, "1.0.0")
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
                GarmentLeftoverWarehouseExpenditureAccessoriesPDFTemplate PdfTemplate = new GarmentLeftoverWarehouseExpenditureAccessoriesPDFTemplate();
                MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, products);

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = $"Bon Keluar Accessories Gudang Sisa {viewModel.ExpenditureNo}.pdf"
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
