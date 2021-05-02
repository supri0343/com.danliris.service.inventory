using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.FPReturnInvToPurchasingService;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/fp-return-inv-to-purchasings")]
    [Authorize]
    public class FPReturnInvToPurchasingController : BaseController<FPReturnInvToPurchasing, FPReturnInvToPurchasingViewModel, IFPReturnInvToPurchasingService>
    {
        public FPReturnInvToPurchasingController(IIdentityService identityService, IValidateService validateService, IFPReturnInvToPurchasingService service) : base(identityService, validateService, service, "1.0.0")
        {

        }

        public override IActionResult Get(int Page = 1, int Size = 25, string Order = "{}", [Bind(Prefix = "Select[]")]List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            try
            {
                var read = Service.Read(Page, Size, Order, Keyword, Filter);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = read.Item1,
                    info = new Dictionary<string, object>
                    {
                        { "count", read.Item1.Count },
                        { "total", read.Item2 },
                        { "order", read.Item3 },
                        { "page", Page },
                        { "size", Size }
                    },
                    message = Helpers.General.OK_MESSAGE,
                    statusCode = Helpers.General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Helpers.General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Helpers.General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        public override async Task<IActionResult> GetById([FromRoute] int id)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            string pdf = "application/pdf";
            if (accept != null && accept.IndexOf(pdf) != -1) // PDF
            {
                var model = await Service.ReadByIdAsync(id);
                FPReturnInvToPurchasingPdfTemplate PdfTemplate = new FPReturnInvToPurchasingPdfTemplate();
                MemoryStream stream = PdfTemplate.GeneratePdfTemplate(Service.MapToViewModel(model), offset);

                return new FileStreamResult(stream, pdf)
                {
                    FileDownloadName = $"Bon Retur Barang {model.No}.pdf"
                };
            }
            else
            {

                return await base.GetById(id);
            }
        }

        public override async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                VerifyUser();
                int Result = await Service.DeleteAsync(id);

                if (Result.Equals(0))
                {
                    Dictionary<string, object> ResultNotFound =
                       new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                       .Fail();
                    return NotFound(ResultNotFound);
                }

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

    }
}