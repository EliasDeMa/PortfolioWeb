using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using PortfolioWeb.Data;
using PortfolioWeb.Domain;
using PortfolioWeb.Models;
using PortfolioWeb.Services;

namespace PortfolioWeb.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _portfolioDbContext;
        private readonly IPhotoService _photoService;

        public ProjectController(ApplicationDbContext applicationDbContext, IPhotoService photoService)
        {
            _portfolioDbContext = applicationDbContext;
            _photoService = photoService;
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

        public async Task<IActionResult> Detail(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var project = await _portfolioDbContext.Projects
                .Include(proj => proj.Status)
                .Include(proj => proj.ProjectTags)
                .ThenInclude(pTag => pTag.Tag)
                .FirstOrDefaultAsync(proj => proj.PortfolioUserId == userId && proj.Id == id);

            var vm = new ProjectDetailViewModel
            {
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                Status = project.Status.Description,
                ProjectTags = project.ProjectTags.Select(item => item.Tag.Name)
            };

            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var statuses = await _portfolioDbContext.Statuses.ToListAsync();
            var tags = await _portfolioDbContext.Tags.ToListAsync();

            var vm = new ProjectCreateViewModel { 
                Statuses = statuses.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Description
                }),
                Tags = tags.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                })
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var project = new Project
            {
                Name = vm.Name,
                Description = vm.Description,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                StatusId = vm.SelectedStatus,
                ProjectTags = vm.SelectedTags.Select(id => new ProjectTag { TagId = id }).ToList(),
                PortfolioUserId = userId
            };

            if (vm.File != null)
            {
                project.PhotoUrl = _photoService.AddPhoto(vm.File);
            }

            await _portfolioDbContext.AddAsync(project);
            await _portfolioDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
