using CoreAdminWeb.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAdminWeb.Model
{
    /// <summary>
    /// Model representing a fertilizer production facility
    /// </summary>
    public class QLCLSanPhamSanXuatModel : BaseModel<int>
    {
        public QLCLLoaiSanPhamModel? loai_sp { get; set; }
    }

    /// <summary>
    /// Model for CRUD operations on fertilizer production facilities
    /// </summary>
    public class QLCLSanPhamSanXuatCRUDModel : BaseDetailModel
    {
        public string status { get; set; }
        public int? loai_sp { get; set; }
    }

}
