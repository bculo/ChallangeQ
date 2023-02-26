using QCode.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCode.BGWorker.Options
{
    public sealed class BGWorkerOptions
    {
        public FileType FileType { get; set; }
        public int IntervalTimeInMinutes { get; set; }
    }
}
