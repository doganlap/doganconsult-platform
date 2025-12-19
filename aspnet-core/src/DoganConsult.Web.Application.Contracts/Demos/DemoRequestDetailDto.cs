using System;
using System.Collections.Generic;

namespace DoganConsult.Web.Demos
{
    public class DemoRequestDetailDto : DemoRequestDto
    {
        public List<DemoActivityDto> Activities { get; set; } = new();
    }

    public class DemoActivityDto
    {
        public int Id { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
