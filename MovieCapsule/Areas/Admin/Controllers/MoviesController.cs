﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieCapsule.Models;

namespace MovieCapsule.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Admin/Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Admin/Movies/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Genres"] = await _context.Genres.ToListAsync();
            return View();
        }

        // POST: Admin/Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,Rating")] Movie movie , int[] selectedGenres)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                await AssignGenresAsync(movie.Id, selectedGenres);
                return RedirectToAction(nameof(Index));
            }
            ViewData["SelectedGenres"] = selectedGenres;
            ViewData["Genres"] = await _context.Genres.ToListAsync();
            return View(movie);
        }

        private async Task AssignGenresAsync(int movieId, int[] selectedGenres)
        {
            Movie movie = await _context.Movies.FindAsync(movieId);
            await _context.Entry(movie).Collection(m => m.Genres).LoadAsync();
            movie.Genres.Clear();
            foreach (int genreId in selectedGenres)
            {
                movie.Genres.Add(await _context.Genres.FindAsync(genreId));
            }
            await _context.SaveChangesAsync();
        }

        // GET: Admin/Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            await _context.Entry(movie).Collection(m => m.Genres).LoadAsync();
            ViewData["Genres"] = await _context.Genres.ToListAsync();
            ViewData["SelectedGenres"] = movie.Genres.Select(g => g.Id).ToArray();
            return View(movie);
        }

        // POST: Admin/Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Rating")] Movie movie, int[] selectedGenres)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    await AssignGenresAsync(movie.Id, selectedGenres);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            ViewData["SelectedGenres"] = selectedGenres;
            ViewData["Genres"] = await _context.Genres.ToListAsync();
            return View(movie);
        }

        // GET: Admin/Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Admin/Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
