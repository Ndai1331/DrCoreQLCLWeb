using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.LoaiPhanBon
{
    public class LoaiPhanBonModel : BaseModel<int>
    {
    }
    public class LoaiPhanBonCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }
}
