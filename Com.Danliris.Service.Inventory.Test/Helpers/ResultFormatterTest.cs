using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.StockTransferNoteViewModel;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Helpers
{
    public class ResultFormatterTest
    {
        [Fact]
        public void Should_Success_OK()
        {
            //Setup
            string ApiVersion = "V1";
            int StatusCode = 200;
            string Message = "OK";
            ResultFormatter formatter = new ResultFormatter(ApiVersion, StatusCode, Message);
            
            StockTransferNote model = new StockTransferNote();
            
            Dictionary<string, string> Order = new Dictionary<string, string>();
            Order.Add("Code", "asc");

            List<string> Select = new List<string>()
            {
                "Code"
            };

            //Act
            var result = formatter.Ok<StockTransferNote>(new List<StockTransferNote>() { model }, 1, 1, 10, 10, Order, Select);
            
            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_OK_with_mapping()
        {
            string ApiVersion = "V1";
            int StatusCode = 200;
            string Message = "OK";

            Func<StockTransferNote, StockTransferNoteViewModel> resultMaping = new Func<StockTransferNote, StockTransferNoteViewModel>(MapToViewModel);

            StockTransferNote data = new StockTransferNote();
          
            ResultFormatter formatter = new ResultFormatter(ApiVersion, StatusCode, Message);
            var result = formatter.Ok<StockTransferNote, StockTransferNoteViewModel>(new List<StockTransferNote>() { data}, resultMaping);
            Assert.True(0 < result.Count());
        }

        [Fact]
        public void Should_OK_with_Mapping_and_Order()
        {
            string ApiVersion = "V1";
            int StatusCode = 200;
            string Message = "OK";

            Func<StockTransferNote, StockTransferNoteViewModel> resultMaping = new Func<StockTransferNote, StockTransferNoteViewModel>(MapToViewModel);
            StockTransferNote data = new StockTransferNote();
            ResultFormatter formatter = new ResultFormatter(ApiVersion, StatusCode, Message);

            Dictionary<string, string> order = new Dictionary<string, string>();
            order.Add("Code", "asc");

            List<string> select = new List<string>()
            {
                "Code"
            };

            var result = formatter.Ok<StockTransferNote, StockTransferNoteViewModel>(new List<StockTransferNote>() { data }, resultMaping,1,25,10,1,order,select);
            Assert.True(0 < result.Count());
        }

        static StockTransferNoteViewModel MapToViewModel(StockTransferNote basic)
        {
            return new StockTransferNoteViewModel()
            {
                Code=basic.Code
            };
        }

        [Fact]
        public void Fail_Return_Success()
        {
            //Setup
            string ApiVersion = "V1";
            int StatusCode = 200;
            string Message = "OK";

            StockTransferNoteViewModel viewModel = new StockTransferNoteViewModel();
            ResultFormatter formatter = new ResultFormatter(ApiVersion, StatusCode, Message);
            ValidationContext validationContext = new ValidationContext(viewModel);

            var errorData = new
            {
                WarningError = "Format Not Match"
            };

            string error = JsonConvert.SerializeObject(errorData);
            var exception = new ServiceValidationExeption(validationContext, new List<ValidationResult>() { new ValidationResult(error, new List<string>() { "WarningError"}) });
            
            //Act
            var result = formatter.Fail(exception);

            //Assert
            Assert.True(0 < result.Count());
        }

        [Fact]
        public void Fail_Throws_Exception()
        {
            //Setup
            string ApiVersion = "V1";
            int StatusCode = 200;
            string Message = "OK";

            StockTransferNoteViewModel viewModel = new StockTransferNoteViewModel();
            ResultFormatter formatter = new ResultFormatter(ApiVersion, StatusCode, Message);
            ValidationContext validationContext = new ValidationContext(viewModel);
            var exception = new ServiceValidationExeption(validationContext, new List<ValidationResult>() { new ValidationResult("errorMessaage", new List<string>() { "WarningError" }) });

            //Act
            var result = formatter.Fail(exception);

            //Assert
            Assert.True(0 < result.Count());
        }
    }
}
