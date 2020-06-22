using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioWeb.Data;
using PortfolioWeb.Models;

namespace PortfolioWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _portfolioDbContext;

        public UserController(ApplicationDbContext portfolioDbContext)
        {
            _portfolioDbContext = portfolioDbContext;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userDb = await _portfolioDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            var user = new UserDetailViewModel
            {
                Name = userDb.UserName,
                Description = userDb.Description,
            };

            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> EditInfo()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userDb = await _portfolioDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            var userInfo = new UserEditInfoViewModel
            {
                Info = userDb.Description,
            };

            return View(userInfo);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditInfo(UserEditInfoViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userDb = await _portfolioDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            userDb.Description = vm.Info;

            await _portfolioDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
