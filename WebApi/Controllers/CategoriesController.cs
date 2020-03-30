using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoriesController(DataContext context)
        {
            _context = context;
        }

        // GET: Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _context.Categories.Where(c => c.UserId == Convert.ToInt32(User.Identity.Name)).Select(d => new { d.Id, d.Name }).ToListAsync());
        }

        // GET: Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            else if (Convert.ToInt32(User.Identity.Name) != category.UserId)
            {
                return Unauthorized();
            }
            return Ok(new { category.Id, category.Name });
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            category.UserId = Convert.ToInt32(User.Identity.Name);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category.Id);
        }

        // PUT: Categories/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            category.UserId = Convert.ToInt32(User.Identity.Name);
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: Categories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            else if (Convert.ToInt32(User.Identity.Name) != category.UserId)
            {
                return Unauthorized();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { category.Id, category.Name });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
