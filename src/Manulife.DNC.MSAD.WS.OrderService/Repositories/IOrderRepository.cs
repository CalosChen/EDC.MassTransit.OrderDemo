using Manulife.DNC.MSAD.WS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.OrderService.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> CreateOrder(IOrder order);
    }
}
