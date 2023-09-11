using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using System.Text;
using static OrderAPI.OrderService;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApiSettings _settings;
        private readonly ILogger<AuthController> _logger;

        public AuthController(HttpClient client, IOptions<ApiSettings> settings, ILogger<AuthController> logger)
        {
            _client = client;
            _settings = settings.Value;
            _logger = logger;
        }
        private string ProductUrl => _settings.BaseUrl;
        [HttpPost]
        public async Task<Response<string>> Login(RequestDto request)
        {
            try
            {
                var url = $"{ProductUrl}/account/login";

                var payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);

                using var response = await _client.PostAsync(url, content);
                var str = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<Response<string>>(str);

                if (response.IsSuccessStatusCode) ContextHelper.Current.Session.SetString(_settings.AccessTokenName, res.Data);

                return new Response<string> { Data = res.Data, StatusCode = response.StatusCode, Success = true };

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Response<string> { Message = "An error occurred", StatusCode = HttpStatusCode.InternalServerError, Success = false };
            }
        }
    }
    public class RequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
