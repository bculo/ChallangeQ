using QCode.Application.Features.Trades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCode.Application.Interfaces
{
    public interface IExtractAttemptStorage
    {
        void AddAttempt(CreatePositionsReport.Command command);
        void RemoveAttempt(CreatePositionsReport.Command command);
        List<CreatePositionsReport.Command> GetFailedAttempts(int maxNumberOfAttempts);
    }
}
