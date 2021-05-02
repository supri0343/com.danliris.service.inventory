using Microsoft.AspNetCore.Mvc;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Com.Danliris.Service.Inventory.Lib;
using Microsoft.AspNetCore.Authorization;
using Com.Danliris.Service.Inventory.Lib.Services.StockTransferNoteService;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using System.Collections.Generic;
using System;
using System.Linq;
using Com.Danliris.Service.Inventory.Lib.PDFTemplates;
using System.IO;
using System.Threading.Tasks;
using Com.Danliris.Service.Inventory.Lib.Services;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/stock-transfer-notes")]
    [Authorize]
    public class StockTransferNoteController : BaseController<StockTransferNote, StockTransferNoteViewModel, IStockTransferNoteService>
    {
        public StockTransferNoteController(IIdentityService identityService, IValidateService validateService, IStockTransferNoteService service) : base(identityService, validateService, service, "1.0.0")
        {
        }

        [HttpPut("approve/{Id}")]
        public async Task<IActionResult> Approve([FromRoute] int Id)
        {
            try
            {
                VerifyUser();
                bool result = await this.Service.UpdateIsApprove(Id);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    Dictionary<string, object> Result =
                       new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, "false")
                       .Fail();
                    return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
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

        [HttpGet("by-not-user")]
        public IActionResult GetByUser(int Page = 1, int Size = 25, string Order = "{}", [Bind(Prefix = "Select[]")]List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            try
            {
                VerifyUser();
                Tuple<List<StockTransferNote>, int, Dictionary<string, string>, List<string>> Data = Service.ReadModelByNotUser(Page, Size, Order, Select, Keyword, Filter);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data.Item1, Service.MapToViewModel, Page, Size, Data.Item2, Data.Item1.Count, Data.Item3, Data.Item4);

                return Ok(Result);
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
