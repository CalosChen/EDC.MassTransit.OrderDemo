using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.EventService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.EventService.Repositories
{
    public interface IEventRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetEvents(string eventType);

        Task<bool> UpdateEventStatus(string eventType, T orderEvent);
    }
}
