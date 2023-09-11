using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts([Required][FromHeader] string token)
        {

            var result = await _orderService.GetProductsAsync(token);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {

            var result = await _orderService.GetOrdersAsync();

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderDto orderDto, [Required][FromHeader] string token)
        {

            var result = await _orderService.CreateOrderAsync(orderDto, token);

            return StatusCode((int)result.StatusCode, result);
        }

    }
}
