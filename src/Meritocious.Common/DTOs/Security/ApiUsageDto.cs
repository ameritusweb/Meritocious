namespace Meritocious.Common.DTOs.Security;

public class ApiUsageDto
{
    public string EndpointPath { get; set; }
    public string HttpMethod { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public double AverageResponseTime { get; set; }
    public double ErrorRate { get; set; }
    public Dictionary<string, int> ResponseStatusCodes { get; set; }
    public Dictionary<string, int> ErrorTypes { get; set; }
    public DateTime TimeStamp { get; set; }
    public string ClientId { get; set; }
}