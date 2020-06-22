using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using PortfolioWeb.Data;
using PortfolioWeb.Models;

namespace PortfolioWeb.Controllers
{
    public class ProjectController : Controller
    {
        ApplicationDbContext _portfolioDbContext;

        public ProjectController(ApplicationDbContext applicationDbContext)
        {
            _portfolioDbContext = applicationDbContext;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var projects = await _portfolioDbContext.Projects
                .Include(x => x.Status)
                .Where(x => userId == x.PortfolioUserId)
                .ToListAsync();

            return View(projects.Select(item => new ProjectIndexViewModel 
            { 
                Id = item.Id,
                Name = item.Name,
                Status = item.Status.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate
            }));
        }
    }
}
