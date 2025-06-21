using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLKenhQuangBaXucTienThuongMaiService : IBaseService<QLCLKenhQuangBaXucTienThuongMaiModel>
    {
        private readonly string _collection = "QLCLKenhQuangBaXucTienThuongMai";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name";

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
        private static QLCLKenhQuangBaXucTienThuongMaiCRUDModel MapToCRUDModel(QLCLKenhQuangBaXucTienThuongMaiModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                sort = model.sort,
                status = model.status.ToString(),
                deleted = model.deleted,
                ngay_to_chuc = model.ngay_to_chuc,
                dia_diem_to_chuc = model.dia_diem_to_chuc,
                province = model.province?.id,
                ward = model.ward?.id,
                kenh_quang_ba = model.kenh_quang_ba,
                hinh_thuc_quang_ba = model.hinh_thuc_quang_ba,
                pham_vi_tiep_can = model.pham_vi_tiep_can,
                doi_tuong_tiep_can = model.doi_tuong_tiep_can,
                so_luong_chu_the_tham_gia = model.so_luong_chu_the_tham_gia,
                luot_khach_tham_quan = model.luot_khach_tham_quan,
                so_hop_dong_ky_ket = model.so_hop_dong_ky_ket,
                gia_tri_giao_dich = model.gia_tri_giao_dich,
                san_pham_tham_gia = model.san_pham_tham_gia,
                nguon_kinh_phi_thuc_hien = model.nguon_kinh_phi_thuc_hien,
                noi_dung_chuong_trinh = model.noi_dung_chuong_trinh
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLKenhQuangBaXucTienThuongMaiModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLKenhQuangBaXucTienThuongMaiModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLKenhQuangBaXucTienThuongMaiModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLKenhQuangBaXucTienThuongMaiModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLKenhQuangBaXucTienThuongMaiModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLKenhQuangBaXucTienThuongMaiModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel>> CreateAsync(QLCLKenhQuangBaXucTienThuongMaiModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiModel>
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
                return CreateErrorResponse<QLCLKenhQuangBaXucTienThuongMaiModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLKenhQuangBaXucTienThuongMaiModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLKenhQuangBaXucTienThuongMaiModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLKenhQuangBaXucTienThuongMaiCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
