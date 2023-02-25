using QCode.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCode.Application.Interfaces
{
    public interface IReportCreator
    {
        Task CreateReport(ReportRequest request);
    }
}
