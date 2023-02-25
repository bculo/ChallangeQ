using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using QCode.Application.Interfaces;
using Services;

namespace QCode.Application.Features.Trades
{
    public static class GetTradePositions
    {
        public class Query : IRequest<Response> { }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IMapper _mapper;
            private readonly IDateTime _time;
            private readonly ILogger<Handler> _logger;
            private readonly IPowerService _powerService;

            public Handler(IDateTime time,
                IMapper mapper,
                ILogger<Handler> logger,
                IPowerService powerService)
            {
                _time = time;
                _mapper = mapper;
                _logger = logger;
                _powerService = powerService;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                _logger.LogTrace("Fetching powertrades from {0} service", nameof(IPowerService));

                DateTime dateTime = _time.DateTime;
                var response = await _powerService.GetTradesAsync(dateTime);

                _logger.LogTrace("Trades fetched from {0} service", nameof(IPowerService));

                return new Response
                {
                    ExecutedOn = dateTime,
                    TimeZone = _time.TimeZoneName,
                    ExecutedOnUtc = dateTime.ToUniversalTime(),
                    Trades = _mapper.Map<IEnumerable<Trade>>(response)
                };
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<PowerTrade, Trade>()
                    .ForMember(i => i.Periods, opt => opt.MapFrom(i => i.Periods));

                CreateMap<PowerPeriod, TradePeriod>()
                    .ForMember(i => i.Period, opt => opt.MapFrom(i => i.Period))
                    .ForMember(i => i.Volume, opt => opt.MapFrom(i => i.Volume));
            }
        }

        public class Response
        {
            public string? TimeZone { get; set; }
            public DateTime ExecutedOn { get; set; }
            public DateTime ExecutedOnUtc { get; set; }
            public IEnumerable<Trade>? Trades { get; set; }
        }

        public class Trade
        {
            public IEnumerable<TradePeriod>? Periods { get; set; }
        }

        public class TradePeriod
        {
            public int Period { get; set; }
            public double Volume { get; set; }
        }
    }
}
