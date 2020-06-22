using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioWeb.Models
{
    public class ProjectIndexPageViewModel
    {
        public IEnumerable<ProjectIndexViewModel> Projects { get; set; }

        public int? SelectedStatus { get; set; }
        public int? SelectedTag { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }
        public IEnumerable<SelectListItem> Tags { get; set; }
    }
}
