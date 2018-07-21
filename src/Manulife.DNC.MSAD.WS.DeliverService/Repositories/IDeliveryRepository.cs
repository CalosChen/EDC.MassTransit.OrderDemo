using Manulife.DNC.MSAD.WS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.DeliveryService.Repositories
{
    public interface IDeliveryRepository
    {
        Task<bool> CreateDelivery(IOrder order);
    }
}
