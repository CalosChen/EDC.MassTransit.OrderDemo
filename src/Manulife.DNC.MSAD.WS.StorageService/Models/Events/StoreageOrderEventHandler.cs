using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.StorageService.Repositories;
using MassTransit;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.StorageService.Models
{
    public class StoreageOrderEventHandler : IConsumer<IOrder>
    {
        public IStorageRepository StorageRepository { get; }
        public IBusControl EventBus { get; }

        public StoreageOrderEventHandler(IStorageRepository StorageRepository)
        {
            this.StorageRepository = StorageRepository;
            this.EventBus = Startup.BusControl;
        }

        public async Task Consume(ConsumeContext<IOrder> context)
        {
            var order = context.Message;
            if (order.StatusKey != EventConstants.EVENT_STATUS_KEY_STORAGE)
            {
                // 如果不是StorageService要处理的Event则忽略该消息
                return;
            }

            var result = StorageRepository.CreateStorage(order).GetAwaiter().GetResult();
            if (result)
            {
                IOrderEventEntity orderEventEntity = new OrderEventEntity
                {
                    OrderID = order.ID,
                    EventType = EventConstants.EVENT_TYPE_CREATE_ORDER,
                    StatusKey = EventConstants.EVENT_STATUS_KEY_STORAGE,
                    StatusValue = EventStatusEnum.HANDLED
                };

                await EventBus.Publish(orderEventEntity);
            }
        }
    }
}
