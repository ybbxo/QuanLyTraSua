using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebTraSua.Models
{
    [Table("Size")]
    public class Size
    {
        [Key] public int MaSize { get; set; }
        [Required] public string TenSize { get; set; } = "";
        public bool IsBase { get; set; } = false;
        public decimal HeSoGia { get; set; } = 1m;
        public int PhuThu { get; set; } = 0;
        public decimal HeSoDinhLuong { get; set; } = 1m;
        public int DungTichML { get; set; }
    }
}
