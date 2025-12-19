using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DoganConsult.Web.Demos
{
    public interface IDemoAppService : IApplicationService
    {
        Task<DemoStatisticsDto> GetStatisticsAsync();
        
        Task<List<DemoRequestDto>> GetRecentAsync(int count = 5);
        
        Task<List<DemoRequestDto>> GetListAsync(DemoFilterDto input);
        
        Task<DemoRequestDetailDto> GetAsync(int id);
        
        Task<DemoRequestDto> CreateAsync(CreateDemoRequestDto input);
        
        Task<DemoRequestDto> UpdateAsync(int id, UpdateDemoRequestDto input);
        
        Task ApproveAsync(int id, string approvedBy);
        
        Task RejectAsync(int id, string rejectedBy, string reason);
        
        Task ScheduleAsync(int id, DateTime scheduledDate);
        
        Task StartAsync(int id);
        
        Task CompleteAsync(int id);
        
        Task AcceptAsync(int id);
        
        Task MoveToPocAsync(int id);
        
        Task MoveToProductionAsync(int id);
    }
}
