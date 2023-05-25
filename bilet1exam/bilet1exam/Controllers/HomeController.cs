using bilet1exam.DAL;
using bilet1exam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace bilet1exam.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _Context;

        public HomeController(AppDbContext context)
        {
            _Context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            return View(await _Context.Posts.Where(p => !p.IsDeleted).Take(3).OrderByDescending(o=>o.Id).ToListAsync());
        }

       
    }
}