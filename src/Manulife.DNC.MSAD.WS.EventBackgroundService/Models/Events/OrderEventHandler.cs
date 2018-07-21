using System;
using System.Threading.Tasks;
using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.EventService.Repositories;
using MassTransit;

namespace Manulife.DNC.MSAD.WS.EventService.Models
{
    public class OrderEventHandler : IConsumer<IOrderEventEntity>
    {
        public IEventRepository<IOrderEventEntity> EventRepository { get; }

        public OrderEventHandler(IEventRepository<IOrderEventEntity> EventRepository)
        {
            this.EventRepository = EventRepository;
        }

        public async Task Consume(ConsumeContext<IOrderEventEntity> context)
        {
            var eventResult = await EventRepository.UpdateEventStatus(EventConstants.EVENT_TYPE_CREATE_ORDER, context.Message);
        }
    }
}
