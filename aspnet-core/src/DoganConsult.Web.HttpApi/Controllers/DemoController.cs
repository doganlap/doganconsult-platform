using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoganConsult.Web.Demos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Web.Controllers
{
    [Route("api/demos")]
    [ApiController]
    public class DemoController : AbpControllerBase
    {
        private readonly IDemoAppService _demoAppService;

        public DemoController(IDemoAppService demoAppService)
        {
            _demoAppService = demoAppService;
        }

        [HttpGet("statistics")]
        public async Task<DemoStatisticsDto> GetStatisticsAsync()
        {
            return await _demoAppService.GetStatisticsAsync();
        }

        [HttpGet("recent")]
        public async Task<List<DemoRequestDto>> GetRecentAsync([FromQuery] int count = 5)
        {
            return await _demoAppService.GetRecentAsync(count);
        }

        [HttpGet]
        public async Task<List<DemoRequestDto>> GetListAsync([FromQuery] DemoFilterDto input)
        {
            return await _demoAppService.GetListAsync(input);
        }

        [HttpGet("{id}")]
        public async Task<DemoRequestDetailDto> GetAsync(int id)
        {
            return await _demoAppService.GetAsync(id);
        }

        [HttpPost]
        public async Task<DemoRequestDto> CreateAsync([FromBody] CreateDemoRequestDto input)
        {
            return await _demoAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public async Task<DemoRequestDto> UpdateAsync(int id, [FromBody] UpdateDemoRequestDto input)
        {
            return await _demoAppService.UpdateAsync(id, input);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveAsync(int id, [FromBody] ApproveRequestDto input)
        {
            await _demoAppService.ApproveAsync(id, input.ApprovedBy);
            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectAsync(int id, [FromBody] RejectRequestDto input)
        {
            await _demoAppService.RejectAsync(id, input.RejectedBy, input.Reason);
            return Ok();
        }

        [HttpPost("{id}/schedule")]
        public async Task<IActionResult> ScheduleAsync(int id, [FromBody] ScheduleRequestDto input)
        {
            await _demoAppService.ScheduleAsync(id, input.ScheduledDate);
            return Ok();
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartAsync(int id)
        {
            await _demoAppService.StartAsync(id);
            return Ok();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteAsync(int id)
        {
            await _demoAppService.CompleteAsync(id);
            return Ok();
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptAsync(int id)
        {
            await _demoAppService.AcceptAsync(id);
            return Ok();
        }

        [HttpPost("{id}/poc")]
        public async Task<IActionResult> MoveToPocAsync(int id)
        {
            await _demoAppService.MoveToPocAsync(id);
            return Ok();
        }

        [HttpPost("{id}/production")]
        public async Task<IActionResult> MoveToProductionAsync(int id)
        {
            await _demoAppService.MoveToProductionAsync(id);
            return Ok();
        }
    }

    public class ApproveRequestDto
    {
        public string ApprovedBy { get; set; } = string.Empty;
    }

    public class RejectRequestDto
    {
        public string RejectedBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class ScheduleRequestDto
    {
        public DateTime ScheduledDate { get; set; }
    }
}
