using HostelFinder.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WebhookController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost("HandlePaymentWebhook")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] WebhookType webhookType)
        {
            try
            {
                long orderCode = webhookType.data.orderCode;
                var response = await _walletService.CheckTransactionStatusAsync(orderCode);
                Console.WriteLine("11111");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing webhook: {ex.Message}");
            }
        }
    }
}
