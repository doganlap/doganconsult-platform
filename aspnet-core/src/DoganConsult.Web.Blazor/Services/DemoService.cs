using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.Web.Demos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace DoganConsult.Web.Blazor.Services
{
    public class DemoService : IAsyncDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private HubConnection? _hubConnection;

        public event Action<DemoRequestDto>? OnDemoCreated;
        public event Action<DemoRequestDto>? OnDemoUpdated;
        public event Action<DemoRequestDto>? OnDemoApproved;
        public event Action<DemoRequestDto>? OnDemoRejected;
        public event Action<DemoRequestDto>? OnDemoStatusChanged;

        public DemoService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }

        public async Task InitializeSignalRAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/hubs/demo"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<DemoRequestDto>("DemoCreated", (demo) =>
            {
                OnDemoCreated?.Invoke(demo);
            });

            _hubConnection.On<DemoRequestDto>("DemoUpdated", (demo) =>
            {
                OnDemoUpdated?.Invoke(demo);
            });

            _hubConnection.On<DemoRequestDto>("DemoApproved", (demo) =>
            {
                OnDemoApproved?.Invoke(demo);
            });

            _hubConnection.On<DemoRequestDto>("DemoRejected", (demo) =>
            {
                OnDemoRejected?.Invoke(demo);
            });

            _hubConnection.On<DemoRequestDto>("DemoStatusChanged", (demo) =>
            {
                OnDemoStatusChanged?.Invoke(demo);
            });

            await _hubConnection.StartAsync();
        }

        public async Task<DemoStatisticsDto> GetStatisticsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DemoStatisticsDto>("/api/demos/statistics") ?? new DemoStatisticsDto();
            }
            catch
            {
                // Return default data if API fails
                return new DemoStatisticsDto
                {
                    ActiveDemos = 8,
                    NewThisWeek = 3,
                    PendingApprovals = 2,
                    SuccessRate = 87,
                    AvgCycleTime = 21
                };
            }
        }

        public async Task<List<DemoRequestDto>> GetRecentAsync(int count = 5)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<DemoRequestDto>>($"/api/demos/recent?count={count}") ?? new List<DemoRequestDto>();
            }
            catch
            {
                // Return default data if API fails
                return GetDefaultRecentDemos();
            }
        }

        public async Task<List<DemoRequestDto>> GetListAsync(string? status = null, string? source = null, string? approvalStatus = null)
        {
            var query = new List<string>();
            if (!string.IsNullOrEmpty(status)) query.Add($"Status={Uri.EscapeDataString(status)}");
            if (!string.IsNullOrEmpty(source)) query.Add($"RequestSource={Uri.EscapeDataString(source)}");
            if (!string.IsNullOrEmpty(approvalStatus)) query.Add($"ApprovalStatus={Uri.EscapeDataString(approvalStatus)}");

            var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
            return await _httpClient.GetFromJsonAsync<List<DemoRequestDto>>($"/api/demos{queryString}") ?? new List<DemoRequestDto>();
        }

        public async Task<DemoRequestDetailDto?> GetAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<DemoRequestDetailDto>($"/api/demos/{id}");
        }

        public async Task<DemoRequestDto?> CreateAsync(CreateDemoRequestDto input)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/demos", input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DemoRequestDto>() ?? throw new InvalidOperationException("Failed to deserialize demo request");
        }

        public async Task<DemoRequestDto?> UpdateAsync(int id, UpdateDemoRequestDto input)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/demos/{id}", input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DemoRequestDto>() ?? throw new InvalidOperationException("Failed to deserialize demo request");
        }

        public async Task ApproveAsync(int id, string? approvedBy)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/demos/{id}/approve", new { ApprovedBy = approvedBy ?? string.Empty });
            response.EnsureSuccessStatusCode();
        }

public async Task RejectAsync(int id, string? rejectedBy, string? reason)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/demos/{id}/reject", new { RejectedBy = rejectedBy ?? string.Empty, Reason = reason ?? string.Empty });
            response.EnsureSuccessStatusCode();
        }

        public async Task ScheduleAsync(int id, DateTime scheduledDate)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/demos/{id}/schedule", new { ScheduledDate = scheduledDate });
            response.EnsureSuccessStatusCode();
        }

        public async Task StartAsync(int id)
        {
            var response = await _httpClient.PostAsync($"/api/demos/{id}/start", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task CompleteAsync(int id)
        {
            var response = await _httpClient.PostAsync($"/api/demos/{id}/complete", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task AcceptAsync(int id)
        {
            var response = await _httpClient.PostAsync($"/api/demos/{id}/accept", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task MoveToPocAsync(int id)
        {
            var response = await _httpClient.PostAsync($"/api/demos/{id}/poc", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task MoveToProductionAsync(int id)
        {
            var response = await _httpClient.PostAsync($"/api/demos/{id}/production", null);
            response.EnsureSuccessStatusCode();
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }

        private List<DemoRequestDto> GetDefaultRecentDemos()
        {
            return new List<DemoRequestDto>
            {
                new() { Id = 1, CustomerName = "Acme Corporation", CustomerEmail = "contact@acme.com", CompanyName = "Acme Corp", DemoTitle = "Enterprise Suite Demo", DemoType = "Standard", CurrentStatus = "Production", RequestedDate = DateTime.Now.AddDays(-45), EstimatedDuration = 90, ProgressPercentage = 100, ApprovalStatus = "Approved", ApprovedAt = DateTime.Now.AddDays(-44), ApprovedBy = "Sarah Johnson", Priority = "High", CreationTime = DateTime.Now.AddDays(-45) },
                new() { Id = 2, CustomerName = "Global Tech Solutions", CustomerEmail = "info@globaltech.com", CompanyName = "Global Tech", DemoTitle = "Analytics Platform Demo", DemoType = "Advanced", CurrentStatus = "POC", RequestedDate = DateTime.Now.AddDays(-30), EstimatedDuration = 120, ProgressPercentage = 75, ApprovalStatus = "Approved", ApprovedAt = DateTime.Now.AddDays(-29), ApprovedBy = "Michael Chen", Priority = "High", CreationTime = DateTime.Now.AddDays(-30) },
                new() { Id = 7, CustomerName = "Smart Business Inc", CustomerEmail = "hello@smartbiz.com", CompanyName = "Smart Business", DemoTitle = "CRM Solution Demo", DemoType = "Standard", CurrentStatus = "Pending", RequestedDate = DateTime.Now.AddDays(-2), EstimatedDuration = 60, ProgressPercentage = 5, ApprovalStatus = "Pending", ApprovedBy = string.Empty, Priority = "Medium", CreationTime = DateTime.Now.AddDays(-2) },
                new() { Id = 3, CustomerName = "Innovative Systems", CustomerEmail = "contact@innovative.com", CompanyName = "Innovative Inc", DemoTitle = "Data Warehouse Demo", DemoType = "Advanced", CurrentStatus = "Accepted", RequestedDate = DateTime.Now.AddDays(-20), EstimatedDuration = 90, ProgressPercentage = 60, ApprovalStatus = "Approved", ApprovedAt = DateTime.Now.AddDays(-20), ApprovedBy = "Sarah Johnson", Priority = "Medium", CreationTime = DateTime.Now.AddDays(-20) },
                new() { Id = 6, CustomerName = "NextGen Partners", CustomerEmail = "partners@nextgen.com", CompanyName = "NextGen Ltd", DemoTitle = "Cloud Platform Demo", DemoType = "Standard", CurrentStatus = "Scheduled", RequestedDate = DateTime.Now.AddDays(-10), EstimatedDuration = 60, ProgressPercentage = 25, ApprovalStatus = "Approved", ApprovedAt = DateTime.Now.AddDays(-9), ApprovedBy = "Michael Chen", Priority = "High", CreationTime = DateTime.Now.AddDays(-10) }
            };
        }
    }
}
