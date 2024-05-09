namespace WebFindJob.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public DateTime AppliedDate { get; set; }
        public string CoverLetter { get; set; }
        public string? CvFilePath { get; set; }
        public int JobListingId { get; set; }
        public virtual Seeker? Seeker { get; set; }
        public string? SeekerId { get; set; }
        public virtual JobListing? JobListing { get; set; }
    }
}
