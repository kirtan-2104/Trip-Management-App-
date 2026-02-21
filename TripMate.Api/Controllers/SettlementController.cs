using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripMate.Api.Data;

namespace TripMate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettlementController : ControllerBase
{
    private readonly AppDbContext _db;

    public SettlementController(AppDbContext db) => _db = db;

    [HttpGet("trip/{tripId:int}")]
    public async Task<ActionResult<IEnumerable<object>>> GetSettlements(int tripId)
    {
        var members = await _db.TripMembers.Include(m => m.User).Where(m => m.TripId == tripId).ToListAsync();
        var expenses = await _db.Expenses.Include(e => e.Splits).Where(e => e.TripId == tripId).ToListAsync();

        var balances = new Dictionary<int, decimal>();
        foreach (var m in members)
            balances[m.UserId] = 0;

        foreach (var exp in expenses)
        {
            balances[exp.PaidByUserId] += exp.Amount;
            foreach (var split in exp.Splits)
                balances[split.UserId] -= split.ShareAmount;
        }

        var creditorList = balances.Where(b => b.Value > 0.01m).Select(b => (b.Key, b.Value)).OrderByDescending(x => x.Value).ToList();
        var debtorList = balances.Where(b => b.Value < -0.01m).Select(b => (b.Key, b.Value)).OrderBy(x => x.Value).ToList();
        var result = new List<object>();
        int ci = 0, di = 0;
        while (ci < creditorList.Count && di < debtorList.Count)
        {
            var (cid, credit) = creditorList[ci];
            var (did, debt) = debtorList[di];
            var amt = Math.Min(credit, Math.Abs(debt));
            if (amt < 0.01m) break;
            var payer = members.First(m => m.UserId == did).User!;
            var receiver = members.First(m => m.UserId == cid).User!;
            result.Add(new { PayerName = payer.Name, ReceiverName = receiver.Name, Amount = Math.Round(amt, 2) });
            creditorList[ci] = (cid, credit - amt);
            debtorList[di] = (did, debt + amt);
            if (creditorList[ci].Value < 0.01m) ci++;
            if (Math.Abs(debtorList[di].Value) < 0.01m) di++;
        }
        return Ok(result);
    }
}
