using Manulife.DNC.MSAD.WS.Events;
using System;

namespace Manulife.DNC.MSAD.WS.Events
{
    public class OrderEventEntity : IOrderEventEntity
    {
        public int ID { get; set; }
        public string EventType { get; set; }
        public string OrderID { get; set; }
        public DateTime CreatedTime { get; set; }
        public string StatusKey { get; set; }
        public EventStatusEnum StatusValue { get; set; }
        public string EntityJson { get; set; }
    }
}
