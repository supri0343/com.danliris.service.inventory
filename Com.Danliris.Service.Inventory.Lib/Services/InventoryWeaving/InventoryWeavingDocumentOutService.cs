using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public class InventoryWeavingDocumentOutService : IInventoryWeavingDocumentOutService
    {
        private string USER_AGENT = "Service";
        private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingDocumentItem> DbSetItem;
        protected DbSet<InventoryWeavingMovement> DbSetMovement;
        protected DbSet<InventoryWeavingDocument> DbSetDoc;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;


        public InventoryWeavingDocumentOutService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSetItem = dbContext.Set<InventoryWeavingDocumentItem>();
            DbSetMovement = dbContext.Set<InventoryWeavingMovement>();
            DbSetDoc = dbContext.Set<InventoryWeavingDocument>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public ListResult<InventoryWeavingDocument> Read(int page, int size, string order, string keyword, string filter)
        {
            var query = this.DbSetDoc.Where(s => s.Type == "OUT");


            List<string> SearchAttributes = new List<string>()
            {
                "BonNo"
            };

            query = QueryHelper<InventoryWeavingDocument>.Search(query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<InventoryWeavingDocument>.Filter(query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<InventoryWeavingDocument>.Order(query, OrderDictionary);
            var data = query.Skip((page - 1) * size).Take(size).Select(s => new InventoryWeavingDocument()
            {
                Id = s.Id,

                Date = s.Date,
                BonNo = s.BonNo,
                BonType = s.BonType,
                StorageCode = s.StorageCode,
                StorageId = s.StorageId,
                StorageName = s.StorageName,
                Type = s.Type,
                _LastModifiedUtc = s._LastModifiedUtc,
            });

            return new ListResult<InventoryWeavingDocument>(data.ToList(), page, size, data.Count());
        }

        public ReadResponse<InventoryWeavingDocumentItem> GetDistinctMaterial(int page, int size, string filter, string order, string keyword)
        {
            IQueryable<InventoryWeavingDocumentItem> Query = this.DbSetItem;


            List<string> SearchAttributes = new List<string>()
                {
                    "Construction"
                };
            Query = QueryHelper<InventoryWeavingDocumentItem>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<InventoryWeavingDocumentItem>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<InventoryWeavingDocumentItem>.Order(Query, OrderDictionary);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "Construction", 
            };

            var Data = Query.GroupBy(d => d.Construction)
                .Select(s => s.First())
                .Skip((page - 1) * size).Take(size)
                .OrderBy(s => s.Construction)
                .Select(s => new InventoryWeavingDocumentItem
                {
                    Id = s.Id,
                    Construction = s.Construction,
                });


            #region OrderBy

            //Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //Query = QueryHelper<InventoryWeavingDocumentItem>.Order(Query, OrderDictionary);
            #endregion OrderBy

            #region Paging

            Pageable<InventoryWeavingDocumentItem> pageable = new Pageable<InventoryWeavingDocumentItem>(Data, page - 1, size);
            List<InventoryWeavingDocumentItem> data = pageable.Data.ToList<InventoryWeavingDocumentItem>();
            int totalData = pageable.TotalCount;

            #endregion Paging

            return new ReadResponse<InventoryWeavingDocumentItem>(data, totalData, OrderDictionary, new List<string>());
        }

        public List<InventoryWeavingItemDetailViewModel> GetMaterialItemList(string material)
        {
            IQueryable<InventoryWeavingMovement> query = this.DbSetMovement;

           if (material == null)
            {

                query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s._IsDeleted.Equals(false)).Take(50);
            }
            else
            {
                query = query.OrderByDescending(s => s._LastModifiedUtc).Where(s => s.Construction.Contains(material) && s._IsDeleted.Equals(false)).Take(50);

            }

           var data = query.GroupBy( s => new { s.Grade, s.Piece, s.Type, s.MaterialName, s.WovenType, s.Width,
                      s.Yarn1, s.Yarn2, s.YarnOrigin1, s.YarnOrigin2, s.YarnType1, s.YarnType2}).Select( s => new InventoryWeavingInOutViewModel()
           {
               Construction = s.FirstOrDefault().Construction,
               Grade = s.Key.Grade,
               Piece = s.Key.Piece,

               MaterialName = s.Key.MaterialName,
               WovenType = s.Key.WovenType,
               Width = s.Key.Width,
               Yarn1 = s.Key.Yarn1,
               Yarn2 = s.Key.Yarn2,
               YarnOrigin1 = s.Key.YarnOrigin1,
               YarnOrigin2 = s.Key.YarnOrigin2,
               YarnType1 = s.Key.YarnType1,
               YarnType2 = s.Key.YarnType2,
               UomUnit = s.FirstOrDefault().UomUnit,
               Type = s.Key.Type,
               Qty = s.Sum( d => d.Quantity),
               QtyPiece = s.Sum(d => d.QuantityPiece),

               //QtyOut = 0,
               //QtyPieceOut = 0

           });

           // var dataOUT = query.GroupBy(s => new { s.Construction, s.Grade, s.Piece }).Where(x => x.FirstOrDefault().Type == "OUT").Select(s => new InventoryWeavingInOutViewModel()
           // {
           //     Construction = s.Key.Construction,
           //     Grade = s.Key.Grade,
           //     Piece = s.Key.Piece,
           //     Type = s.FirstOrDefault().Type,
           //     QtyIn = 0,
           //     QtyPieceIn =0,
           //     QtyOut = s.Sum(d => d.Quantity),
           //     QtyPieceOut = s.Sum( d=> d.QuantityPiece)
           //});


          

            var result = data.GroupBy(s => new {
                s.MaterialName,
                s.WovenType,
                s.Width,
                s.Yarn1,
                s.Yarn2,
                s.YarnOrigin1,
                s.YarnOrigin2,
                s.YarnType1,
                s.YarnType2
            }).Select(s => new InventoryWeavingItemDetailViewModel()
            {

                Construction = s.FirstOrDefault().Construction,
                ListItems = s.GroupBy(x => new { x.Grade, x.Piece }).Select(item => new ItemListDetailViewModel()
                {
                    Grade = item.Key.Grade,
                    Piece = item.Key.Piece == "1" ? "BESAR": item.Key.Piece == "2" ? "KECIL" : "POTONGAN",

                    MaterialName = item.FirstOrDefault().MaterialName,
                    WovenType = item.FirstOrDefault().WovenType,
                    Width = item.FirstOrDefault().Width,
                    Yarn1 = item.FirstOrDefault().Yarn1,
                    Yarn2  = item.FirstOrDefault().Yarn2,
                    YarnOrigin1 = item.FirstOrDefault().YarnOrigin1,
                    YarnOrigin2 = item.FirstOrDefault().YarnOrigin2,
                    YarnType1 = item.FirstOrDefault().YarnType1,
                    YarnType2 = item.FirstOrDefault().YarnType2,
                    //Width = item.FirstOrDefault().Width,
                    UomUnit = item.FirstOrDefault().UomUnit,

                    //Quantity = 0,
                    //QuantityPiece = 0,
                    Quantity = item.FirstOrDefault(d => d.Type == "OUT") != null ? item.FirstOrDefault(d => d.Type == "IN").Qty - item.FirstOrDefault(d => d.Type == "OUT").Qty :
                                item.FirstOrDefault(d => d.Type == "IN").Qty,
                    QuantityPiece = item.FirstOrDefault(d => d.Type == "OUT") != null ? item.FirstOrDefault(d => d.Type == "IN").QtyPiece - item.FirstOrDefault(d => d.Type == "OUT").QtyPiece :
                                item.FirstOrDefault(d => d.Type == "IN").QtyPiece
                    //Quantity = item.FirstOrDefault().QtyOut== null ? item.FirstOrDefault().QtyIn : item.FirstOrDefault().QtyIn - item.FirstOrDefault().QtyOut
                }).Where( x => x.Quantity >0 && x.QuantityPiece > 0).ToList()
            });

            //var data = query.GroupBy(s => new { s.Construction }).Select(s => new InventoryWeavingItemDetailViewModel()
            //{
            //    Construction = s.Key.Construction,
            //    ListItems = s.GroupBy(x => new { x.Grade, x.Piece}).Select(item => new ItemListDetailViewModel()
            //    {
            //        Grade = item.Key.Grade,
            //        Piece = item.Key.Piece,
            //        //Quantity = item.First().Quantity,
            //        Quantity = (item.FirstOrDefault().Type == "OUT") == null ? item.FirstOrDefault(d => d.Type == "IN").Quantity
            //                    : 
            //                    item.FirstOrDefault(d => d.Type == "OUT").Quantity

            //        //Quantity = (item.FirstOrDefault().Type == "OUT") == null ? item.FirstOrDefault(d => d.Type == "IN").Quantity 
            //        //            : item.FirstOrDefault(d => d.Type == "IN").Quantity -
            //        //            item.FirstOrDefault(d => d.Type == "OUT").Quantity

            //    }).ToList()

            //});

            return result.ToList();
        }

        public async Task<InventoryWeavingDocument> MapToModel(InventoryWeavingDocumentOutViewModel data)
        {
            List<InventoryWeavingDocumentItem> DocsItems = new List<InventoryWeavingDocumentItem>();
            foreach (var i in data.items) {
                i.ListItems = i.ListItems.Where(s => s.IsSave).ToList();
                foreach (var d in i.ListItems) {
                    DocsItems.Add( new InventoryWeavingDocumentItem
                    {
                        Id = d.Id,
                        Active = d.Active,
                        _CreatedBy = d._CreatedBy,
                        _CreatedUtc = d._CreatedUtc,
                        _CreatedAgent = d._CreatedAgent,
                        _LastModifiedBy = d._LastModifiedBy,
                        _LastModifiedUtc = d._LastModifiedUtc,
                        _LastModifiedAgent = d._LastModifiedAgent,
                        _IsDeleted = d._IsDeleted,
                        ProductOrderName = i.ProductOrderNo,
                        ReferenceNo = i.ReferenceNo,
                        Construction = i.Construction,

                        Grade = d.Grade,
                        Piece = d.Piece == "BESAR" ? "1" : d.Piece == "KECIL"? "2" : "3",
                        MaterialName = d.MaterialName,
                        WovenType = d.WovenType,
                        Width = d.Width,
                        Yarn1 = d.Yarn1,
                        Yarn2 = d.Yarn2,
                        YarnType1 = d.YarnType1,
                        YarnType2 = d.YarnType2,
                        YarnOrigin1 = d.YarnOrigin1,
                        YarnOrigin2 = d.YarnOrigin2,

                        UomId = 35,
                        UomUnit = "MTR",
                        Quantity = d.Qty,
                        QuantityPiece = d.QtyPiece,
                        ProductRemark = d.ProductRemark,
                        //InventoryWeavingDocumentId = d.InventoryWeavingDocumentId
                    });


                }
            }

            InventoryWeavingDocument model = new InventoryWeavingDocument
            {
                BonNo = data.bonNo,
                BonType = data.bonType,
                Date = data.date,
                StorageId = 105,
                StorageCode = "DNWDX2GZ",
                StorageName = "WEAVING 2 (EX. WEAVING 3) / WEAVING",
                Remark = data.remark,
                Type = "OUT",
                Items = DocsItems
               
            };
            return model;

        }


        public async Task Create(InventoryWeavingDocument model)
        {
            //var bonCheck = this.DbSetDoc.FirstOrDefault(s => s.Date.Date == model.Date.Date && s.BonType == model.BonType && s.Type == "OUT");

            var bonCheck = this.DbContext.InventoryWeavingDocuments.Where(s => s.Date.Date == model.Date.Date && s.BonType == model.BonType && s.Type == "OUT").FirstOrDefault();

            if (bonCheck == null)
            {
                model.BonNo = GenerateBon(model.BonType, model.Date);
                model.FlagForCreate(IdentityService.Username, UserAgent);
                model.FlagForUpdate(IdentityService.Username, UserAgent);

                foreach (var item in model.Items)
                {
                    item.FlagForCreate(IdentityService.Username, UserAgent);
                    item.FlagForUpdate(IdentityService.Username, UserAgent);
                }

                DbSetDoc.Add(model);

                var result = await DbContext.SaveChangesAsync();

                foreach (var item in model.Items)
                {
                    InventoryWeavingMovement movement = new InventoryWeavingMovement
                    {
                        ProductOrderName = item.ProductOrderName,
                        BonNo = model.BonNo,
                        ReferenceNo = item.ReferenceNo,
                        Construction = item.Construction,
                        Grade = item.Grade,
                        Piece = item.Piece,
                        MaterialName = item.MaterialName,
                        WovenType = item.WovenType,
                        Width = item.Width,
                        Yarn1 = item.Yarn1,
                        Yarn2 = item.Yarn2,
                        YarnType1 = item.YarnType1,
                        YarnType2 = item.YarnType2,
                        YarnOrigin1 = item.YarnOrigin1,
                        YarnOrigin2 = item.YarnOrigin2,
                        UomId = item.UomId,
                        UomUnit = item.UomUnit,
                        Quantity = item.Quantity,
                        QuantityPiece = item.QuantityPiece,
                        ProductRemark = item.ProductRemark,
                        Type = model.Type,
                        InventoryWeavingDocumentId = model.Id,
                        InventoryWeavingDocumentItemId = item.Id
                    };

                    movement.FlagForCreate(IdentityService.Username, UserAgent);
                    movement.FlagForUpdate(IdentityService.Username, UserAgent);
                    DbSetMovement.Add(movement);
                }

                var result2 = await DbContext.SaveChangesAsync();

            }
            else
            {



                foreach (var i in model.Items)
                {
                    InventoryWeavingDocumentItem DocItem = new InventoryWeavingDocumentItem()
                    {
                        ProductOrderName = i.ProductOrderName,
                        ReferenceNo = i.ReferenceNo,
                        Construction = i.Construction,

                        Grade = i.Grade,
                        Piece = i.Piece,
                        MaterialName = i.MaterialName,
                        WovenType = i.WovenType,
                        Width = i.Width,
                        Yarn1 = i.Yarn1,
                        Yarn2 = i.Yarn2,
                        YarnType1 = i.YarnType1,
                        YarnType2 = i.YarnType2,
                        YarnOrigin1 = i.YarnOrigin1,
                        YarnOrigin2 = i.YarnOrigin2,

                        UomId = 35,
                        UomUnit = "MTR",
                        Quantity = i.Quantity,
                        QuantityPiece = i.QuantityPiece,
                        ProductRemark = i.ProductRemark,
                        InventoryWeavingDocumentId = bonCheck.Id

                    };

                    DocItem.FlagForCreate(IdentityService.Username, UserAgent);
                    DocItem.FlagForUpdate(IdentityService.Username, UserAgent);
                    DbSetItem.Add(DocItem);
                }
                var result = await DbContext.SaveChangesAsync();
                //DbSetItem.Add(model.Items);

                foreach (var item in model.Items)
                {
                    InventoryWeavingMovement movement = new InventoryWeavingMovement
                    {
                        ProductOrderName = item.ProductOrderName,
                        BonNo = model.BonNo,
                        ReferenceNo = item.ReferenceNo,
                        Construction = item.Construction,
                        Grade = item.Grade,
                        Piece = item.Piece,
                        MaterialName = item.MaterialName,
                        WovenType = item.WovenType,
                        Width = item.Width,
                        Yarn1 = item.Yarn1,
                        Yarn2 = item.Yarn2,
                        YarnType1 = item.YarnType1,
                        YarnType2 = item.YarnType2,
                        YarnOrigin1 = item.YarnOrigin1,
                        YarnOrigin2 = item.YarnOrigin2,
                        UomId = item.UomId,
                        UomUnit = item.UomUnit,
                        Quantity = item.Quantity,
                        QuantityPiece = item.QuantityPiece,
                        ProductRemark = item.ProductRemark,
                        Type = model.Type,
                        InventoryWeavingDocumentId = bonCheck.Id,
                        InventoryWeavingDocumentItemId = item.Id
                    };

                    movement.FlagForCreate(IdentityService.Username, UserAgent);
                    movement.FlagForUpdate(IdentityService.Username, UserAgent);
                    DbSetMovement.Add(movement);
                }

                var result2 = await DbContext.SaveChangesAsync();
            }


        }

        private string GenerateBon(string from, DateTimeOffset date)
        {
            
            
            var type = from == "PACKING" ? "PC" : from == "FINISHING" ? "FN" : from == "PRINTING" ? "PR" 
                      :from == "PRODUKSI" ? "PR"  : from == "KOTOR" ? "KR" : from == "INSPECTING WEAVING" ? "IW" : "LL";

            var totalData = DbSetDoc.Count(s => s.BonNo.Substring(3,2) == type && s._CreatedUtc.Year == date.Date.Year)+1;

           
            return string.Format("{0}.{1}.{2}.{3}","GD", type, date.ToString("yy"), totalData.ToString().PadLeft(4, '0'));
            
        }

        public InventoryWeavingDocumentDetailViewModel ReadById(int id)
        {
            var data = this.DbSetDoc.Where(d => d.Id.Equals(id) && d._IsDeleted.Equals(false))
                 .Include(p => p.Items).FirstOrDefault();


            if (data == null)
                return null;

            InventoryWeavingDocumentDetailViewModel DocsItems = MapToViewModelById(data);


            return DocsItems;
        }

        private InventoryWeavingDocumentDetailViewModel MapToViewModelById(InventoryWeavingDocument model)
        {
            var vm = new InventoryWeavingDocumentDetailViewModel()
            {
                Active = model.Active,
                Id = model.Id,
                _CreatedAgent = model._CreatedAgent,
                _CreatedBy = model._CreatedBy,
                _CreatedUtc = model._CreatedUtc,
                _IsDeleted = model._IsDeleted,
                _LastModifiedAgent = model._LastModifiedAgent,
                _LastModifiedBy = model._LastModifiedBy,
                _LastModifiedUtc = model._LastModifiedUtc,
                BonNo = model.BonNo,
                Date = model.Date,
                BonType = model.BonType,
                StorageId = model.StorageId,
                StorageCode = model.StorageCode,
                StorageName = model.StorageName,
                Type = model.Type,

                Detail = model.Items.GroupBy(x => x.Construction).Select(item => new InventoryWeavingItemDetailViewModel()
                {
                
                    Construction = item.First().Construction,
                  

                    ListItems = item.Where( s=> s._IsDeleted.Equals(false)).Select(s => new ItemListDetailViewModel()
                    {
                        Active = s.Active,
                        _CreatedAgent = s._CreatedAgent,
                        _CreatedBy = s._CreatedBy,
                        _CreatedUtc = s._CreatedUtc,

                        Id = s.Id,
                        _IsDeleted = s._IsDeleted,
                        _LastModifiedAgent = s._LastModifiedAgent,
                        _LastModifiedBy = s._LastModifiedBy,
                        _LastModifiedUtc = s._LastModifiedUtc,

                        Grade = s.Grade,
                        Piece = s.Piece == "1" ? "BESAR" : s.Piece == "2"? "KECIL": "POTONGAN",
                        MaterialName = s.MaterialName,
                        WovenType = s.WovenType,
                        Yarn1 = s.Yarn1,
                        Yarn2 = s.Yarn2,
                        YarnType1 = s.YarnType1,
                        YarnType2 = s.YarnType2,
                        YarnOrigin1 = s.YarnOrigin1,
                        YarnOrigin2 = s.YarnOrigin2,
                        YarnOrigin = s.YarnOrigin1 + " / " + s.YarnOrigin2,
                        Width = s.Width,
                        UomUnit = s.UomUnit,
                        Quantity = s.Quantity,
                        QuantityPiece = s.QuantityPiece,
                        ProductRemark = s.ProductRemark
                    }).ToList()
                }).ToList()
            };
            return vm;
        }
    }
}
