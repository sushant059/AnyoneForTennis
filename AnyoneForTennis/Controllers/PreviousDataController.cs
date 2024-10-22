using AnyoneForTennis.OldDatas.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnyoneForTennis.Controllers
{
    public partial class PreviousDataController : Controller
    {
        private readonly Hitdb1Context _hitdb1Context;

        public PreviousDataController(Hitdb1Context hitdb1Context)
        {
            _hitdb1Context = hitdb1Context;
        }

        public async Task<IActionResult> Members()
        {
            List<OldDatas.Models.Member> result = await _hitdb1Context.Members.ToListAsync();
            return View(result);
        }
        public async Task<IActionResult> Schedules()
        {
            List<OldDatas.Models.Schedule> result = await _hitdb1Context.Schedules.ToListAsync();
            return View(result);
        }

        public async Task<IActionResult> Coaches()
        {
            List<OldDatas.Models.Coach> result = await _hitdb1Context.Coaches.ToListAsync();
            return View(result);
        }
    }
}
