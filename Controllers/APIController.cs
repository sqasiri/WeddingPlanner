using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.FileProviders;





namespace WeddingPlanner.Controllers
{
    // [Produces("application/json")]
    public class APIController : Controller
    {
        private MyContext _context;
        // private IHostingEnvironment _env;

        // public APIController(MyContext context, IHostingEnvironment env)
        // {
        //     _context = context;
        //     _env = env;
        // }

        private readonly IWebHostEnvironment _env;

        public APIController(IWebHostEnvironment env)
        {	
            _env = env;
        } 


        [HttpGet("upload/images")]
        public IActionResult TestAPI()
        {
            // return (_context.Users.SingleOrDefault(u => u.FirstName == "Abdul"));
            return View();
            
        }

        [HttpPost("upload/image")]
        public IActionResult SingleImage(IFormFile image){
            var dir = _env.WebRootPath;

            using (var fileStream = new FileStream(Path.Combine(dir+"/images", Guid.NewGuid().ToString()+".jpg"),FileMode.Create, FileAccess.Write)){
                image.CopyTo(fileStream);
            }

            return RedirectToAction("DisplayImages");
        }

        [BindProperty]
        public List<string> ImageList {get;set;}

        [HttpGet("display/images")]
        public IActionResult DisplayImages()
        {
            var dir = _env.WebRootPath;

            var provider = new PhysicalFileProvider(_env.WebRootPath);
            var contents = provider.GetDirectoryContents(Path.Combine("images"));
            var objFiles = contents.OrderBy(m => m.LastModified);

            ImageList = new List<string>();
            foreach(var item in objFiles.ToList())
            {
                ImageList.Add(item.Name);
            }

            // return new JsonResult(objFiles);
            ViewBag.ImageList = ImageList;
            return View();

        }

        
    }
}