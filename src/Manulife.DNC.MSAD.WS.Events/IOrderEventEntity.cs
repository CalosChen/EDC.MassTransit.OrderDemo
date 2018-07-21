using System;
using System.Collections.Generic;
using System.Text;

namespace Manulife.DNC.MSAD.WS.Events
{
    public interface IOrderEventEntity
    {
        int ID { get; set; }

        string EventType { get; set; }

        string OrderID { get; set; }

        DateTime CreatedTime { get; set; }

        string StatusKey { get; set; }

        EventStatusEnum StatusValue { get; set; }

        string EntityJson { get; set; }
    }
}
