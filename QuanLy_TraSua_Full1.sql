CREATE DATABASE QuanLy_TraSua_Full1;
GO
USE QuanLy_TraSua_Full1;
GO
-- 1) Khách hàng
CREATE TABLE KhachHang (
    MaKH INT PRIMARY KEY IDENTITY,
    HoTen NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    MatKhau NVARCHAR(100) NOT NULL,
    SDT NVARCHAR(15),
    DiemTichLuy INT DEFAULT 0,
    NgayTao DATETIME DEFAULT GETDATE()
);
select*from KhachHang
-- 2) Nhân viên
CREATE TABLE NhanVien (
    MaNV INT PRIMARY KEY IDENTITY,
    HoTen NVARCHAR(100),
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    SDT NVARCHAR(15),
    DiaChi NVARCHAR(200),
    TenChucVu NVARCHAR(50),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    MatKhau NVARCHAR(100) NOT NULL,
    VaiTro NVARCHAR(20) DEFAULT N'Admin'
);

-- 3) Danh mục sản phẩm
CREATE TABLE DanhMucSanPham (
    MaDM INT PRIMARY KEY IDENTITY,
    TenDM NVARCHAR(100)
);
select*from HoaDon
-- 4) Size
CREATE TABLE Size (
    MaSize INT PRIMARY KEY IDENTITY,
    TenSize NVARCHAR(10),
    IsBase  BIT NOT NULL DEFAULT 0,          
    HeSoGia DECIMAL(5,2) NOT NULL DEFAULT 1, 
    PhuThu  INT NOT NULL DEFAULT 0,           
    HeSoDinhLuong DECIMAL(5,2) NOT NULL DEFAULT 1
);

-- 5) Sản phẩm
CREATE TABLE SanPham (
    MaSP INT PRIMARY KEY IDENTITY,
    TenSP NVARCHAR(100),
    MaDM INT,
    Anh NVARCHAR(255) NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (MaDM) REFERENCES DanhMucSanPham(MaDM)
);

-- 6) Giá theo size
CREATE TABLE SanPhamSize (
    MaSP INT,
    MaSize INT,
    Gia DECIMAL(18,2),
    PRIMARY KEY (MaSP, MaSize),
    FOREIGN KEY (MaSP)  REFERENCES SanPham(MaSP),
    FOREIGN KEY (MaSize) REFERENCES Size(MaSize)
);

-- 7) Nguyên liệu
CREATE TABLE NguyenLieu (
    MaNL INT PRIMARY KEY IDENTITY,
    TenNL NVARCHAR(100),
    DonViTinh NVARCHAR(20)
);

-- 8) Công thức pha chế (theo size cơ sở)
CREATE TABLE CongThucPhaChe (
    MaSP INT,
    MaNL INT,
    SoLuongCoSo DECIMAL(18,2),
    PRIMARY KEY (MaSP, MaNL),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP),
    FOREIGN KEY (MaNL) REFERENCES NguyenLieu(MaNL)
);

-- 9) Hình thức thanh toán
CREATE TABLE HinhThucThanhToan (
    MaHTTT INT PRIMARY KEY IDENTITY,
    TenHinhThuc NVARCHAR(50)
);

-- 10) Hoá đơn
CREATE TABLE HoaDon (
    MaHD INT PRIMARY KEY IDENTITY,
    MaKH INT,
    MaNV INT,
    NgayLap DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) DEFAULT 0,
    MaHTTT INT,
    TrangThai NVARCHAR(50) DEFAULT N'Chờ xác nhận',
    FOREIGN KEY (MaKH)  REFERENCES KhachHang(MaKH),
    FOREIGN KEY (MaNV)  REFERENCES NhanVien(MaNV),
    FOREIGN KEY (MaHTTT) REFERENCES HinhThucThanhToan(MaHTTT)
);

-- 11) Chi tiết hoá đơn
CREATE TABLE ChiTietHoaDon (
    MaHD INT,
    MaSP INT,
    MaSize INT,
    SoLuong INT DEFAULT 1,
    DonGia DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaHD, MaSP, MaSize),
    FOREIGN KEY (MaHD)  REFERENCES HoaDon(MaHD),
    FOREIGN KEY (MaSP)  REFERENCES SanPham(MaSP),
FOREIGN KEY (MaSize) REFERENCES Size(MaSize)
);

-- 12) Topping
CREATE TABLE Topping (
    ToppingId INT PRIMARY KEY IDENTITY,
    TenTopping NVARCHAR(100),
    Gia DECIMAL(18,2)
);

-- 13) Topping theo hoá đơn
CREATE TABLE ToppingHoaDon (
    MaHD INT,
    ToppingId INT,
    SoLuong INT DEFAULT 1,
    PRIMARY KEY (MaHD, ToppingId),
    FOREIGN KEY (MaHD)     REFERENCES HoaDon(MaHD),
    FOREIGN KEY (ToppingId) REFERENCES Topping(ToppingId)
);
CREATE TABLE NhapKho (
    MaNK INT IDENTITY(1,1) PRIMARY KEY,     
    MaNL INT NOT NULL,                      
    SoLuong INT NOT NULL,                    
    NgayNhap DATETIME NOT NULL DEFAULT GETDATE(), 
    MaNV INT NOT NULL,                     
    CONSTRAINT FK_NhapKho_NguyenLieu FOREIGN KEY (MaNL) REFERENCES NguyenLieu(MaNL),
    CONSTRAINT FK_NhapKho_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);
CREATE TABLE BinhLuan (
    MaBL INT IDENTITY(1,1) PRIMARY KEY,
    MaSP INT NOT NULL,                     
    TenNguoiDung NVARCHAR(100) NOT NULL,  
    NoiDung NVARCHAR(MAX) NOT NULL,       
    Sao INT NOT NULL CHECK (Sao BETWEEN 1 AND 5),  
    NgayBinhLuan DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_BinhLuan_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
ALTER TABLE SanPham
ADD MoTa NVARCHAR(MAX) NULL;
ALTER TABLE BinhLuan
ADD TrangThai NVARCHAR(20) DEFAULT 'Chưa duyệt';
INSERT INTO BinhLuan (MaSP, TenNguoiDung, NoiDung, Sao)
VALUES
(1, N'Nguyễn Văn A', N'Trà sữa truyền thống rất ngon, thơm mùi trà.', 5),
(3, N'Phạm Thị B', N'Trà sữa matcha có vị đắng nhẹ, rất vừa miệng.', 4),
(10, N'Lê C', N'Trái cây tươi, uống mát, rất hài lòng.', 5),
(15, N'Hoàng D', N'Cà phê ngon, thơm và đậm vị.', 4),
(1, N'Mai E', N'Mình thích vị ngọt thanh của trà sữa.', 4),
(5, N'Trần F', N'Không quá ngọt, phù hợp với khẩu vị của tôi.', 3);
INSERT INTO NhapKho (MaNL, SoLuong, MaNV)
VALUES 
(1, 100, 1),  -- Trà Sữa, nhập 100ml
(2, 200, 1),  -- Đường, nhập 200g
(3, 50, 1);   -- Đá, nhập 50g

GO
/* =====================
   HÀM + TRIGGER GIÁ THEO SIZE
   ===================== */
IF OBJECT_ID('dbo.fn_TinhGiaSize', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_TinhGiaSize;
GO
CREATE FUNCTION dbo.fn_TinhGiaSize
(
    @GiaCoSo DECIMAL(18,2),
    @HeSoGia DECIMAL(5,2),
    @PhuThu  INT
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @val DECIMAL(18,2) = (@GiaCoSo * @HeSoGia) + @PhuThu;
    RETURN ROUND(@val, -3);
END
GO
-- Xóa trigger cũ nếu tồn tại
IF OBJECT_ID('trg_SanPhamSize_AutoPricing', 'TR') IS NOT NULL
    DROP TRIGGER trg_SanPhamSize_AutoPricing;
GO
-- Tạo lại trigger
CREATE TRIGGER trg_SanPhamSize_AutoPricing
ON SanPhamSize
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AffectedSP AS (
        SELECT DISTINCT MaSP FROM inserted
    ),
    BasePrice AS (
        SELECT sps.MaSP, sps.Gia AS GiaCoSo
        FROM SanPhamSize sps
        JOIN Size sz ON sz.MaSize = sps.MaSize AND sz.IsBase = 1
        JOIN AffectedSP a ON a.MaSP = sps.MaSP
    ),
    ToRecalc AS (
        SELECT bp.MaSP, sz.MaSize,
               dbo.fn_TinhGiaSize(bp.GiaCoSo, sz.HeSoGia, sz.PhuThu) AS GiaMoi
        FROM BasePrice bp
        JOIN Size sz ON sz.IsBase = 0
    )
    MERGE SanPhamSize AS T
    USING ToRecalc AS S
      ON T.MaSP = S.MaSP AND T.MaSize = S.MaSize
    WHEN MATCHED THEN
        UPDATE SET Gia = S.GiaMoi
    WHEN NOT MATCHED THEN
        INSERT (MaSP, MaSize, Gia) VALUES (S.MaSP, S.MaSize, S.GiaMoi);
END
GO


/* =====================
   PROCEDURE LẤY GIÁ + CÔNG THỨC THEO SIZE
   ===================== */
-- Nếu thủ tục đã tồn tại thì xóa
IF OBJECT_ID('usp_GetSanPhamChiTiet_BySize', 'P') IS NOT NULL
    DROP PROCEDURE usp_GetSanPhamChiTiet_BySize;
GO

-- Tạo mới
CREATE PROCEDURE usp_GetSanPhamChiTiet_BySize
    @MaSP   INT,
    @MaSize INT
AS
BEGIN
    SET NOCOUNT ON;
    -- #1: Thông tin sản phẩm + giá
    SELECT TOP (1)
        sp.MaSP, sp.TenSP,
        sz.MaSize, sz.TenSize,
        sps.Gia
    FROM SanPham sp
    JOIN SanPhamSize sps ON sps.MaSP = sp.MaSP AND sps.MaSize = @MaSize
    JOIN Size sz ON sz.MaSize = @MaSize
    WHERE sp.MaSP = @MaSP;
    -- #2: Nguyên liệu tính theo hệ số size
    SELECT 
        nl.MaNL, nl.TenNL, nl.DonViTinh,
        CAST(ROUND(ct.SoLuongCoSo * sz.HeSoDinhLuong, 2) AS DECIMAL(18,2)) AS SoLuongTheoSize
    FROM CongThucPhaChe ct
    JOIN NguyenLieu nl ON nl.MaNL = ct.MaNL
    JOIN Size sz ON sz.MaSize = @MaSize
    WHERE ct.MaSP = @MaSP
    ORDER BY nl.TenNL;
END
GO
/* =====================
   DỮ LIỆU MẪU
   ===================== */
INSERT INTO NhanVien (HoTen, GioiTinh, NgaySinh, SDT, DiaChi, TenChucVu, Email, MatKhau, VaiTro)
VALUES (N'Nguyễn Văn A', N'Nam', '1999-01-01', '0912345678', N'HCM', N'Quản lý', 'admin@trasua.vn', 'admin123', 'Admin');
INSERT INTO KhachHang (HoTen, Email, MatKhau, SDT, DiemTichLuy)
VALUES (N'Trần Minh C', 'tranminhc@gmail.com', 'matkhau123', '0933456789', 20);
INSERT INTO DanhMucSanPham (TenDM)
VALUES (N'Trà sữa'), (N'Trà trái cây'), (N'Cà phê');
INSERT INTO Size (TenSize, IsBase, HeSoGia, PhuThu, HeSoDinhLuong) VALUES
('S', 1, 1.00, 0, 1.00),
('M', 0, 1.15, 0, 1.30),
('L', 0, 1.25, 0, 1.60);
INSERT INTO SanPham (TenSP, MaDM, Anh, TrangThai)
VALUES 
(N'Trà sữa truyền thống', 1, N'tra_sua_truyen_thong.png', 1),
(N'Trà đào cam sả', 2, N'tra_dao_cam_sa.png', 1),
(N'Trà sữa matcha', 1, N'tra_sua_matcha.png', 1),
(N'Trà vải nhãn', 2, N'tra_vai_nhan.png', 1),
(N'Cà phê sữa đá', 3, N'ca_phe_sua.png', 1),
(N'Cà phê đen', 3, N'ca_phe_den.png', 1);
INSERT INTO SanPhamSize (MaSP, MaSize, Gia)
VALUES
(1, 1, 30000),
(2, 1, 28000),
(3, 1, 32000),
(4, 1, 27000),
(5, 1, 25000),
(6, 1, 20000);
INSERT INTO NguyenLieu (TenNL, DonViTinh)
VALUES 
(N'Trà đen', 'g'),
(N'Trà xanh', 'g'),
(N'Đường', 'g'),
(N'Sữa đặc', 'ml'),
(N'Sữa tươi', 'ml'),
(N'Nước cốt đào', 'ml'),
(N'Đào miếng', 'g'),
(N'Nhãn', 'g'),
(N'Cà phê bột', 'g');
INSERT INTO CongThucPhaChe (MaSP, MaNL, SoLuongCoSo) VALUES
(1, 1, 5),  (1, 3, 10), (1, 4, 20), (1, 5, 50),
(2, 1, 5),  (2, 3, 8),  (2, 6, 30), (2, 7, 50),
(3, 2, 5),  (3, 3, 9),  (3, 4, 15), (3, 5, 40),
(4, 1, 5),  (4, 3, 8),  (4, 6, 25), (4, 8, 40),
(5, 9, 20), (5, 3, 8),  (5, 4, 15),
(6, 9, 25), (6, 3, 6);
INSERT INTO HinhThucThanhToan (TenHinhThuc)
VALUES (N'Tiền mặt'), (N'MOMO'), (N'ZaloPay');
GO
UPDATE SanPham SET Anh = N'milk_tea.jpg'  WHERE TenSP = N'Trà sữa truyền thống';
UPDATE SanPham SET Anh = N'matcha.jpg'    WHERE TenSP = N'Trà sữa matcha';

UPDATE dbo.SanPham
SET Anh = 'tra_bi_dao.jpg'
WHERE TenSP = N'Trà bí đao';

/* =========================================================
   THÊM 30 SẢN PHẨM MỚI + ẢNH + GIÁ BASE (SIZE S)
   (M, L sẽ tự động tính bởi trigger trg_SanPhamSize_AutoPricing)
   ========================================================= */
-- 1) Thêm 30 sản phẩm mới (3 danh mục: 1=Trà sữa, 2=Trà trái cây, 3=Cà phê)
INSERT INTO SanPham (TenSP, MaDM, Anh, TrangThai) VALUES
-- Trà sữa (MaDM = 1)
(N'Trà sữa trân châu',                1, N'tra_sua_tran_chau.jpg',              1),
(N'Trà sữa đường đen',               1, N'tra_sua_duong_den.jpg',              1),
(N'Trà sữa socola',                  1, N'tra_sua_socola.jpg',                 1),
(N'Trà sữa caramel',                 1, N'tra_sua_caramel.jpg',                1),
(N'Trà sữa khoai môn',               1, N'tra_sua_khoai_mon.jpg',              1),
(N'Trà sữa hạnh nhân',               1, N'tra_sua_hanh_nhan.jpg',              1),
(N'Trà sữa dâu tây',                 1, N'tra_sua_dau_tay.jpg',                1),
(N'Trà sữa việt quất',               1, N'tra_sua_viet_quat.jpg',              1),
(N'Trà sữa bạc hà',                  1, N'tra_sua_bac_ha.jpg',                 1),
(N'Trà sữa phô mai kem',             1, N'tra_sua_pho_mai_kem.jpg',            1),

-- Trà trái cây (MaDM = 2)
(N'Trà bí đao',                      2, N'tra_bi_dao.jpg',                     1),
(N'Trà đào nhiệt đới',               2, N'tra_dao_nhiet_doi.jpg',              1),
(N'Trà ổi hồng',                     2, N'tra_oi_hong.jpg',                    1),
(N'Trà chanh sả',                    2, N'tra_chanh_sa.jpg',                   1),
(N'Trà tắc mật ong',                 2, N'tra_tac_mat_ong.jpg',                1),
(N'Trà vải sen',                     2, N'tra_vai_sen.jpg',                    1),
(N'Trà xoài nhiệt đới',              2, N'tra_xoai_nhiet_doi.jpg',             1),
(N'Trà dứa bạc hà',                  2, N'tra_dua_bac_ha.jpg',                 1),
(N'Trà cam quýt',                    2, N'tra_cam_quyt.jpg',                   1),
(N'Trà thanh long',                  2, N'tra_thanh_long.jpg',                 1),

-- Cà phê (MaDM = 3)
(N'Bạc xỉu',                         3, N'bac_xiu.jpg',                         1),
(N'Cappuccino',                      3, N'cappuccino.jpg',                      1),
(N'Latte',                           3, N'latte.jpg',                           1),
(N'Mocha',                           3, N'mocha.jpg',                           1),
(N'Espresso',                        3, N'espresso.jpg',                        1),
(N'Americano',                       3, N'americano.jpg',                       1),
(N'Cold brew cam sả',                3, N'cold_brew_cam_sa.jpg',               1),
(N'Cold brew sữa tươi',              3, N'cold_brew_sua_tuoi.jpg',             1),
(N'Phin sữa nóng',                   3, N'phin_sua_nong.jpg',                   1),
(N'Phin đen nóng',                   3, N'phin_den_nong.jpg',                   1);

-- 2) Gán giá base (Size S = MaSize 1) cho 30 sản phẩm vừa thêm
--    (M = 1.15, L = 1.25 sẽ do trigger tự nhân hệ số và làm tròn 1.000đ)
/* Upsert giá base (Size S = MaSize 1) cho 30 món – không sinh trùng khoá */
MERGE SanPhamSize AS T
USING (
    SELECT sp.MaSP, MaSize = 1,
           CASE sp.TenSP
               -- Trà sữa: 29k–35k
               WHEN N'Trà sữa trân châu'       THEN 32000
               WHEN N'Trà sữa đường đen'      THEN 33000
               WHEN N'Trà sữa socola'         THEN 32000
               WHEN N'Trà sữa caramel'        THEN 33000
               WHEN N'Trà sữa khoai môn'      THEN 32000
               WHEN N'Trà sữa hạnh nhân'      THEN 31000
               WHEN N'Trà sữa dâu tây'        THEN 33000
WHEN N'Trà sữa việt quất'      THEN 34000
               WHEN N'Trà sữa bạc hà'         THEN 30000
               WHEN N'Trà sữa phô mai kem'    THEN 35000

               -- Trà trái cây: 28k–34k
               WHEN N'Trà bí đao'             THEN 28000
               WHEN N'Trà đào nhiệt đới'      THEN 32000
               WHEN N'Trà ổi hồng'            THEN 30000
               WHEN N'Trà chanh sả'           THEN 30000
               WHEN N'Trà tắc mật ong'        THEN 29000
               WHEN N'Trà vải sen'            THEN 32000
               WHEN N'Trà xoài nhiệt đới'     THEN 32000
               WHEN N'Trà dứa bạc hà'         THEN 31000
               WHEN N'Trà cam quýt'           THEN 30000
               WHEN N'Trà thanh long'         THEN 30000

               -- Cà phê: 24k–36k
               WHEN N'Bạc xỉu'                THEN 26000
               WHEN N'Cappuccino'             THEN 34000
               WHEN N'Latte'                  THEN 34000
               WHEN N'Mocha'                  THEN 36000
               WHEN N'Espresso'               THEN 24000
               WHEN N'Americano'              THEN 26000
               WHEN N'Cold brew cam sả'       THEN 34000
               WHEN N'Cold brew sữa tươi'     THEN 34000
               WHEN N'Phin sữa nóng'          THEN 26000
               WHEN N'Phin đen nóng'          THEN 24000
           END AS Gia
    FROM SanPham sp
    WHERE sp.TenSP IN (
        N'Trà sữa trân châu', N'Trà sữa đường đen', N'Trà sữa socola', N'Trà sữa caramel', N'Trà sữa khoai môn',
        N'Trà sữa hạnh nhân', N'Trà sữa dâu tây', N'Trà sữa việt quất', N'Trà sữa bạc hà', N'Trà sữa phô mai kem',
        N'Trà bí đao', N'Trà đào nhiệt đới', N'Trà ổi hồng', N'Trà chanh sả', N'Trà tắc mật ong',
        N'Trà vải sen', N'Trà xoài nhiệt đới', N'Trà dứa bạc hà', N'Trà cam quýt', N'Trà thanh long',
        N'Bạc xỉu', N'Cappuccino', N'Latte', N'Mocha', N'Espresso',
        N'Americano', N'Cold brew cam sả', N'Cold brew sữa tươi', N'Phin sữa nóng', N'Phin đen nóng'
    )
) AS S
ON T.MaSP = S.MaSP AND T.MaSize = S.MaSize
WHEN MATCHED THEN 
    UPDATE SET T.Gia = S.Gia
WHEN NOT MATCHED THEN
    INSERT (MaSP, MaSize, Gia) VALUES (S.MaSP, S.MaSize, S.Gia);

DBCC CHECKIDENT ('DanhMucSanPham', RESEED, 3);
DBCC CHECKIDENT ('SanPham', RESEED, 37);

-- 1) Thêm cột dung tích (ml) cho size
ALTER TABLE dbo.Size ADD DungTichML INT NULL;

-- 2) Gán dung tích chuẩn
UPDATE dbo.Size SET DungTichML = 250 WHERE TenSize = 'S';
UPDATE dbo.Size SET DungTichML = 350 WHERE TenSize = 'M';
UPDATE dbo.Size SET DungTichML = 500 WHERE TenSize = 'L';

-- 3) Bắt buộc phải có dung tích
ALTER TABLE dbo.Size ALTER COLUMN DungTichML INT NOT NULL;

-- Xóa thủ tục cũ nếu đã tồn tại
IF OBJECT_ID('usp_CheckTheTichSanPham', 'P') IS NOT NULL
    DROP PROCEDURE usp_CheckTheTichSanPham;
GO

-- Tạo mới
CREATE PROCEDURE usp_CheckTheTichSanPham
    @MaSP INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @BaseML DECIMAL(18,2) =
    (
        SELECT ISNULL(SUM(ct.SoLuongCoSo),0)
        FROM dbo.CongThucPhaChe ct
        JOIN dbo.NguyenLieu nl ON nl.MaNL = ct.MaNL
        WHERE nl.DonViTinh = 'ml' AND ct.MaSP = @MaSP
    );
    SELECT 
        sz.TenSize,
        DungTich   = sz.DungTichML,
        ML_DaDung  = CAST(ROUND(@BaseML * sz.HeSoDinhLuong, 2) AS DECIMAL(18,2)),
        HopLe      = CASE WHEN (@BaseML * sz.HeSoDinhLuong) <= sz.DungTichML THEN 1 ELSE 0 END
    FROM dbo.Size sz
    ORDER BY sz.TenSize;
END
GO
SELECT  s.name  AS [schema],
        tb.name AS [table],
        t.name  AS [trigger_name],
        t.is_disabled
FROM sys.triggers t
JOIN sys.tables  tb ON t.parent_id = tb.object_id
JOIN sys.schemas s ON tb.schema_id = s.schema_id
WHERE tb.name = 'SanPhamSize';
DISABLE TRIGGER ALL ON [dbo].[SanPhamSize];
select *from NguyenLieu
SELECT * FROM dbo.BinhLuan;
select*from NguyenLieu
select *from BinhLuan
select *from SanPham
select* from SanPhamSize
INSERT INTO HoaDon (NgayLap, TongTien, TrangThai)
VALUES
('2025-08-20', 50000, 'HoanTat'),
('2025-08-20', 70000, 'HoanTat');
INSERT INTO HoaDon (MaKH, MaNV, NgayLap, TongTien, TrangThai)
VALUES (1, 1, '2025-08-20', 120000, 'HoanTat'),
       (1, 1, '2025-08-21', 75000, 'HoanTat');
	   select*from HoaDon
	   ALTER TABLE SanPhamSize
ADD SoLuongTon INT NOT NULL DEFAULT 0;
ALTER TABLE NguyenLieu
ADD SoLuongTon decimal(18,2) NOT NULL DEFAULT 0;
ALTER TABLE HoaDon
ADD TenNguoiNhan     nvarchar(100) NULL,
    DiaChiNhan       nvarchar(255) NULL,
    SoDienThoaiNhan  nvarchar(20)  NULL;
select*from KhachHang
/* 1) Bảng phát sinh kho */
IF OBJECT_ID('dbo.KhoPhatSinh','U') IS NOT NULL
    DROP TABLE dbo.KhoPhatSinh;
GO
CREATE TABLE dbo.KhoPhatSinh (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaNL INT NOT NULL,
    SoLuong DECIMAL(18,2) NOT NULL,   -- + nhập, - xuất
    Loai NVARCHAR(20) NOT NULL,       -- 'Nhap' | 'Xuat'
    RefType NVARCHAR(30) NULL,        -- 'NhapKho' | 'HoaDon'
    RefId INT NULL,                   -- MaNK / MaHD
    GhiChu NVARCHAR(255) NULL,
    Ngay DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_KPS_NL FOREIGN KEY (MaNL) REFERENCES dbo.NguyenLieu(MaNL)
);
CREATE INDEX IX_KPS_MaNL ON dbo.KhoPhatSinh(MaNL);
CREATE INDEX IX_KPS_Ref ON dbo.KhoPhatSinh(RefType, RefId);
/* 2) VIEW tồn hiện tại (SUM phát sinh) */
GO
CREATE VIEW dbo.v_TonKhoHienTai AS
SELECT MaNL, SUM(SoLuong) AS Ton
FROM dbo.KhoPhatSinh
GROUP BY MaNL;
/* 3) VIEW cảnh báo sắp hết (ngưỡng 100 có thể đổi) */
IF OBJECT_ID('dbo.v_NguyenLieuSapHet','V') IS NOT NULL
    DROP VIEW dbo.v_NguyenLieuSapHet;
GO
CREATE VIEW dbo.v_NguyenLieuSapHet AS
SELECT nl.MaNL, nl.TenNL, ISNULL(t.Ton,0) AS Ton
FROM dbo.NguyenLieu nl
LEFT JOIN dbo.v_TonKhoHienTai t ON t.MaNL = nl.MaNL
WHERE ISNULL(t.Ton,0) <= 100;
-- INSERT → +SoLuong
IF OBJECT_ID('dbo.trg_NhapKho_AI','TR') IS NOT NULL DROP TRIGGER dbo.trg_NhapKho_AI;
GO
CREATE TRIGGER dbo.trg_NhapKho_AI ON dbo.NhapKho
AFTER INSERT
AS
BEGIN
  SET NOCOUNT ON;
  INSERT dbo.KhoPhatSinh(MaNL, SoLuong, Loai, RefType, RefId, GhiChu, Ngay)
  SELECT i.MaNL, CAST(i.SoLuong AS DECIMAL(18,2)), N'Nhap', N'NhapKho', i.MaNK, N'Nhập kho', i.NgayNhap
  FROM inserted i;
END
GO
-- UPDATE → chênh lệch (mới - cũ)
IF OBJECT_ID('dbo.trg_NhapKho_AU','TR') IS NOT NULL DROP TRIGGER dbo.trg_NhapKho_AU;
GO
CREATE TRIGGER dbo.trg_NhapKho_AU ON dbo.NhapKho
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;
  INSERT dbo.KhoPhatSinh(MaNL, SoLuong, Loai, RefType, RefId, GhiChu, Ngay)
  SELECT i.MaNL,
         CAST(i.SoLuong - d.SoLuong AS DECIMAL(18,2)) AS Delta,
         N'Nhap', N'NhapKho', i.MaNK,
         N'Hiệu chỉnh phiếu nhập', GETDATE()
  FROM inserted i
  JOIN deleted  d ON d.MaNK = i.MaNK
  WHERE i.SoLuong <> d.SoLuong;
END
GO
-- DELETE → -SoLuong
IF OBJECT_ID('dbo.trg_NhapKho_AD','TR') IS NOT NULL DROP TRIGGER dbo.trg_NhapKho_AD;
GO
CREATE TRIGGER dbo.trg_NhapKho_AD ON dbo.NhapKho
AFTER DELETE
AS
BEGIN
  SET NOCOUNT ON;
  INSERT dbo.KhoPhatSinh(MaNL, SoLuong, Loai, RefType, RefId, GhiChu, Ngay)
  SELECT d.MaNL, CAST(-d.SoLuong AS DECIMAL(18,2)), N'Nhap', N'NhapKho', d.MaNK, N'Xoá phiếu nhập', GETDATE()
  FROM deleted d;
END
GO

IF OBJECT_ID('dbo.usp_XuatKho_KhiHoanTat','P') IS NOT NULL
  DROP PROCEDURE dbo.usp_XuatKho_KhiHoanTat;
GO
CREATE PROCEDURE dbo.usp_XuatKho_KhiHoanTat @MaHD INT
AS
BEGIN
  SET NOCOUNT ON;
  -- Nếu đã trừ kho cho hóa đơn này thì bỏ qua
  IF EXISTS(SELECT 1 FROM dbo.KhoPhatSinh WHERE RefType=N'HoaDon' AND RefId=@MaHD)
    RETURN;
  ;WITH HangXuat AS (
    SELECT 
      nl.MaNL,
      SUM( CAST(ROUND(ct.SoLuongCoSo * sz.HeSoDinhLuong, 2) AS DECIMAL(18,2)) * cthd.SoLuong ) AS SLXuat
    FROM dbo.ChiTietHoaDon cthd
    JOIN dbo.Size            sz ON sz.MaSize = cthd.MaSize
    JOIN dbo.CongThucPhaChe  ct ON ct.MaSP   = cthd.MaSP
    JOIN dbo.NguyenLieu     nl  ON nl.MaNL   = ct.MaNL
    WHERE cthd.MaHD = @MaHD
    GROUP BY nl.MaNL
  )
  INSERT dbo.KhoPhatSinh(MaNL, SoLuong, Loai, RefType, RefId, GhiChu, Ngay)
  SELECT MaNL, -SLXuat, N'Xuat', N'HoaDon', @MaHD, N'Xuất khi đơn hoàn tất', GETDATE()
  FROM HangXuat
  WHERE SLXuat > 0;
END
GO
IF OBJECT_ID('dbo.usp_HoanTacXuatKho_HoaDon','P') IS NOT NULL
  DROP PROCEDURE dbo.usp_HoanTacXuatKho_HoaDon;
GO
CREATE PROCEDURE dbo.usp_HoanTacXuatKho_HoaDon @MaHD INT
AS
BEGIN
  SET NOCOUNT ON;
  -- chỉ khi đã từng trừ kho
  IF NOT EXISTS (SELECT 1 FROM dbo.KhoPhatSinh WHERE RefType=N'HoaDon' AND RefId=@MaHD) RETURN;
  -- Cộng bù đúng lượng đã xuất (đảo dấu)
  INSERT dbo.KhoPhatSinh(MaNL, SoLuong, Loai, RefType, RefId, GhiChu, Ngay)
  SELECT MaNL, -SoLuong, N'Nhap', N'HoaDon', @MaHD, N'Hoàn tác xuất kho', GETDATE()
  FROM dbo.KhoPhatSinh
  WHERE RefType=N'HoaDon' AND RefId=@MaHD AND SoLuong<0;
END
GO
IF OBJECT_ID('dbo.v_TonKhoBangTongHop','V') IS NOT NULL
  DROP VIEW dbo.v_TonKhoBangTongHop;
GO
CREATE VIEW dbo.v_TonKhoBangTongHop AS
SELECT nl.MaNL, nl.TenNL, nl.DonViTinh, ISNULL(t.Ton,0) AS Ton
FROM dbo.NguyenLieu nl
LEFT JOIN (
  SELECT MaNL, SUM(SoLuong) AS Ton
  FROM dbo.KhoPhatSinh
  GROUP BY MaNL
) t ON t.MaNL = nl.MaNL;
GO
ALTER TABLE BinhLuan
ADD PhanHoi NVARCHAR(MAX) NULL,
    NgayPhanHoi DATETIME NULL;
