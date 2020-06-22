using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioWeb.Models
{
    public class ProjectEditViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PhotoUrl { get; set; }

        public int SelectedStatus { get; set; }
        public int[] SelectedTags { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }
        public IEnumerable<SelectListItem> Tags { get; set; }
        public IFormFile File { get; set; }
    }
}
