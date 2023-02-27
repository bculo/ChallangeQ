using MediatR;
using QCode.Application.Common.Enums;
using QCode.Application.Interfaces;

namespace QCode.Application.Features.Trades
{
    public static class StartReportBuildProcess
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public DateTime DateTime { get; set; }
            public DateTime StartOfTheDay { get; set; }
            public DateTime EndOfTheDay { get; set; }
            public FileType Type { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IExtractAttemptManager _manager;

            public Handler(IExtractAttemptManager manager)
            {
                _manager = manager;
            }

            public Task Handle(Command request, CancellationToken cancellationToken)
            {
                _manager.HandleEvent(new Events.CreatePositionsReport
                {
                    DateTime = request.DateTime,
                    EndOfTheDay = request.EndOfTheDay,
                    FileType = (int)request.Type,
                    Id = request.Id,
                    StartOfTheDay = request.StartOfTheDay
                });

                return Task.CompletedTask;
            }
        }
    }
}
