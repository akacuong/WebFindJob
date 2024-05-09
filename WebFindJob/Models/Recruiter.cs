using Microsoft.AspNetCore.Identity;

namespace WebFindJob.Models
{
    public class Recruiter
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public virtual IdentityUser? User { get; set; }
        public ICollection<JobListing>? JobListings { get; set; }
    }
}
