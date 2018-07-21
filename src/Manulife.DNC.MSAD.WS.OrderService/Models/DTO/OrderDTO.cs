using Manulife.DNC.MSAD.WS.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.OrderService.Models
{
    public class OrderDTO : IOrder
    {
        public string ID { get; set; }

        public DateTime OrderTime { get; set; }

        public List<IOrderItems> OrderItems { get; set; }

        public string OrderUserID { get; set; }

        public string StatusKey { get; set; }
    }
}
