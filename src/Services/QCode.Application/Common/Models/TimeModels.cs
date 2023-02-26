using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCode.Application.Common.Models
{
    public class MomentOfTheDayInfo
    {
        public DateTime Current { get; private set; }
        public DateTime StartOfTheDay { get; private set; }
        public DateTime EndOfTheDay { get; private set; }

        public MomentOfTheDayInfo(DateTime current, DateTime starts, DateTime ends)
        {
            Current = current;
            StartOfTheDay = starts;
            EndOfTheDay = ends;
        }
    }
}
