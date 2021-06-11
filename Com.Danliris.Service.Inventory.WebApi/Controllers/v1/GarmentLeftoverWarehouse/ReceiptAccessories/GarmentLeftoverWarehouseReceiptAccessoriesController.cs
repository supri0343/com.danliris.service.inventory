using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates.GarmentLeftoverWarehouse;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.ReceiptAccessories
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-receipts/accessories")]
    [Authorize]
    public class GarmentLeftoverWarehouseReceiptAccessoriesController : BaseController<GarmentLeftoverWarehouseReceiptAccessory, GarmentLeftoverWarehouseReceiptAccessoriesViewModel, IGarmentLeftoverWarehouseReceiptAccessoriesService>
    {

        public GarmentLeftoverWarehouseReceiptAccessoriesController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseReceiptAccessoriesService service) : base(identityService, validateService, service, "1.0.0")
        {
        }

        [HttpGet("pdf/{Id}")]
        public async Task<IActionResult> GetPdfById([FromRoute] int Id)
        {
            try
            {
                var model = await Service.ReadByIdAsync(Id);
                var viewModel = Service.MapToViewModel(model);

                GarmentLeftoverWarehouseReceiptAccessoriesPDFTemplate PdfTemplate = new GarmentLeftoverWarehouseReceiptAccessoriesPDFTemplate();
                MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel);

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = $"Bon Terima Accessories Gudang Sisa {viewModel.InvoiceNoReceive}.pdf"
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
