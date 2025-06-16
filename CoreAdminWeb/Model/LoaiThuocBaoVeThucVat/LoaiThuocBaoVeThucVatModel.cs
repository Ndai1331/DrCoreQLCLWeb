using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.LoaiThuocBaoVeThucVat  
{
    public class LoaiThuocBaoVeThucVatModel : BaseModel<int>
    {
    }
    public class LoaiThuocBaoVeThucVatCRUDModel : BaseDetailModel
    {
        public string status { set; get; }
    }
}