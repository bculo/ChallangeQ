namespace Events
{
    public class CreatePositionsReport
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime StartOfTheDay { get; set; }
        public DateTime EndOfTheDay { get; set; }
        public int FileType { get; set; }
    }
}
