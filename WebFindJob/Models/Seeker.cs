using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace WebFindJob.Models
{
    public class Seeker
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public virtual IdentityUser? User { get; set; }
        public ICollection<JobApplication>? JobApplications { get; set; }
    }
}
