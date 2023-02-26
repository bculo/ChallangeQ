using QCode.Application.Common.Enums;

namespace QCode.Application.Common.Options
{
    public sealed class BGWorkerOptions
    {
        public FileType FileType { get; set; }
        public int IntervalTimeInMinutes { get; set; }
    }
}
