using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.NhomThuocBaoVeThucVat  
{
    public class NhomThuocBaoVeThucVatModel : BaseModel<int>
    {
    }
    public class NhomThuocBaoVeThucVatCRUDModel : BaseDetailModel
    {
        public string status { set; get; }
    }
}