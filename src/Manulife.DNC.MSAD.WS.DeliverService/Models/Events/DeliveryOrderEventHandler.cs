using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.DeliveryService.Repositories;
using MassTransit;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.DeliveryService.Models
{
    public class DeliveryOrderEventHandler : IConsumer<IOrder>
    {
        public IDeliveryRepository DeliveryRepository { get; }
        public IBusControl EventBus { get; }

        public DeliveryOrderEventHandler(IDeliveryRepository StorageRepository)
        {
            this.DeliveryRepository = StorageRepository;
            this.EventBus = Startup.BusControl;
        }

        public async Task Consume(ConsumeContext<IOrder> context)
        {
            var order = context.Message;
            if (order.StatusKey != EventConstants.EVENT_STATUS_KEY_DELIVERY)
            {
                // 如果不是DeliveryService要处理的Event则忽略该消息
                return;
            }

            var result = DeliveryRepository.CreateDelivery(order).GetAwaiter().GetResult();
            if (result)
            {
                IOrderEventEntity orderEventEntity = new OrderEventEntity
                {
                    OrderID = order.ID,
                    EventType = EventConstants.EVENT_TYPE_CREATE_ORDER,
                    StatusKey = EventConstants.EVENT_STATUS_KEY_DELIVERY,
                    StatusValue = EventStatusEnum.HANDLED
                };

                await EventBus.Publish(orderEventEntity);
            }
        }
    }
}
