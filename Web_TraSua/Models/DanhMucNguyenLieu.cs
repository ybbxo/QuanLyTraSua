using System.ComponentModel.DataAnnotations.Schema;
using WebTraSua.Models;

namespace Web_TraSua.Models
{
    [Table("DanhMucNguyenLieu")]
    public class DanhMucNguyenLieu
    {
        public int MaDM { get; set; }
        public DanhMucSanPham DanhMuc { get; set; }
        public int MaNL { get; set; }
        public NguyenLieu NguyenLieu { get; set; }
    }
}
