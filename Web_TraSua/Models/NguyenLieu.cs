using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_TraSua.Models;

namespace WebTraSua.Models
{
    [Table("NguyenLieu")]
    public class NguyenLieu
    {
        [Key] public int MaNL { get; set; }
        [Required] public string TenNL { get; set; } = "";
        [Required] public string DonViTinh { get; set; } = "ml";
        public ICollection<DanhMucNguyenLieu> DanhMucNguyenLieus { get; set; } = new List<DanhMucNguyenLieu>();
    }
}
