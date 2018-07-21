using Manulife.DNC.MSAD.WS.OrderService.Models;
using Manulife.DNC.MSAD.WS.OrderService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Manulife.DNC.MSAD.WS.OrderService.Controllers
{
    [Route("api/Order")]
    public class OrderController : Controller
    {
        public IOrderRepository OrderRepository { get; }

        public OrderController(IOrderRepository OrderRepository)
        {
            this.OrderRepository = OrderRepository;
        }

        [HttpPost]
        public string Post([FromBody]OrderDTO orderDTO)
        {
            var result = OrderRepository.CreateOrder(orderDTO).GetAwaiter().GetResult();

            return result ? "Post Success" : "Post Failed";
        }
    }
}