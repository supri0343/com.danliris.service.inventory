using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Helpers
{
    public class QueryHelperTest
    {
        [Fact]
        public void Filter_Return_Succes()
        {
            var query = new List<StockTransferNote>()
                        {
                            new StockTransferNote()
                        }.AsQueryable();

            Dictionary<string, object> filterDictionary = new Dictionary<string, object>();
            filterDictionary.Add("Code", "");
            var result = QueryHelper<StockTransferNote>.Filter(query, filterDictionary);
            Assert.NotNull(result);
        }

        [Fact]
        public void Order_Return_Succes()
        {
            var query = new List<StockTransferNote>()
                        {
                            new StockTransferNote()
                        }.AsQueryable();

            Dictionary<string, string> order = new Dictionary<string, string>();
            order.Add("Code", General.DESCENDING);
            var result = QueryHelper<StockTransferNote>.Order(query, order);
            Assert.NotNull(result);
        }

        [Fact]
        public void Search_Return_Succes()
        {
            var query = new List<StockTransferNote>()
                        {
                            new StockTransferNote()
                        }.AsQueryable();

            List<string> searchAttributes = new List<string>()
            {
                "Code"
            };
            var result = QueryHelper<StockTransferNote>.Search(query, searchAttributes,"",true);
            Assert.NotNull(result);
        }
    }
}
