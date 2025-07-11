using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Services.Reports
{
    public class ReportQLCLBienDongGiaService : IReportService<QLCLBienDongGiaModel>
    {
       
        public async Task<RequestHttpResponse<List<QLCLBienDongGiaModel>>> GetAllAsync(string query)
        {
            try
            {
                var response = await ReportRequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLBienDongGiaModel>>>(query);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLBienDongGiaModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLBienDongGiaModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLBienDongGiaModel>>(ex);
            }
        }
        private static RequestHttpResponse<T> CreateErrorResponse<T>(Exception ex)
        {
            return new RequestHttpResponse<T>
            {
                Errors = new List<ErrorResponse>
                {
                    new()
                    {
                        Message = ex.Message,
                        Code = "500"
                    }
                }
            };
        }
    }
}
