using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCode.Application.Common.Models
{
    public class ReportRequest
    {
        public string? FileName { get; set; }
        public string? SaveToLocation { get; set; }
        public ReportRow? Header { get; set; }
        public List<ReportRow>? Body { get; set; }
    }

    public class ReportRow
    {
        public List<ReportRowItem>? Items { get; set; }
    }

    public class ReportRowItem
    {
        public int Width { get; set; }
        public string? Text { get; set; }
        public ReportRowItemAlignment Alignment { get; set; }
    }

    public enum ReportRowItemAlignment
    {
        LEFT = 0,
        RIGHT = 1,
    }

    public class ReportResponse
    {
        public string? Base64Content { get; set; }
        public string? ContentType { get; set; }
        public string? Extension { get; set; }
        public string? Name { get; set; }
    }
}
