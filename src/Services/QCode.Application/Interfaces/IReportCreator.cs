using QCode.Application.Common.Models;

namespace QCode.Application.Interfaces
{
    public interface IReportCreator
    {
        Task<string> CreateReport(ReportRequest request);
    }
}
