using System;
using System.Collections.Generic;

namespace DoganConsult.Organization.Organizations;

public class OrganizationStatisticsDto
{
    public long TotalOrganizations { get; set; }
    public long ActiveOrganizations { get; set; }
    public long InactiveOrganizations { get; set; }
    public Dictionary<string, int> ByType { get; set; } = new();
    public Dictionary<string, int> BySector { get; set; } = new();
    public Dictionary<string, int> ByCountry { get; set; } = new();
    public List<TrendDataPoint> Trends { get; set; } = new();
}

public class TrendDataPoint
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime Date { get; set; }
}
