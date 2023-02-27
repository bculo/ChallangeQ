using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public class PositionsReportBuildFailed
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime StartOfTheDay { get; set; }
        public DateTime EndOfTheDay { get; set; }
        public int FileType { get; set; }
    }
}
