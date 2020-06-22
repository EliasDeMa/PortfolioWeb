using PortfolioWeb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioWeb.Models
{
    public class ProjectDetailViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<string> ProjectTags { get; set; }
        public string Status { get; set; }
    }
}
