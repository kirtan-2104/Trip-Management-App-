using TripMate.Data;

namespace TripMate.Services;

/// <summary>
/// Settlement calculation using simplified debt resolution algorithm.
/// Computes minimal transactions to settle all balances.
/// </summary>
public class SettlementService : ISettlementService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ITripRepository _tripRepository;

    public SettlementService(IExpenseRepository expenseRepository, ITripRepository tripRepository)
    {
        _expenseRepository = expenseRepository;
        _tripRepository = tripRepository;
    }

    public async Task<IReadOnlyList<SettlementItem>> GetSettlementsAsync(int tripId, CancellationToken ct = default)
    {
        var members = await _tripRepository.GetMembersAsync(tripId, ct);
        var expenses = await _expenseRepository.GetByTripIdAsync(tripId, null, ct);

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

        var settlements = new List<SettlementItem>();
        var creditorIdx = 0;
        var debtorIdx = 0;

        while (creditorIdx < creditorList.Count && debtorIdx < debtorList.Count)
        {
            var (creditorId, credit) = creditorList[creditorIdx];
            var (debtorId, debt) = debtorList[debtorIdx];
            var amount = Math.Min(credit, Math.Abs(debt));
            if (amount < 0.01m) break;

            var creditorMember = members.First(m => m.UserId == creditorId);
            var debtorMember = members.First(m => m.UserId == debtorId);

            settlements.Add(new SettlementItem(debtorMember.User.Name, creditorMember.User.Name, Math.Round(amount, 2)));

            creditorList[creditorIdx] = (creditorId, credit - amount);
            debtorList[debtorIdx] = (debtorId, debt + amount);

            if (creditorList[creditorIdx].Value < 0.01m) creditorIdx++;
            if (Math.Abs(debtorList[debtorIdx].Value) < 0.01m) debtorIdx++;
        }

        return settlements;
    }
}
