using Events;

namespace QCode.Application.Interfaces
{
    public interface IExtractAttemptManager
    {
        void HandleEvent(CreatePositionsReport @event);
        CreatePositionsReport? GetEvent();
        void HandleFailedAttemptEvent(PositionsReportBuildFailed @event);
        PositionsReportBuildFailed? GetFailedAttemptEvent();
    }
}
