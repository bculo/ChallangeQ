using MediatR;
using Microsoft.AspNetCore.Mvc;
using QCode.Application.Features.Trades;
using QCode.Application.Interfaces;

namespace QCode.RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly IDateTime _time;
        private readonly IMediator _mediator;
        private readonly ILogger<TradeController> _logger;
        
        public TradeController(IDateTime time,
            IMediator mediator,
            ILogger<TradeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
            _time = time;
        }

        [HttpGet("GetTradePositions")]
        public async Task<IActionResult> GetTradePositionsAsync()
        {
            return Ok(await _mediator.Send(new GetTradePositions.Query()));
        }

        [HttpPost("CreateReportWithServerLocalTime")]
        public async Task<IActionResult> CreateReportWithServerLocalTime([FromBody] CreateTradePositionsReport.Command command)
        {
            var systemCurrentTimeInfo = _time.DayInfo;

            command.StartOfTheDay = systemCurrentTimeInfo.StartOfTheDay;
            command.EndOfTheDay = systemCurrentTimeInfo.EndOfTheDay;
            command.DateTime = systemCurrentTimeInfo.Current;

            await _mediator.Send(command);

            return NoContent();
        }
    }
}
