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
            return Ok(await _mediator.Send(new GetPositions.Query()));
        }

        [HttpPost("GetPositionsReport")]
        public async Task<IActionResult> GetPositionsReport([FromBody] GetPositionsReport.Query query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}
