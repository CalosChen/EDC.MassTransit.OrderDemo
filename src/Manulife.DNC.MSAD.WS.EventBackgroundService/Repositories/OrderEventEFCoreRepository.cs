using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.EventService.Models;
using Microsoft.EntityFrameworkCore;

namespace Manulife.DNC.MSAD.WS.EventService.Repositories
{
    public class OrderEventEFCoreRepository : IEventRepository<IOrderEventEntity>
    {
        public OrderDbContext OrderContext { get; }

        public OrderEventEFCoreRepository(OrderDbContext OrderContext)
        {
            this.OrderContext = OrderContext;
        }

        public async Task<IEnumerable<IOrderEventEntity>> GetEvents(string eventType)
        {
            var events = await OrderContext.Events.Where(e => 
                e.Type == eventType &&
                e.StatusValue == EventStatusEnum.UNHANDLE).ToListAsync();

            return events as IEnumerable<IOrderEventEntity>;
        }

        public async Task<bool> UpdateEventStatus(string eventType, IOrderEventEntity orderEvent)
        {
            var currentEvent = await OrderContext.Events.FirstOrDefaultAsync(e =>
                e.OrderID == orderEvent.OrderID &&
                e.Type == eventType &&
                e.StatusKey == orderEvent.StatusKey &&
                e.StatusValue == EventStatusEnum.UNHANDLE);

            if (currentEvent == null)
            {
                return true;
            }

            // mark it as handled
            currentEvent.StatusValue = EventStatusEnum.HANDLED;

            var count = await OrderContext.SaveChangesAsync();
            return count > 0;
        }
    }
}
