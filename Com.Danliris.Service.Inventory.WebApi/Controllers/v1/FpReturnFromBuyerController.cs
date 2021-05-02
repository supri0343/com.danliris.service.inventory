using Com.Danliris.Service.Inventory.Lib.Models.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.Lib.ViewModels.FpReturnFromBuyers;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/fp-return-from-buyers")]
    [Authorize]
    public class FpReturnFromBuyerController : BaseController<FpReturnFromBuyerModel, FpReturnFromBuyerViewModel, IFpReturnFromBuyerService>
    {
        public FpReturnFromBuyerController(IIdentityService identityService, IValidateService validateService, IFpReturnFromBuyerService service) : base(identityService, validateService, service, "1.0.0")
        {

        }
    }
}
