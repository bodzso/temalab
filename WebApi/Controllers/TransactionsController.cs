using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;

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
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
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

            return transaction;
        }

        [HttpGet("user-transactions/{id}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserTransactions(int id)
        {
            return await _context.Transactions.Where(t => t.UserId == id).ToListAsync();
        }

        [HttpGet("user-revenues/{id}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserRevenues(int id)
        {
            return await _context.Transactions.Where(t => t.UserId == id && t.Amount > 0).ToListAsync();
        }

        [HttpGet("user-expenses/{id}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetExpenses(int id)
        {
            return await _context.Transactions.Where(t => t.UserId == id && t.Amount < 0).ToListAsync();
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetPendingTransactions()
        {
            return await _context.Transactions.Where(t => t.Date.CompareTo(DateTime.Now) > 0).ToListAsync();
        }

        [HttpGet("pending/{id}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetPendingTransactionsByUser(int id)
        {
            return await _context.Transactions.Where(t => t.UserId == id && t.Date.CompareTo(DateTime.Now) > 0).ToListAsync();
        }

        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetLatestTransactions()
        {
            return await _context.Transactions.Where(t => t.Date.CompareTo(DateTime.Now) < 0).OrderBy(t => t.Date).Take(10).ToListAsync();
        }

        [HttpGet("latest/{id}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetLatestTransactionsByUser(int id)
        {
            return await _context.Transactions.Where(t => t.UserId == id && t.Date.CompareTo(DateTime.Now) < 0).OrderBy(t => t.Date).Take(10).ToListAsync();
        }

        // PUT: Transactions/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

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
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
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
