using Microsoft.EntityFrameworkCore;
using Web_TraSua.Models;
using WebTraSua.Models;

namespace WebTraSua.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DanhMucSanPham> DanhMucSanPhams { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<SanPhamSize> SanPhamSizes { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<NguyenLieu> NguyenLieus { get; set; }
        public DbSet<CongThucPhaChe> CongThucPhaChes { get; set; }
        public DbSet<NhapKho> NhapKhos { get; set; }
        public DbSet<DanhMucNguyenLieu> DanhMucNguyenLieus { get; set; }
        public DbSet<BinhLuan> BinhLuans { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SanPhamSize>()
                .HasKey(x => new { x.MaSP, x.MaSize });

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasKey(ct => new { ct.MaHD, ct.MaSP, ct.MaSize });

            modelBuilder.Entity<CongThucPhaChe>()
                .HasKey(ct => new { ct.MaSP, ct.MaNL });

            modelBuilder.Entity<DanhMucNguyenLieu>()
                .HasKey(dmnl => new { dmnl.MaDM, dmnl.MaNL });

            modelBuilder.Entity<DanhMucNguyenLieu>()
                .HasOne(dmnl => dmnl.DanhMuc)
                .WithMany(dm => dm.DanhMucNguyenLieus)
                .HasForeignKey(dmnl => dmnl.MaDM);

            modelBuilder.Entity<DanhMucNguyenLieu>()
                .HasOne(dmnl => dmnl.NguyenLieu)
                .WithMany(nl => nl.DanhMucNguyenLieus)
                .HasForeignKey(dmnl => dmnl.MaNL);

            modelBuilder.Entity<SanPhamSize>()
                .HasOne(sps => sps.SanPham)
                .WithMany(sp => sp.SanPhamSizes)
                .HasForeignKey(sps => sps.MaSP)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SanPhamSize>()
                .HasOne(sps => sps.Size)
                .WithMany()
                .HasForeignKey(sps => sps.MaSize)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(ct => ct.HoaDon)
                .WithMany()
                .HasForeignKey(ct => ct.MaHD)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChiTietHoaDon>()
       .HasOne(ct => ct.HoaDon)
       .WithMany(hd => hd.ChiTietHoaDons)  
       .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<HoaDon>()
    .HasOne(h => h.KhachHang)
    .WithMany()
    .HasForeignKey(h => h.MaKH)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(ct => ct.Size)
                .WithMany()
                .HasForeignKey(ct => ct.MaSize)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<NhapKho>(e =>
            {
                e.HasKey(x => x.MaNK); 
                e.HasOne(x => x.NguyenLieu).WithMany()
                 .HasForeignKey(x => x.MaNL).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.NhanVien).WithMany()
                 .HasForeignKey(x => x.MaNV).OnDelete(DeleteBehavior.Restrict);
                e.ToTable("NhapKho"); 
            });
        }
    }
}
