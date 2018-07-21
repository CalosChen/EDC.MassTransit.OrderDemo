using Manulife.DNC.MSAD.WS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.StorageService.Repositories
{
    public interface IStorageRepository
    {
        Task<bool> CreateStorage(IOrder order);
    }
}
