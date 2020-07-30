using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using Films.Models;

namespace Films.Controllers
{
    [Authorize]
    public class FilmsController : Controller
    {
        private readonly FilmContext _context;
        private readonly int pageSize = 5;
        private readonly string savePath = @"\posters\";
        private IHostingEnvironment _appEnvironment { get; }

        public FilmsController(FilmContext context,IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _appEnvironment = hostingEnvironment;
        }

        // GET: Films
        public async Task<IActionResult> Index(int page = 1)
        {
            var filmContext = _context.Films.Include(f => f.Owner);
            var count = await filmContext.CountAsync();
            var items = await filmContext.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Films = items
            };
            return View(viewModel);
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Film film,IFormFile poster)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            FileSuccess result = null;
            if (poster != null)
                result = await SaveFile(poster, film);
            else
            {
                film.PosterFileNameOnDisk = @"\Files\empty.jpg";
                film.PosterFileNameOriginally = null;
            }
            if(result!=null && !result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(film);
            }
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", film.OwnerId);
            return View(film);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", film.OwnerId);
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Film film,IFormFile poster)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    FileSuccess result = null;
                    if (poster != null)
                        result = await SaveFile(poster, film);
                    if (result != null && !result.Success)
                    {
                        ModelState.AddModelError("", result.Message);
                        return View(film);
                    }
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", film.OwnerId);
            return View(film);
        }

        private async Task<FileSuccess> SaveFile(IFormFile poster, Film film)
        {
            FileSuccess result = new FileSuccess();
            if (poster != null)
            {
                if (poster.Length > 20000000)
                {
                    result.Message = "файл слишком большой";
                    result.Success = false;
                    return result;
                }
                if (poster.ContentType.ToLower() != "image/jpg" &&
                       poster.ContentType.ToLower() != "image/jpeg" &&
                       poster.ContentType.ToLower() != "image/pjpeg" &&
                       poster.ContentType.ToLower() != "image/gif" &&
                       poster.ContentType.ToLower() != "image/x-png" &&
                       poster.ContentType.ToLower() != "image/png")
                {
                    result.Message ="Неверное расширение файла";
                    result.Success = false;
                    return result;
                }
                string halfPath = _appEnvironment.WebRootPath + savePath;
                if (!Directory.Exists(halfPath))
                    Directory.CreateDirectory(halfPath);
                var fileNameOnDisk = savePath + Guid.NewGuid().ToString() + Path.GetExtension(poster.FileName);

                var fullPath = _appEnvironment.WebRootPath + fileNameOnDisk;
                using (FileStream savedFile = new FileStream(fullPath, FileMode.Create))
                {
                    await poster.CopyToAsync(savedFile);
                }
                film.PosterFileNameOnDisk = fileNameOnDisk;
                film.PosterFileNameOriginally = poster.FileName;
                result.Message = "";
                result.Success = true;
                return result;
            }
            result.Message = "файл отсутствует";
            result.Success = false;
            return result;
        }
        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var film = await _context.Films.FindAsync(id);
            _context.Films.Remove(film);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(long id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
    }
    public class FileSuccess {
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
