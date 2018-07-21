using Manulife.DNC.MSAD.Common;
using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.EventService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.StorageService.Repositories
{
    public class StorageRepsitory : IStorageRepository
    {
        public OrderDbContext  OrderContext{ get; }

        public StorageRepsitory(OrderDbContext OrderContext)
        {
            this.OrderContext = OrderContext;
        }

        public async Task<bool> CreateStorage(IOrder order)
        {
            var eventList = await OrderContext.Events.Where(e => e.OrderID == order.ID 
                && e.Type == EventConstants.EVENT_TYPE_CREATE_ORDER
                && e.StatusKey == EventConstants.EVENT_STATUS_KEY_STORAGE).ToListAsync();

            if (eventList == null)
            {
                return false;
            }

            foreach (var eventItem in eventList)
            {
                try
                {
                    // TODO : Add record to Storage DB
                    Console.WriteLine($"Add one record to Storage DB : { JsonHelper.SerializeObject(order) }");
                }
                catch (Exception ex)
                {
                    // TODO : Exception log
                    Console.WriteLine($"Exception: {ex.Message}");
                    return false;
                }
            }

            return true;
        }
    }
}
