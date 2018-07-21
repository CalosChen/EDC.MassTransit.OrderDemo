using Manulife.DNC.MSAD.Common;
using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.EventService.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.EventService.Models
{
    public class OrderEventJob: IJob
    {
        public IEventRepository<IOrderEventEntity> OrderEventRepository { get; }
        public IBusControl EventBus { get; }
        private int MaxHours;

        public OrderEventJob(IEventRepository<IOrderEventEntity> OrderEventRepository, IConfiguration configuration)
        {
            this.OrderEventRepository = OrderEventRepository;
            this.EventBus = Startup.BusControl;
            this.MaxHours = Convert.ToInt32(configuration["Service:MaxHours"]);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var events = OrderEventRepository.GetEvents(EventConstants.EVENT_TYPE_CREATE_ORDER).GetAwaiter().GetResult();

            if (events == null)
            {
                await Console.Out.WriteLineAsync($"[Tip] There's no pending to process Order events.");
                return;
            }

            foreach (var eventItem in events)
            {
                if (GetDifferenceInHours(eventItem.CreatedTime) >= MaxHours)
                {
                    // TODO: 
                    // 1.Rollback previous transaction by send rollback message
                    // 2.Send Email/Messages to administrator
                    // ......

                    break;
                }

                IOrder order = JsonHelper.DeserializeJsonToObject<Order>(eventItem.EntityJson);
                await EventBus.Publish(order);
            }
        }

        private double GetDifferenceInHours(DateTime createdTime)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan ts = currentTime.Subtract(createdTime);
            double differenceInDays = ts.TotalHours;

            return differenceInDays;
        }
    }
}
