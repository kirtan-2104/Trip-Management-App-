using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripMate.Api.Data;
using TripMate.Api.DTOs;
using TripMate.Api.Models;

namespace TripMate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExpensesController(AppDbContext db) => _db = db;

    [HttpGet("trip/{tripId:int}")]
    public async Task<ActionResult<IEnumerable<Expense>>> GetByTrip(int tripId, [FromQuery] int? paidByUserId = null)
    {
        var query = _db.Expenses.Include(e => e.PaidByUser).Include(e => e.Splits).ThenInclude(s => s.User)
            .Where(e => e.TripId == tripId);
        if (paidByUserId.HasValue)
            query = query.Where(e => e.PaidByUserId == paidByUserId.Value);
        return Ok(await query.OrderByDescending(e => e.CreatedDate).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Expense>> GetById(int id)
    {
        var exp = await _db.Expenses.Include(e => e.PaidByUser).Include(e => e.Splits).ThenInclude(s => s.User)
            .FirstOrDefaultAsync(e => e.Id == id);
        return exp == null ? NotFound() : Ok(exp);
    }

    [HttpPost]
    public async Task<ActionResult<Expense>> Create(CreateExpenseRequest req)
    {
        var members = await _db.TripMembers.Where(m => m.TripId == req.TripId).Select(m => m.UserId).ToListAsync();
        var expense = new Expense
        {
            TripId = req.TripId,
            Title = req.Title,
            Amount = req.Amount,
            PaidByUserId = req.PaidByUserId,
            SplitType = req.SplitType,
            Note = req.Note,
            CreatedDate = DateTime.UtcNow
        };
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        if (req.SplitType == 1 && req.CustomSplits != null && req.CustomSplits.Count > 0)
        {
            foreach (var kv in req.CustomSplits)
                _db.ExpenseSplits.Add(new ExpenseSplit { ExpenseId = expense.Id, UserId = kv.Key, ShareAmount = kv.Value });
        }
        else
        {
            var share = Math.Round(req.Amount / members.Count, 2);
            var rem = req.Amount - share * members.Count;
            for (int i = 0; i < members.Count; i++)
                _db.ExpenseSplits.Add(new ExpenseSplit { ExpenseId = expense.Id, UserId = members[i], ShareAmount = share + (i == 0 ? rem : 0) });
        }
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseRequest req)
    {
        var exp = await _db.Expenses.FindAsync(id);
        if (exp == null) return NotFound();
        exp.Title = req.Title;
        exp.Amount = req.Amount;
        exp.PaidByUserId = req.PaidByUserId;
        exp.SplitType = req.SplitType;
        exp.Note = req.Note;

        var existingSplits = await _db.ExpenseSplits.Where(s => s.ExpenseId == id).ToListAsync();
        _db.ExpenseSplits.RemoveRange(existingSplits);

        if (req.SplitType == 1 && req.CustomSplits != null && req.CustomSplits.Count > 0)
        {
            foreach (var kv in req.CustomSplits)
                _db.ExpenseSplits.Add(new Models.ExpenseSplit { ExpenseId = id, UserId = kv.Key, ShareAmount = kv.Value });
        }
        else
        {
            var members = await _db.TripMembers.Where(m => m.TripId == exp.TripId).Select(m => m.UserId).ToListAsync();
            var share = Math.Round(exp.Amount / members.Count, 2);
            var rem = exp.Amount - share * members.Count;
            for (int i = 0; i < members.Count; i++)
                _db.ExpenseSplits.Add(new Models.ExpenseSplit { ExpenseId = id, UserId = members[i], ShareAmount = share + (i == 0 ? rem : 0) });
        }
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var exp = await _db.Expenses.FindAsync(id);
        if (exp == null) return NotFound();
        _db.Expenses.Remove(exp);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
