using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class CoSoSanXuatCheBienService : IBaseService<CoSoSanXuatCheBienModel>
    {
        private readonly string _collection = "CoSoSanXuatCheBien";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province,province.name,ward.id,ward,ward.name,loai_hinh_kinh_doanh.id,loai_hinh_kinh_doanh.code,loai_hinh_kinh_doanh.name";

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
        private static CoSoSanXuatCheBienCRUDModel MapToCRUDModel(CoSoSanXuatCheBienModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                sort = model.sort,
                status = model.status.ToString(),
                province = model.province?.id,
                ward = model.ward?.id,
                dia_chi = model.dia_chi,
                dien_thoai = model.dien_thoai,
                email = model.email,
                so_cccd = model.so_cccd,
                dai_dien = model.dai_dien,
                cong_suat_tan_nam = model.cong_suat_tan_nam,
                so_giay_phep = model.so_giay_phep,
                loai_hinh_kinh_doanh = model.loai_hinh_kinh_doanh?.id,
                ngay_cap = model.ngay_cap,
                thoi_han_den = model.thoi_han_den,
                co_quan_cap_phep = model.co_quan_cap_phep,
                san_pham = model.san_pham,
                thi_truong = model.thi_truong,
                quy_trinh_san_xuat = model.quy_trinh_san_xuat,
                chung_nhan_tieu_chuan = model.chung_nhan_tieu_chuan,
                ket_qua_kiem_tra = model.ket_qua_kiem_tra,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CoSoSanXuatCheBienModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CoSoSanXuatCheBienModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CoSoSanXuatCheBienModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CoSoSanXuatCheBienModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoSanXuatCheBienModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CoSoSanXuatCheBienModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CoSoSanXuatCheBienModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CoSoSanXuatCheBienModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CoSoSanXuatCheBienModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CoSoSanXuatCheBienModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoSanXuatCheBienModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CoSoSanXuatCheBienModel>> CreateAsync(CoSoSanXuatCheBienModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CoSoSanXuatCheBienModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CoSoSanXuatCheBienCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CoSoSanXuatCheBienModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<CoSoSanXuatCheBienModel>
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
                return CreateErrorResponse<CoSoSanXuatCheBienModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CoSoSanXuatCheBienModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoSanXuatCheBienCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CoSoSanXuatCheBienModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoSanXuatCheBienCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
