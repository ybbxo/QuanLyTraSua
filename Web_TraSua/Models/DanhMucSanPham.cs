using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_TraSua.Models;

namespace WebTraSua.Models
{
    [Table("DanhMucSanPham")]
    public class DanhMucSanPham
    {
        [Key] public int MaDM { get; set; }
        [Required] public string TenDM { get; set; } = "";
        public ICollection<DanhMucNguyenLieu> DanhMucNguyenLieus { get; set; } = new List<DanhMucNguyenLieu>();
    }
}
