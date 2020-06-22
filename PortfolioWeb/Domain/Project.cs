using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioWeb.Domain
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<ProjectTags> ProjectTags { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int PortfolioUserId { get; set; }
        public PortfolioUser PortfolioUser { get; set; }
    }
}
