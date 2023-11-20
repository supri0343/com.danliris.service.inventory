using Com.Moonlay.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Configs;
using Com.Danliris.Service.Inventory.Lib.Configs.MaterialDistributionNoteConfig;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Danliris.Service.Inventory.Lib.Models.StockTransferNoteModel;
using Com.Danliris.Service.Inventory.Lib.Configs.StockTransferNoteConfig;
using Com.Danliris.Service.Inventory.Lib.Configs.MaterialsRequestNoteConfig;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Configs.FPReturnInvToPurchasingConfig;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Configs.InventoriesConfig;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodModels;

using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricConfigs;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodConfigs;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalModels;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalConfigs;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureFabric;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ReceiptAccessories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.BalanceStock;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Configs.InventoryWeavingsConfig;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ExpenditureWaste;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentWasteProductionConfig.ReceiptWaste;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentLeftoverWarehouse.GarmentExpenditureWasteProductionsConfig;
using Com.Danliris.Service.Inventory.Lib.Configs.GarmentWasteProductionConfig.ExpenditureWasteConfig;
using com.Danliris.Service.Inventory.Lib.Models.LogHistory;

namespace Com.Danliris.Service.Inventory.Lib
{
    public class InventoryDbContext : BaseDbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<MaterialsRequestNote> MaterialsRequestNotes { get; set; }
        public DbSet<MaterialsRequestNote_Item> MaterialsRequestNote_Items { get; set; }
        public DbSet<FpRegradingResultDocsDetails> fpRegradingResultDocsDetails { get; set; }
        public DbSet<FpRegradingResultDocs> fpRegradingResultDocs { get; set; }
        public DbSet<MaterialDistributionNote> MaterialDistributionNotes { get; set; }
        public DbSet<MaterialDistributionNoteItem> MaterialDistributionNoteItems { get; set; }
        public DbSet<MaterialDistributionNoteDetail> MaterialDistributionNoteDetails { get; set; }
        public DbSet<StockTransferNote> StockTransferNotes { get; set; }
        public DbSet<StockTransferNote_Item> StockTransferNoteItems { get; set; }
        public DbSet<FPReturnInvToPurchasing> FPReturnInvToPurchasings { get; set; }
        public DbSet<FPReturnInvToPurchasingDetail> FPReturnInvToPurchasingDetails { get; set; }

        public DbSet<InventoryDocument> InventoryDocuments { get; set; }
        public DbSet<InventoryDocumentItem> InventoryDocumentItems { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<InventorySummary> InventorySummaries { get; set; }
        public DbSet<GarmentLeftoverWarehouseReceiptFabric> GarmentLeftoverWarehouseReceiptFabrics { get; set; }
        public DbSet<GarmentLeftoverWarehouseReceiptFabricItem> GarmentLeftoverWarehouseReceiptFabricItems { get; set; }

        public DbSet<GarmentLeftoverWarehouseReceiptFinishedGood> GarmentLeftoverWarehouseReceiptFinishedGoods { get; set; }
        public DbSet<GarmentLeftoverWarehouseReceiptFinishedGoodItem> GarmentLeftoverWarehouseReceiptFinishedGoodItems { get; set; }

        public DbSet<GarmentLeftoverWarehouseStock> GarmentLeftoverWarehouseStocks { get; set; }
        public DbSet<GarmentLeftoverWarehouseStockHistory> GarmentLeftoverWarehouseStockHistories { get; set; }


        public DbSet<GarmentLeftoverWarehouseReceiptAval> GarmentLeftoverWarehouseReceiptAvals { get; set; }
        public DbSet<GarmentLeftoverWarehouseReceiptAvalItem> GarmentLeftoverWarehouseReceiptAvalItems { get; set; }

        public DbSet<GarmentLeftoverWarehouseExpenditureFinishedGood> GarmentLeftoverWarehouseExpenditureFinishedGoods { get; set; }
        public DbSet<GarmentLeftoverWarehouseExpenditureFinishedGoodItem> GarmentLeftoverWarehouseExpenditureFinishedGoodItems { get; set; }
        public DbSet<GarmentLeftoverWarehouseExpenditureFabric> GarmentLeftoverWarehouseExpenditureFabrics { get; set; }
        public DbSet<GarmentLeftoverWarehouseExpenditureFabricItem> GarmentLeftoverWarehouseExpenditureFabricItems { get; set; }

        public DbSet<GarmentLeftoverWarehouseExpenditureAccessories> GarmentLeftoverWarehouseExpenditureAccessories { get; set; }
        public DbSet<GarmentLeftoverWarehouseExpenditureAccessoriesItem> GarmentLeftoverWarehouseExpenditureAccessoriesItems { get; set; }

        public DbSet<GarmentLeftoverWarehouseExpenditureAval> GarmentLeftoverWarehouseExpenditureAvals { get; set; }
        public DbSet<GarmentLeftoverWarehouseExpenditureAvalItem> GarmentLeftoverWarehouseExpenditureAvalItems { get; set; }

        public DbSet<GarmentLeftoverWarehouseReceiptAccessory> GarmentLeftoverWarehouseReceiptAccessories { get; set; }
        public DbSet<GarmentLeftoverWarehouseReceiptAccessoryItem> GarmentLeftoverWarehouseReceiptAccessoryItems { get; set; }

        public DbSet<GarmentLeftoverWarehouseBalanceStock> GarmentLeftoverWarehouseBalanceStocks { get; set; }
        public DbSet<GarmentLeftoverWarehouseBalanceStockItem> GarmentLeftoverWarehouseBalanceStocksItems { get; set; }
        public DbSet<InventoryWeavingDocument> InventoryWeavingDocuments { get; set; }
        public DbSet<InventoryWeavingDocumentItem> InventoryWeavingDocumentItems { get; set; }
        public DbSet<InventoryWeavingMovement> InventoryWeavingMovements { get; set; }
        public DbSet<GarmentReceiptWasteProductions> GarmentReceiptWasteProductions { get; set; }
        public DbSet<GarmentReceiptWasteProductionItems> GarmentReceiptWasteProductionItems { get; set; }
        public DbSet<GarmentExpenditureWasteProductions> GarmentExpenditureWasteProductions { get; set; }
        public DbSet<GarmentExpenditureWasteProductionItems> GarmentExpenditureWasteProductionItems { get; set; }
        public DbSet<LogHistory> LogHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MaterialsRequestNoteConfig());
            modelBuilder.ApplyConfiguration(new MaterialsRequestNote_ItemConfig());
            modelBuilder.ApplyConfiguration(new FpRegradingResultDocsDetailsConfig());
            modelBuilder.ApplyConfiguration(new FpRegradingResultDocsConfig());
            modelBuilder.ApplyConfiguration(new MaterialDistributionNoteConfig());
            modelBuilder.ApplyConfiguration(new MaterialDistributionNoteItemConfig());
            modelBuilder.ApplyConfiguration(new MaterialDistributionNoteDetailConfig());
            modelBuilder.ApplyConfiguration(new StockTransferNoteConfig());
            modelBuilder.ApplyConfiguration(new StockTransferNoteItemConfig());
            modelBuilder.ApplyConfiguration(new FPReturnInvToPurchasingConfig());
            modelBuilder.ApplyConfiguration(new FPReturnInvToPurchasingDetailConfig());
            modelBuilder.ApplyConfiguration(new InventoryDocumentConfig());
            modelBuilder.ApplyConfiguration(new InventoryDocumentItemConfig());
            modelBuilder.ApplyConfiguration(new InventoryMovementConfig());
            modelBuilder.ApplyConfiguration(new InventorySummaryConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseReceiptFabricConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseReceiptFabricItemConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseReceiptFinishedGoodConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseReceiptFinishedGoodItemConfig());

            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseStockConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseStockHistoryConfig());

            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseReceiptAvalConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseReceiptAvalItemConfig());

            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseExpenditureFinishedGoodConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseExpenditureFinishedGoodItemConfig());

            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseExpenditureFabricConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseExpenditureFabricItemConfig());

            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseExpenditureAccessoriesConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseExpenditureAccessoriesItemConfig());

            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseExpenditureAvalConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseExpenditureAvalItemConfig());

            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseBalanceStockConfig());
            modelBuilder.ApplyConfiguration(new GarmentLeftoverWarehouseBalanceStockItemConfig());
            modelBuilder.ApplyConfiguration(new InventoryWeavingDocumentConfig());
            modelBuilder.ApplyConfiguration(new InventoryWeavingDocumentItemConfig());
            modelBuilder.ApplyConfiguration(new InventoryWeavingMovementConfig());

            modelBuilder.ApplyConfiguration(new GarmentReceiptWasteProductionConfig());
            modelBuilder.ApplyConfiguration(new GarmentReceiptWasteProductionItemConfig());
            modelBuilder.ApplyConfiguration(new GarmentExpenditureWasteProductionsConfig());
            modelBuilder.ApplyConfiguration(new GarmentExpenditureWasteProductionsItemConfig());
        }
    }
}