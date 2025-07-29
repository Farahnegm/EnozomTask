using EnozomTask.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace EnozomTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClockifyController : ControllerBase
    {
        private readonly IClockifyService _clockifyService;
        public ClockifyController(IClockifyService clockifyService)
        {
            _clockifyService = clockifyService;
        }

        [HttpPost("push-sample")]
        public async Task<IActionResult> PushSample()
        {
            await _clockifyService.PushSampleDataAsync();
            return Ok(new { message = "Sample data pushed to Clockify and saved locally." });
        }
    }

   
} 