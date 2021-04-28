using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public class InventoryWeavingMovementService : IInventoryWeavingMovementService
    {
        private string USER_AGENT = "Service";
        //private const string UserAgent = "inventory-service";
        protected DbSet<InventoryWeavingMovement> DbSet;
        public IIdentityService IdentityService;
        public readonly IServiceProvider ServiceProvider;
        public InventoryDbContext DbContext;

        public InventoryWeavingMovementService(IServiceProvider serviceProvider, InventoryDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<InventoryWeavingMovement>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            //IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> Create(InventoryWeavingMovement model, string username)
        {
            

            int Created = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.FlagForCreate(username, USER_AGENT);
                    model.FlagForUpdate(username, USER_AGENT);
                    DbSet.Add(model);
                    Created = await DbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Insert Error : " + e.Message);
                }
            }

            return Created;
        }
    }
}
