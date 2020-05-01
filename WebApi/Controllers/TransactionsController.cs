using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Transactions;
using AutoMapper;
using System.Collections;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly DataContext _context;
        private IMapper _mapper;

        public TransactionsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.Where(t => t.UserId == Convert.ToInt32(User.Identity.Name)).ToListAsync();
        }

        // GET: Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }
            else if (Convert.ToInt32(User.Identity.Name) != transaction.UserId)
            {
                return Unauthorized();
            }

            return transaction;
        }

        [HttpGet("revenues")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetRevenues()
        {
            return await _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount > 0)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        [HttpGet("expenses")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetExpenses()
        {
            return await _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount < 0)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetPendingTransactions()
        {
            return await _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Finished == false)
                .OrderBy(t => t.Date)
                .ToListAsync();
        }

        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetLatestTransactions()
        {
            return await _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Finished == true)
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToListAsync();
        }

        [HttpGet("expenses/categorized")]
        public async Task<ActionResult<IEnumerable>> GetCategorizedExpenses()
        {
            return await _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount < 0 && t.Finished == true)
                .GroupBy(t => new { t.CategoryId, t.Category.CategoryName })
                .Select(g => new { categoryName = g.Key.CategoryName, amount = g.Sum(i => Math.Abs(i.Amount))})
                .ToListAsync();
        }

        [HttpGet("revenues/monthly")]
        public async Task<ActionResult<IEnumerable>> GetMonthlyRevenues([FromQuery] int limit=0)
        {

            var transactions = _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount > 0 && t.Finished == true)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new { date = g.Key.ToString(), amount = g.Sum(h => h.Amount) });

            if (limit != 0)
            {
                return transactions.AsEnumerable().TakeLast(limit).ToList();
            } else
            {
                return await transactions.ToListAsync();
            }
        }

        [HttpGet("revenues/yearly")]
        public async Task<ActionResult<IEnumerable>> GetYearlyRevenues([FromQuery] int limit = 0)
        {

            var transactions = _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount > 0 && t.Finished == true)
                .GroupBy(t => new { t.Date.Year })
                .OrderBy(g => g.Key.Year)
                .Select(g => new { date = g.Key.ToString(), amount = g.Sum(h => h.Amount) });

            if (limit != 0)
            {
                return transactions.AsEnumerable().TakeLast(limit).ToList();
            }
            else
            {
                return await transactions.ToListAsync();
            }
        }

        [HttpGet("expenses/monthly")]
        public async Task<ActionResult<IEnumerable>> GetMonthlyExpenses([FromQuery] int limit = 0)
        {

            var transactions = _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount < 0 && t.Finished == true)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new { date = g.Key.ToString(), amount = g.Sum(h => Math.Abs(h.Amount)) });

            if (limit != 0)
            {
                return transactions.AsEnumerable().TakeLast(limit).ToList();
            }
            else
            {
                return await transactions.ToListAsync();
            }
        }

        [HttpGet("expenses/yearly")]
        public async Task<ActionResult<IEnumerable>> GetYearlyExpenses([FromQuery] int limit = 0)
        {

            var transactions = _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount < 0 && t.Finished == true)
                .GroupBy(t => new { t.Date.Year })
                .OrderBy(g => g.Key.Year)
                .Select(g => new { date = g.Key.ToString(), amount = g.Sum(h => Math.Abs(h.Amount)) });

            if (limit != 0)
            {
                return transactions.AsEnumerable().TakeLast(limit).ToList();
            }
            else
            {
                return await transactions.ToListAsync();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, UpdateModel transactionParam)
        {
            if (id != transactionParam.Id)
            {
                return BadRequest();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if(transaction == null)
            {
                return NotFound();
            }
            else if (Convert.ToInt32(User.Identity.Name) != transaction.UserId)
            {
                return Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(transactionParam.Name))
                transaction.Name = transactionParam.Name;

            transaction.Description = transactionParam.Description;
            transaction.CategoryId = transactionParam.CategoryId;

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Transaction>> AddTransaction(CreateModel model)
        {
            var transaction = _mapper.Map<Transaction>(model);
            transaction.UserId = Convert.ToInt32(User.Identity.Name);
            _context.Transactions.Add(transaction);
            
            if(transaction.Date.CompareTo(DateTime.Now) <= 0)
            {
                transaction.User.Balance += transaction.Amount;
                transaction.Finished = true;
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        // DELETE: Transactions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Transaction>> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }
            else if (Convert.ToInt32(User.Identity.Name) != transaction.UserId)
            {
                return Unauthorized();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
