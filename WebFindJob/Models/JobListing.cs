using Microsoft.AspNetCore.Builder;

namespace WebFindJob.Models
{
    public class JobListing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public DateTime PostedDate { get; set; }
        public string? RecruiterId { get; set; }
        public virtual Recruiter? Recruiter { get; set; }
        public ICollection<JobApplication>? JobApplications { get; set; }
    }
}
