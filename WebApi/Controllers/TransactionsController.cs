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

namespace WebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly DataContext _context;

        public TransactionsController(DataContext context)
        {
            _context = context;
        }

        // GET: Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> GetTransactions()
        {
            return Ok(await _context.Transactions.Where(t => t.UserId == Convert.ToInt32(User.Identity.Name)).Select(d => MapTransaction(d, d.Category.Name)).ToListAsync());
        }

        // GET: Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionModel>> GetTransaction(int id)
        {
            var transactions = _context.Transactions.Include("Category");
            var transaction = await transactions.SingleAsync(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            else if (Convert.ToInt32(User.Identity.Name) != transaction.UserId)
            {
                return Unauthorized();
            }

            return Ok(MapTransaction(transaction, transaction.Category?.Name));
        }

        [HttpGet("revenues")]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> GetRevenues()
        {
            return await _context.Transactions.Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount > 0).Select(d => MapTransaction(d, d.Category.Name)).ToListAsync();
        }

        [HttpGet("expenses")]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> GetExpenses()
        {
            return await _context.Transactions.Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Amount < 0).Select(d => MapTransaction(d, d.Category.Name)).ToListAsync();
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> GetPendingTransactions()
        {
            return await _context.Transactions.Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Date.CompareTo(DateTime.Now) > 0).Select(d => MapTransaction(d, d.Category.Name)).ToListAsync();
        }

        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> GetLatestTransactions()
        {
            return await _context.Transactions.Where(t => t.UserId == Convert.ToInt32(User.Identity.Name) && t.Date.CompareTo(DateTime.Now) < 0).OrderByDescending(t => t.Date).Take(10).Select(d => MapTransaction(d, d.Category.Name)).ToListAsync();
        }

        // PUT: Transactions/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            transaction.UserId = Convert.ToInt32(User.Identity.Name);
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

        // POST: Transactions
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Transaction>> AddTransaction(Transaction transaction)
        {
            transaction.UserId = Convert.ToInt32(User.Identity.Name);
            _context.Transactions.Add(transaction);
            var user = _context.Users.Find(transaction.UserId);
            user.Balance += transaction.Amount;
            await _context.SaveChangesAsync();
            return Ok(transaction.Id);
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

            return Ok(transaction.Id);
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }

        private static TransactionModel MapTransaction(Transaction transaction, string categoryname)
        {
            return new TransactionModel()
            {
                Id = transaction.Id,
                Name = transaction.Name,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description,
                CategoryId = transaction.CategoryId,
                CategoryName = categoryname
            };
        }
    }
}
