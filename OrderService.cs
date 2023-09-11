using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderAPI.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime;

namespace OrderAPI
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly HttpClient _client;
        private readonly ApiSettings _settings;
        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger, HttpClient client, IOptions<ApiSettings> settings)
        {
            _context= context;
            _logger= logger;
            _client= client;
            _settings = settings.Value;
        }
        private string ProductUrl => _settings.BaseUrl;
        public async Task<Response<List<Order>>> GetOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders.ToListAsync();
                return new Response<List<Order>> { Data= orders, Success = true, StatusCode = HttpStatusCode.OK };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Response<List<Order>> { Success = false, Message = "An error occurred", StatusCode = HttpStatusCode.InternalServerError };
            }
        }
        public async Task<Response<List<ProductDto>>> GetProductsAsync(string token)
        {
            try
            {
                var url = $"{ProductUrl}/product/getProducts";
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{token}");
                var response = await _client.GetAsync(url);
                var str = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<List<ProductDto>>(str);
                return new Response<List<ProductDto>> { Data= res, Success = response.IsSuccessStatusCode, StatusCode = response.StatusCode };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Response<List<ProductDto>> { Success = false, Message = "An error occurred", StatusCode = HttpStatusCode.InternalServerError };
            }
        }
        public async Task<Response<Order>> CreateOrderAsync(OrderDto orderDto, string token)
        {
            try
            {
                var url = $"{ProductUrl}/product/getProductById?Id={orderDto.ProductId}";
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{token}");
                var response = await _client.GetAsync(url);
                var str = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ProductDto>(str);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response<Order> { Success = response.IsSuccessStatusCode, StatusCode = response.StatusCode , Message = str};
                }
                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = orderDto.Name,
                    Description = orderDto.Description,
                    ProductId = res.Id,
                    ProductName = res.Name,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();
                return new Response<Order> { Success = true, Data = order, StatusCode= HttpStatusCode.Created };
            }
            catch (Exception e)
            {

                _logger.LogError(e.Message);
                return new Response<Order> { Success = false, Message = "An error occurred", StatusCode = HttpStatusCode.InternalServerError };
            }
        }
        public class Response<T>
        {
            public T Data { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            [JsonIgnore]
            public HttpStatusCode StatusCode { get; set; }
        }
    }
}
