using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.Auth;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.DanhMucDungChung;
using CoreAdminWeb.Services.Files;
using CoreAdminWeb.Services.Menus;
using CoreAdminWeb.Services.Settings;
using CoreAdminWeb.Services.Users;
using CoreAdminWeb.Services.Reports;
using CoreAdminWeb.Model.Reports;

namespace CoreAdminWeb.DIInjections
{
    public static class DIInjections
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IBaseService<QLCLLoaiHinhKinhDoanhModel>, QLCLLoaiHinhKinhDoanhService>();
            services.AddScoped<IBaseService<QLCLLoaiSanPhamModel>, QLCLLoaiSanPhamService>();
            services.AddScoped<IBaseService<QLCLSanPhamSanXuatModel>, QLCLSanPhamSanXuatService>();
            services.AddScoped<IBaseService<TinhModel>, TinhService>();
            services.AddScoped<IBaseService<XaPhuongModel>, XaPhuongService>();
            services.AddScoped<IBaseService<DonViTinhModel>, DonViTinhService>();
            services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, AuthStateProvider>();
            services.AddScoped<IBaseService<LinhVucVanBanModel>, LinhVucVanBanService>();
            services.AddScoped<IBaseService<PhanLoaiVanBanModel>, PhanLoaiVanBanService>();
            services.AddScoped<IBaseService<FolderModel>, FolderService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IBaseService<CountryModel>, CountryService>();
            services.AddScoped<IBaseService<QLCLLoaiHinhCoSoModel>, QLCLLoaiHinhCoSoService>();
            services.AddScoped<IBaseService<QLCLNguyenLieuCheBienModel>, QLCLNguyenLieuCheBienService>();
            services.AddScoped<IBaseService<QLCLPhamViHoatDongModel>, QLCLPhamViHoatDongService>();
            services.AddScoped<IBaseService<QLCLCoSoCheBienNLTSModel>, QLCLCoSoCheBienNLTSService>();
            services.AddScoped<IBaseService<QLCLCoSoViPhamATTPModel>, QLCLCoSoViPhamATTPService>();
            services.AddScoped<IBaseService<QLCLKenhQuangBaXucTienThuongMaiModel>, QLCLKenhQuangBaXucTienThuongMaiService>();
            services.AddScoped<IBaseService<QLCLLoaiSanPhamModel>, QLCLLoaiSanPhamService>();
            services.AddScoped<IBaseService<QLCLSanPhamSanXuatModel>, QLCLSanPhamSanXuatService>();
            services.AddScoped<IBaseService<QLCLTinhHinhSXKDNLTSModel>, QLCLTinhHinhSXKDNLTSService>();
            services.AddScoped<IQLCLTinhHinhSXKDNLTSSanPhamService, QLCLTinhHinhSXKDNLTSSanPhamService>();
            services.AddScoped<IQLCLTinhHinhSXKDNLTSNguyenLieuService, QLCLTinhHinhSXKDNLTSNguyenLieuService>();
            services.AddScoped<IBaseService<QLCLHanhViViPhamModel>, QLCLHanhViViPhamService>();
            services.AddScoped<IBaseService<QLCLHinhThucXuPhatModel>, QLCLHinhThucXuPhatService>();
            services.AddScoped<IBaseService<QLCLCoSoNLTSDuDieuKienATTPModel>, QLCLCoSoNLTSDuDieuKienATTPService>();
            services.AddScoped<IQLCLCoSoNLTSDuDieuKienATTPSanPhamService, QLCLCoSoNLTSDuDieuKienATTPSanPhamService>();
            services.AddScoped<IBaseService<QLCLCoSoVatTuNongNghiepModel>, QLCLCoSoVatTuNongNghiepService>();
            services.AddScoped<IQLCLCoSoVatTuNongNghiepSanPhamService, QLCLCoSoVatTuNongNghiepSanPhamService>();
            services.AddScoped<IBaseService<QLCLDotKiemTraHauKiemATTPModel>, QLCLDotKiemTraHauKiemATTPService>();
            services.AddScoped<IBaseService<QLCLKiemTraHauKiemATTPModel>, QLCLKiemTraHauKiemATTPService>();
            services.AddScoped<IQLCLKiemTraHauKiemATTPChiTietService, QLCLKiemTraHauKiemATTPChiTietService>();
            services.AddScoped(typeof(IExportExcelService<>), typeof(ExportExcelService<>));
            services.AddScoped<IReportService<ReportBaoCaoKiemTraHauKiemATTPModel>, ReportBaoCaoKiemTraHauKiemATTPService>();
            services.AddScoped<IReportService<QLCLCoSoNLTSDuDieuKienATTPModel>, ReportBaoCaoChiTietKiemTraHauKiemATTPService>();
            services.AddScoped<IReportService<ReportBaoCaoKiemTraHauKiemLayMauATTPModel>, ReportBaoCaoKiemTraHauKiemLayMauATTPService>();
            services.AddScoped<IReportService<ReportBaoCaoThamDinhCapGCNModel>, ReportBaoCaoThamDinhCapGCNService>();
            services.AddScoped<IReportService<ReportDashboardModel>, ReportDashboardService>();
            services.AddScoped<AlertService>();
        }
    }
}