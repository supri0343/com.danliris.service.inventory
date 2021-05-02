using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving
{
    public interface IInventoryWeavingMovementService
    {
        Task<int> Create(InventoryWeavingMovement model , string username);
    }
}
