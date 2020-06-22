using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortfolioWeb.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioWeb.Models
{
    public class ProjectCreateViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Project has to have a name")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Projects needs a starting date")]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int SelectedStatus { get; set; }
        public int[] SelectedTags { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }
        public IEnumerable<SelectListItem> Tags { get; set; }
        public IFormFile File { get; set; }
    }
}
