using Manulife.DNC.MSAD.WS.Events;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manulife.DNC.MSAD.WS.EventService.Models
{
    [Table("Events")]
    public class Event
    {
        [Column("EventID")]
        public string ID { get; set; }

        [Column("EventType")]
        public string Type { get; set; }

        [Column("OrderID")]
        public string OrderID { get; set; }

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; }

        [Column("StatusKey")]
        public string StatusKey { get; set; }

        [Column("StatusValue")]
        public EventStatusEnum StatusValue { get; set; }

        [Column("EntityJson")]
        public string EntityJson { get; set; }
    }
}
