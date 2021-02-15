using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModulePlayer.DataAccess;
using ModulePlayer.Models;
using Newtonsoft.Json;

namespace ModulePlayer.Controllers
{
    [ApiController]
    public class TinCanController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public TinCanController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        
        [HttpGet("tincan/activities/state")]
        public async Task<IActionResult> TinCan([FromQuery] string stateId, [FromQuery] string activityId, [FromQuery] Guid registration)
        {
            var module = await _ctx.Modules.Include(m => m.TrackingData).FirstOrDefaultAsync(m => m.Id == registration);
            if (stateId == "bookmark")
            {
                return Ok(module.TrackingData.Location);
            }

            if (stateId == "suspend_data")
            {
                return Ok(module.TrackingData.SuspendData);
            }
            return new OkResult();
        }

        [HttpGet("reset/{id}")]
        public async Task<IActionResult> ResetModule(Guid id)
        {
            var module = await _ctx.Modules.Where(m => m.Id == id).Include(m => m.TrackingData).FirstOrDefaultAsync();
            module.TrackingData.Complete = false;
            module.TrackingData.Location = null;
            module.TrackingData.SuspendData = null;
            module.TrackingData.Progress = null;
            module.RestartedTimes = module.RestartedTimes + 1;
            
            await _ctx.SaveChangesAsync();
            return RedirectToAction("module", "Modules", new { @id = module.Id });
        }
        
        [HttpPut("tincan/activities/state")]
        public async Task<IActionResult> TinCan3([FromQuery] string stateId, [FromQuery] string activityId, [FromQuery] Guid registration, [FromBody] byte[] payload)
        {
            string value = System.Text.Encoding.UTF8.GetString(payload);

            var module = await _ctx.Modules.Include(m => m.TrackingData)
                .FirstOrDefaultAsync(m => m.Id == registration);

            if (stateId == "bookmark")
            {
                module.TrackingData.Location = value;
            }

            if (stateId == "suspend_data")
            {
                module.TrackingData.SuspendData = value;
            }

            await _ctx.SaveChangesAsync();
            return new OkResult();
        }

        [HttpPut("tincan/statements")]
        public async Task<IActionResult> TinCan2([FromQuery] string statementId, [FromBody] object data)
        {
            var dataString = JsonConvert.DeserializeObject<DataString>(data.ToString());
            var module = await _ctx.Modules.Include(m => m.TrackingData)
                .Where(m => m.Id == Guid.Parse(dataString.Context.Registration))
                .FirstOrDefaultAsync();

            if (dataString.Result.Completion != null)
            {
                module.TrackingData.Complete = bool.Parse(dataString.Result.Completion);
                if (bool.Parse(dataString.Result.Completion))
                {
                    module.CompletedTimes = module.CompletedTimes + 1;
                }
            }
            if (dataString.Result.Extensions != null)
            {
                module.TrackingData.Progress = dataString.Result.Extensions.Progress;
            }
            await _ctx.SaveChangesAsync();

            return Ok(dataString);
        }
    }
}
