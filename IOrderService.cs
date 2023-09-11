using static OrderAPI.OrderService;

namespace OrderAPI
{
    public interface IOrderService
    {
        Task<Response<List<Order>>> GetOrdersAsync();
        Task<Response<List<ProductDto>>> GetProductsAsync(string token);
        Task<Response<Order>> CreateOrderAsync(OrderDto orderDto, string token);
    }
}
