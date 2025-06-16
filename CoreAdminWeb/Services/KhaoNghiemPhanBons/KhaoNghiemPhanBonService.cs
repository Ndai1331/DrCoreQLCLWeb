using CoreAdminWeb.Model.KhaoNghiemPhanBon;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.KhaoNghiemPhanBons
{
    public class KhaoNghiemPhanBonService : IBaseService<KhaoNghiemPhanBonModel>
    {
        private readonly string _collection = "KhaoNghiemPhanBon";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",phan_bon.id,phan_bon.name"
            + ",co_so_san_xuat_phan_bon.id,co_so_san_xuat_phan_bon.name";

        /// <summary>
        /// Creates a response with error handling
        /// </summary>
        private static RequestHttpResponse<T> CreateErrorResponse<T>(Exception ex)
        {
            return new RequestHttpResponse<T>
            {
                Errors = new List<ErrorResponse> { new() { Message = ex.Message } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        /// <summary>
        /// Maps a model to CRUD model
        /// </summary>
        private static KhaoNghiemPhanBonCRUDModel MapToCRUDModel(KhaoNghiemPhanBonModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                status = model.status.ToString(),
                sort = model.sort,
                dia_diem_thuc_hien = model.dia_diem_thuc_hien,
                loai_hinh_khao_nghiem = model.loai_hinh_khao_nghiem,
                muc_tieu = model.muc_tieu,
                thanh_phan_cong_thuc = model.thanh_phan_cong_thuc,
                chi_tieu_ky_thuat = model.chi_tieu_ky_thuat,
                quy_mo_khao_nghiem = model.quy_mo_khao_nghiem,
                tieu_chuan_thu_nghiem = model.tieu_chuan_thu_nghiem,
                ngay_bat_dau = model.ngay_bat_dau,
                ngay_ket_thuc = model.ngay_ket_thuc,
                phan_bon = model.phan_bon?.id,
                co_so_san_xuat_phan_bon = model.co_so_san_xuat_phan_bon?.id
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<KhaoNghiemPhanBonModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<KhaoNghiemPhanBonModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<KhaoNghiemPhanBonModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<KhaoNghiemPhanBonModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<KhaoNghiemPhanBonModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<KhaoNghiemPhanBonModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<KhaoNghiemPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<KhaoNghiemPhanBonModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<KhaoNghiemPhanBonModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<KhaoNghiemPhanBonModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<KhaoNghiemPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<KhaoNghiemPhanBonModel>> CreateAsync(KhaoNghiemPhanBonModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<KhaoNghiemPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<KhaoNghiemPhanBonCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<KhaoNghiemPhanBonModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<KhaoNghiemPhanBonModel>
                {
                    Data = new()
                    {
                        code = response.Data?.Data?.code,
                        name = response.Data?.Data?.name
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<KhaoNghiemPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(KhaoNghiemPhanBonModel model)
        {
            if (model == null || model.id == 0)
            {
                return new RequestHttpResponse<bool>
                {
                    Data = false,
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng chọn bản ghi để cập nhật" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var updateModel = MapToCRUDModel(model);
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<KhaoNghiemPhanBonCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

                return new RequestHttpResponse<bool>
                {
                    Data = response.IsSuccess,
                    Errors = response.Errors
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<bool>(ex);
            }
        }

        /// <summary>
        /// Deletes a fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> DeleteAsync(KhaoNghiemPhanBonModel model)
        {
            if (model == null || model.id == 0)
            {
                return new RequestHttpResponse<bool>
                {
                    Data = false,
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng chọn bản ghi để xoá" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<KhaoNghiemPhanBonCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

                return new RequestHttpResponse<bool>
                {
                    Data = response.IsSuccess,
                    Errors = response.Errors
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<bool>(ex);
            }
        }
    }
}
