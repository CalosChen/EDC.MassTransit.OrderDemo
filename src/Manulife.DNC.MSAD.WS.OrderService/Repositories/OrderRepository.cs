using Manulife.DNC.MSAD.Common;
using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.OrderService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public OrderDbContext OrderContext { get; }

        public OrderRepository(OrderDbContext OrderContext)
        {
            this.OrderContext = OrderContext;
        }

        public async Task<bool> CreateOrder(IOrder order)
        {
            var orderEntity = new Order()
            {
                ID = GenerateOrderID(),
                OrderUserID = order.OrderUserID,
                OrderTime = order.OrderTime,
                OrderItems = null,
            };

            OrderContext.Orders.Add(orderEntity);

            // patch data
            order.ID = orderEntity.ID;
            order.StatusKey = EventConstants.EVENT_STATUS_KEY_STORAGE;

            var eventEntityA = new Event()
            {
                ID = GenerateEventID(),
                OrderID = orderEntity.ID,
                Type = EventConstants.EVENT_TYPE_CREATE_ORDER,
                StatusKey = EventConstants.EVENT_STATUS_KEY_STORAGE,
                StatusValue = EventStatusEnum.UNHANDLE,
                CreatedTime = DateTime.Now,
                EntityJson = JsonHelper.SerializeObject(order)
            };

            order.StatusKey = EventConstants.EVENT_STATUS_KEY_DELIVERY;

            var eventEntityB = new Event()
            {
                ID = GenerateEventID(),
                OrderID = orderEntity.ID,
                Type = EventConstants.EVENT_TYPE_CREATE_ORDER,
                StatusKey = EventConstants.EVENT_STATUS_KEY_DELIVERY,
                StatusValue = EventStatusEnum.UNHANDLE,
                CreatedTime = DateTime.Now,
                EntityJson = JsonHelper.SerializeObject(order)
            };

            OrderContext.Events.Add(eventEntityA);
            OrderContext.Events.Add(eventEntityB);

            int count = await OrderContext.SaveChangesAsync();

            return count == 3;
        }

        private string GenerateOrderID()
        {
            // TODO: Some business logic to generate Order ID
            return Guid.NewGuid().ToString();
        }

        private string GenerateEventID()
        {
            // TODO: Some business logic to generate Order ID
            return Guid.NewGuid().ToString();
        }
    }
}
