using CoreAdminWeb.Shared.Base;
using Microsoft.JSInterop;
using CoreAdminWeb.Services.Reports;
using CoreAdminWeb.Model.Reports;
using System.Collections.Generic;
using System;

namespace CoreAdminWeb.Pages.Dashboard
{
    public partial class Dashboard(IReportService<ReportDashboardModel> MainService) : BlazorCoreBase
    {
        private ReportDashboardModel? MainModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            try
            {
                var result = await MainService.GetAllAsync("QLCLDashboard/");
                if (result.IsSuccess && result.Data != null && result.Data.Any())
                {
                    MainModel = result.Data.FirstOrDefault();
                    await InitializeLineChart();
                    await InitializeBarChart();
                }
                else
                {
                    MainModel = new ReportDashboardModel();
                    await InitializeLineChart();
                    await InitializeBarChart();
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi tải dữ liệu dashboard: {ex.Message}", "danger");
                MainModel = new ReportDashboardModel();
                await InitializeLineChart();
                await InitializeBarChart();
            }
        }

        private async Task InitializeLineChart()
        {
            try
            {
                string[] chartLabels = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", 
                                       "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
                
                var chartData = MainModel?.so_dot_kiem_tra_theo_thang?.FirstOrDefault();
                
                int[] chartSeries = chartData != null ? 
                [
                    chartData.t1, chartData.t2, chartData.t3, chartData.t4, chartData.t5, chartData.t6,
                    chartData.t7, chartData.t8, chartData.t9, chartData.t10, chartData.t11, chartData.t12
                ] : new int[12];

                await JsRuntime.InvokeVoidAsync("initBassicLineChart", "#tongDotKiemTraChart", chartSeries, chartLabels, new object[] { "#6a69f5" });
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi khởi tạo biểu đồ: {ex.Message}", "warning");
            }
        }


        private async Task InitializeBarChart()
        {
            try
            {
                string[] chartLabels = MainModel?.loai_hinh_co_so?.Select(x => x.name).ToArray() ?? new string[0];
                
                var chartData = MainModel?.loai_hinh_co_so;
                
                int[] chartSeries = chartData?.Select(x => x.so_luong_co_so).ToArray() ?? new int[0];

                // Generate predefined beautiful colors for better visual appeal
                var predefinedColors = new List<string>
                {
                    "#3B82F6", // Blue
                    "#EF4444", // Red
                    "#10B981", // Green
                    "#F59E0B", // Yellow
                    "#8B5CF6", // Purple
                    "#06B6D4", // Cyan
                    "#F97316", // Orange
                    "#EC4899", // Pink
                    "#84CC16", // Lime
                    "#6366F1", // Indigo
                    "#14B8A6", // Teal
                    "#F43F5E"  // Rose
                };

                var colors = new List<string>();
                for (int i = 0; i < chartLabels.Length; i++)
                {
                    // Use predefined colors first, then generate random colors if needed
                    if (i < predefinedColors.Count)
                    {
                        colors.Add(predefinedColors[i]);
                    }
                    else
                    {
                        var random = new Random();
                        var color = $"#{random.Next(0x1000000):X6}";
                        colors.Add(color);
                    }
                }

                await JsRuntime.InvokeVoidAsync("initBassicBarChart", "#coSoSanXuatChart", chartSeries, chartLabels, colors.ToArray());
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi khởi tạo biểu đồ: {ex.Message}", "warning");
            }
        }
    }
}
