using MediatR;

namespace QCode.Application.Common.Events
{
    public class ReportCreated : INotification
    {
        public string? FullPath { get; set; }
    }
}
