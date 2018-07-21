using Manulife.DNC.MSAD.WS.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manulife.DNC.MSAD.WS.EventService.Models
{
    [Table("Orders")]
    public class Order : IOrder
    {
        [Column("OrderID")]
        public string ID { get; set; }

        [Column("OrderTime")]
        public DateTime OrderTime { get; set; }

        [Column("OrderUserID")]
        public string OrderUserID { get; set; }

        [NotMapped]
        public List<IOrderItems> OrderItems { get; set; }

        [NotMapped]
        public string StatusKey { get; set; }
    }
}
