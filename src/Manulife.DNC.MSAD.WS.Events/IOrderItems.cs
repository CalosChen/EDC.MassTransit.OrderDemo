using System;
using System.Collections.Generic;
using System.Text;

namespace Manulife.DNC.MSAD.WS.Events
{
    public interface IOrderItems
    {
        string ID { get; set; }

        string ProductID { get; set; }

        decimal Price { get; set; }

        double Quantity { get; set; }
    }
}
