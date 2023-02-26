namespace Events
{
    public class ReportCreated : IntegrationEvent
    {
        public string? FilePath { get; set; }
    }
}
