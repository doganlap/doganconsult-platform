using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Web.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;

namespace DoganConsult.Web.Demos
{
    [Authorize(WebPermissions.Demos.Default)]
    public class DemoAppService : ApplicationService, IDemoAppService
    {
        private readonly IDemoRepository _demoRepository;
        private readonly IDistributedCache<DemoStatisticsDto> _statisticsCache;
        private readonly IDistributedCache<List<DemoRequestDto>> _recentDemosCache;

        public DemoAppService(
            IDemoRepository demoRepository,
            IDistributedCache<DemoStatisticsDto> statisticsCache,
            IDistributedCache<List<DemoRequestDto>> recentDemosCache)
        {
            _demoRepository = demoRepository;
            _statisticsCache = statisticsCache;
            _recentDemosCache = recentDemosCache;
        }

        public async Task<DemoStatisticsDto> GetStatisticsAsync()
        {
            return await _statisticsCache.GetOrAddAsync(
                "DemoStatistics",
                async () => await CalculateStatisticsAsync(),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }
            ) ?? new DemoStatisticsDto();
        }

        public async Task<List<DemoRequestDto>> GetRecentAsync(int count = 5)
        {
            return await _recentDemosCache.GetOrAddAsync(
                $"RecentDemos_{count}",
                async () => await GetRecentDemosFromSourceAsync(count),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                }
            ) ?? new List<DemoRequestDto>();
        }

        public async Task<List<DemoRequestDto>> GetListAsync(DemoFilterDto input)
        {
            var demos = await _demoRepository.GetListAsync(
                status: input.Status,
                priority: input.Priority,
                assignedTo: input.AssignedTo,
                skipCount: 0,
                maxResultCount: 100
            );
            
            return ObjectMapper.Map<List<DemoRequest>, List<DemoRequestDto>>(demos);
        }

        public async Task<DemoRequestDetailDto> GetAsync(int id)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null)
            {
                throw new EntityNotFoundException(typeof(DemoRequest), id);
            }

            return ObjectMapper.Map<DemoRequest, DemoRequestDetailDto>(demo);
        }

        [Authorize(WebPermissions.Demos.Create)]
        public async Task<DemoRequestDto> CreateAsync(CreateDemoRequestDto input)
        {
            var demo = new DemoRequest(
                id: 0,
                customerName: input.CustomerName ?? string.Empty,
                customerEmail: input.CustomerEmail ?? string.Empty,
                demoTitle: input.DemoTitle ?? "New Demo Request",
                demoType: input.DemoType ?? "Standard",
                requestedDate: input.RequestedDate ?? DateTime.UtcNow,
                estimatedDuration: input.EstimatedDuration ?? 60
            );

            demo.CustomerPhone = input.CustomerPhone;
            demo.CompanyName = input.CompanyName;
            demo.Industry = input.Industry;
            demo.DemoDescription = input.DemoDescription;
            demo.SpecialRequirements = input.SpecialRequirements;
            demo.Priority = input.Priority ?? "Medium";

            var createdDemo = await _demoRepository.InsertAsync(demo, autoSave: true);

            // Invalidate caches
            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");

            return ObjectMapper.Map<DemoRequest, DemoRequestDto>(createdDemo);
        }

        [Authorize(WebPermissions.Demos.Edit)]
        public async Task<DemoRequestDto> UpdateAsync(int id, UpdateDemoRequestDto input)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null)
            {
                throw new EntityNotFoundException(typeof(DemoRequest), id);
            }

            if (!string.IsNullOrEmpty(input.CustomerName))
                demo.CustomerName = input.CustomerName;
            if (!string.IsNullOrEmpty(input.CustomerEmail))
                demo.CustomerEmail = input.CustomerEmail;
            if (!string.IsNullOrEmpty(input.CompanyName))
                demo.CompanyName = input.CompanyName;

            var updatedDemo = await _demoRepository.UpdateAsync(demo, autoSave: true);

            // Invalidate caches
            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");

            return ObjectMapper.Map<DemoRequest, DemoRequestDto>(updatedDemo);
        }

        [Authorize(WebPermissions.Demos.Approve)]
        public async Task ApproveAsync(int id, string approvedBy)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null)
            {
                throw new EntityNotFoundException(typeof(DemoRequest), id);
            }

            demo.Approve(approvedBy);
            await _demoRepository.UpdateAsync(demo, autoSave: true);

            // Invalidate caches
            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");
        }

        [Authorize(WebPermissions.Demos.Approve)]
        public async Task RejectAsync(int id, string rejectedBy, string reason)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null)
            {
                throw new EntityNotFoundException(typeof(DemoRequest), id);
            }

            demo.Reject(rejectedBy, reason);
            await _demoRepository.UpdateAsync(demo, autoSave: true);

            // Invalidate caches
            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");
        }

        [Authorize(WebPermissions.Demos.ManageWorkflow)]
        public async Task ScheduleAsync(int id, DateTime scheduledDate)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null) throw new EntityNotFoundException(typeof(DemoRequest), id);

            demo.Schedule(scheduledDate, CurrentUser.UserName ?? "System");
            await _demoRepository.UpdateAsync(demo, autoSave: true);

            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");
        }

        [Authorize(WebPermissions.Demos.ManageWorkflow)]
        public async Task StartAsync(int id)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null) throw new EntityNotFoundException(typeof(DemoRequest), id);

            demo.Start();
            await _demoRepository.UpdateAsync(demo, autoSave: true);

            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");
        }

        [Authorize(WebPermissions.Demos.ManageWorkflow)]
        public async Task CompleteAsync(int id)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null) throw new EntityNotFoundException(typeof(DemoRequest), id);

            demo.Complete();
            await _demoRepository.UpdateAsync(demo, autoSave: true);

            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");
        }

        [Authorize(WebPermissions.Demos.ManageWorkflow)]
        public async Task AcceptAsync(int id)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null) throw new EntityNotFoundException(typeof(DemoRequest), id);

            demo.MoveToAccepted();
            await _demoRepository.UpdateAsync(demo, autoSave: true);

            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");
        }

        [Authorize(WebPermissions.Demos.ManageWorkflow)]
        public async Task MoveToPocAsync(int id)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null) throw new EntityNotFoundException(typeof(DemoRequest), id);

            demo.MoveToPOC();
            await _demoRepository.UpdateAsync(demo, autoSave: true);

            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");
        }

        [Authorize(WebPermissions.Demos.ManageWorkflow)]
        public async Task MoveToProductionAsync(int id)
        {
            var demo = await _demoRepository.GetAsync(id);
            if (demo == null) throw new EntityNotFoundException(typeof(DemoRequest), id);

            demo.MoveToProduction();
            await _demoRepository.UpdateAsync(demo, autoSave: true);

            await _statisticsCache.RemoveAsync("DemoStatistics");
            await _recentDemosCache.RemoveAsync("RecentDemos_5");
        }

        // Helper methods
        private async Task<DemoStatisticsDto> CalculateStatisticsAsync()
        {
            var statusCounts = await _demoRepository.GetCountByStatusAsync();
            var allDemos = await _demoRepository.GetListAsync(skipCount: 0, maxResultCount: 1000);
            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);

            return new DemoStatisticsDto
            {
                ActiveDemos = allDemos.Count(d => d.CurrentStatus != "Production" && d.CurrentStatus != "Rejected"),
                NewThisWeek = allDemos.Count(d => d.CreationTime >= weekAgo),
                PendingApprovals = allDemos.Count(d => d.ApprovalStatus == "Pending"),
                SuccessRate = allDemos.Any() ? (allDemos.Count(d => d.CurrentStatus == "Production") * 100 / allDemos.Count) : 0,
                AvgCycleTime = CalculateAverageCycleTime(allDemos)
            };
        }

        private int CalculateAverageCycleTime(List<DemoRequest> demos)
        {
            var completedDemos = demos.Where(d => d.CompletedAt.HasValue).ToList();
            if (!completedDemos.Any()) return 0;

            var totalDays = completedDemos.Sum(d => (d.CompletedAt!.Value - d.CreationTime).TotalDays);
            return (int)(totalDays / completedDemos.Count);
        }

        private async Task<List<DemoRequestDto>> GetRecentDemosFromSourceAsync(int count)
        {
            var demos = await _demoRepository.GetRecentAsync(count);
            return ObjectMapper.Map<List<DemoRequest>, List<DemoRequestDto>>(demos);
        }
    }
}
