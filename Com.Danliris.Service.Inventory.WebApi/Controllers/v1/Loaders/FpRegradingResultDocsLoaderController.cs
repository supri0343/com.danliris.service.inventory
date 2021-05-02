using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.Services.FpRegradingResultDocs;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.Loaders
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/fp-regrading-result-docs-loader")]
    [Authorize]
    public class FpRegradingResultDocsLoaderController : Controller
    {
        private IIdentityService IdentityService;
        private readonly IValidateService ValidateService;
        private readonly IFpRegradingResultDocsService Service;
        private readonly string ApiVersion;

        public FpRegradingResultDocsLoaderController(IIdentityService identityService, IValidateService validateService, IFpRegradingResultDocsService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            ApiVersion = "1.0.0";
        }

        [HttpGet]
        public ActionResult GetNo(string Keyword, string Filter = "{}")
        {
            try
            {
                Tuple<List<FpRegradingResultDocs>, int, Dictionary<string, string>, List<string>> Data = Service.ReadNo(Keyword, Filter);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok<FpRegradingResultDocs, FpRegradingResultDocsViewModel>(Data.Item1, Service.MapToViewModel, 1, 25, Data.Item2, Data.Item1.Count, Data.Item3, Data.Item4);

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