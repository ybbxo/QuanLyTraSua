using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTraSua.Models
{
    [Table("NhapKho")] 
    public class NhapKho
    {
        [Key]                    
        public int MaNK { get; set; }
        [Required]
        public int MaNL { get; set; }
        [ForeignKey(nameof(MaNL))]
        public NguyenLieu? NguyenLieu { get; set; }  
        [Required]
        public int MaNV { get; set; }
        [ForeignKey(nameof(MaNV))]
        public NhanVien? NhanVien { get; set; }      
        [Required, Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int SoLuong { get; set; }
        public DateTime NgayNhap { get; set; } = DateTime.Now;
    }
}
