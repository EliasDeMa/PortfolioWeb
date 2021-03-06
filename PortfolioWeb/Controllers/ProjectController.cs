﻿using System;
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

        private async Task<ProjectIndexPageViewModel> CreateIndexPageModel()
        {
            var statuses = await _portfolioDbContext.Statuses.ToListAsync();
            var tags = await _portfolioDbContext.Tags.ToListAsync();

            var insertStatuses = statuses.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.Description
            }).ToList();

            var insertTags = tags.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.Name
            }).ToList();


            insertStatuses.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "None"
            });

            insertTags.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "None"
            });

            return new ProjectIndexPageViewModel
            {
                Statuses = insertStatuses,
                Tags = insertTags,
                
            };
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var vm = await CreateIndexPageModel();

            var projects = await _portfolioDbContext.Projects
                .Include(x => x.Status)
                .Where(x => userId == x.PortfolioUserId)
                .ToListAsync();

            vm.Projects = projects.Select(item => new ProjectIndexViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Status = item.Status.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate
            });

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(ProjectIndexPageViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var projects = _portfolioDbContext.Projects
                .Include(x => x.Status)
                .Include(x => x.ProjectTags)
                .Where(x => userId == x.PortfolioUserId);

            if (vm.SelectedStatus != 0)
            {
                projects = projects.Where(item => item.StatusId == vm.SelectedStatus);
            }

            if (vm.SelectedTag != 0)
            {
                projects = projects.Where(item => item.ProjectTags.Where(pTags => pTags.TagId == vm.SelectedTag).Any());
            }

            var filteredProjects = await projects.ToListAsync();

            var newVm = await CreateIndexPageModel();
            newVm.Projects = filteredProjects.Select(item => new ProjectIndexViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Status = item.Status.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate
            });

            return View(newVm);
        }

        [Authorize]
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
                ProjectTags = project.ProjectTags.Select(item => item.Tag.Name),
                PhotoUrl = project.PhotoUrl
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
            if (!TryValidateModel(vm))
            {
                var statuses = await _portfolioDbContext.Statuses.ToListAsync();
                var tags = await _portfolioDbContext.Tags.ToListAsync();

                vm.Statuses = statuses.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Description
                });
                vm.Tags = tags.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });

                return View(vm);
            }

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

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var project = await _portfolioDbContext.Projects
                .Include(proj => proj.ProjectTags)
                .FirstOrDefaultAsync(item => item.Id == id && item.PortfolioUserId == userId);

            var statuses = await _portfolioDbContext.Statuses.ToListAsync();
            var tags = await _portfolioDbContext.Tags.ToListAsync();

            var vm = new ProjectEditViewModel
            {
                Name = project.Name,
                SelectedStatus = project.StatusId,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                PhotoUrl = project.PhotoUrl,
                Statuses = statuses.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Description
                }),
                Tags = tags.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name,
                    Selected = project.ProjectTags.Where(tag => tag.TagId == item.Id).Any()
                })
            };

            

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectEditViewModel vm)
        {
            if (!TryValidateModel(vm))
            {
                var statuses = await _portfolioDbContext.Statuses.ToListAsync();
                var tags = await _portfolioDbContext.Tags.ToListAsync();

                vm.Statuses = statuses.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Description
                });
                vm.Tags = tags.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                });

                return View(vm);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var project = await _portfolioDbContext.Projects
                .Include(proj => proj.ProjectTags)
                .FirstOrDefaultAsync(item => item.Id == id && item.PortfolioUserId == userId);

            _portfolioDbContext.ProjectTags.RemoveRange(project.ProjectTags);

            project.Name = vm.Name;
            project.Description = vm.Description;
            project.StartDate = vm.StartDate;
            project.EndDate = vm.EndDate;
            project.StatusId = vm.SelectedStatus;
            project.ProjectTags = vm.SelectedTags.Select(item => new ProjectTag
            {
                TagId = item
            }).ToList();

            if (vm.File != null)
            {
                if (!String.IsNullOrEmpty(project.PhotoUrl))
                    _photoService.DeletePhoto(project.PhotoUrl);

                project.PhotoUrl = _photoService.AddPhoto(vm.File);
            }

            await _portfolioDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", new { Id = id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var project = await _portfolioDbContext.Projects
                .Include(proj => proj.ProjectTags)
                .FirstOrDefaultAsync(item => item.Id == id && item.PortfolioUserId == userId);

            var vm = new ProjectDeleteViewModel
            {
                Id = id,
                Name = project.Name
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var project = await _portfolioDbContext.Projects
                .Include(proj => proj.ProjectTags)
                .FirstOrDefaultAsync(item => item.Id == id && item.PortfolioUserId == userId);

            if (!String.IsNullOrEmpty(project.PhotoUrl))
                _photoService.DeletePhoto(project.PhotoUrl);

            _portfolioDbContext.Projects.Remove(project);
            await _portfolioDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
