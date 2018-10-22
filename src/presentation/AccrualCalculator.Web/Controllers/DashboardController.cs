using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppName.Web.Extensions;
using AppName.Web.Models;
using AppName.Web.Providers;
using AppName.Web.Repositories;
using AppName.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppName.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(
            IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";

            string userId = HttpContext.User.UserId();
            var accruals = (
                await _dashboardRepository.GetAllAccrualsForUser(userId));

            return View(new IndexDashboardViewModel {UserAccruals = accruals});
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Create New";

            return View();
        }

        [Route("/dashboard/view/{id}")]
        public IActionResult View(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound("Accrual does not exist.");
            }

            return View("View", id);
        }
    }
}