-- TẠO DATABASE (nếu cần)
IF DB_ID('QuanLyBanHang') IS NULL
    CREATE DATABASE QuanLyBanHang;
GO
USE QuanLyBanHang;
GO

/* =========================
   BẢNG GỐC / DANH MỤC
   ========================= */

-- Tài khoản (nếu bạn cần đăng nhập)
IF OBJECT_ID('dbo.TAIKHOAN','U') IS NULL
CREATE TABLE dbo.TAIKHOAN (
    TenTaiKhoan NVARCHAR(50)  NOT NULL PRIMARY KEY,
    MatKhau     NVARCHAR(100) NOT NULL
);

-- Thông tin đại lý / khách hàng
IF OBJECT_ID('dbo.THONGTINDAILY','U') IS NULL
CREATE TABLE dbo.THONGTINDAILY (
    MaDaiLy      INT IDENTITY(1,1) PRIMARY KEY,
    TenDaiLy     NVARCHAR(100) NOT NULL,
    DiaChi       NVARCHAR(255) NULL,
    SoDienThoai  NVARCHAR(20)  NULL
);

-- Sản phẩm
IF OBJECT_ID('dbo.SANPHAM','U') IS NULL
CREATE TABLE dbo.SANPHAM (
    MaSanPham   INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham  NVARCHAR(100) NOT NULL UNIQUE,
    DonVi       NVARCHAR(50)  NULL,
    Gia         DECIMAL(18,2) NOT NULL CHECK (Gia >= 0),
    SoLuongTon  INT           NOT NULL DEFAULT 0 CHECK (SoLuongTon >= 0)
);
GO

/* =========================
   HÓA ĐƠN BÁN + CHI TIẾT
   ========================= */

-- Hóa đơn bán hàng (header)
IF OBJECT_ID('dbo.HOADONBANHANG','U') IS NULL
CREATE TABLE dbo.HOADONBANHANG (
    MaHoaDon     INT IDENTITY(1,1) PRIMARY KEY,
    NgayNhap     DATE             NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    MaKhachHang  INT              NOT NULL,      -- FK -> THONGTINDAILY
    TenCuaHang   NVARCHAR(100)    NULL,
    TenKhachHang NVARCHAR(100)    NULL,          -- lưu tên hiển thị (không bắt buộc)
    TongTien     DECIMAL(18,2)    NULL,          -- có thể tính lại từ chi tiết
    TongNo       DECIMAL(18,2)    NULL DEFAULT 0 CHECK (TongNo >= 0),

    CONSTRAINT FK_HDBH_DaiLy
        FOREIGN KEY (MaKhachHang) REFERENCES dbo.THONGTINDAILY(MaDaiLy)
        ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Chi tiết hóa đơn bán (nhiều sản phẩm trong 1 hóa đơn)
IF OBJECT_ID('dbo.CHITIETHOADONBAN','U') IS NULL
CREATE TABLE dbo.CHITIETHOADONBAN (
    MaCTBan     INT IDENTITY(1,1) PRIMARY KEY,
    MaHoaDon    INT           NOT NULL,                   -- FK -> HOADONBANHANG
    MaSanPham   INT           NOT NULL,                   -- FK -> SANPHAM
    SoLuong     INT           NOT NULL CHECK (SoLuong > 0),
    DonGia      DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    ThanhTien   AS (SoLuong * DonGia) PERSISTED,         -- cột tính

    CONSTRAINT FK_CTBan_HoaDon FOREIGN KEY (MaHoaDon)
        REFERENCES dbo.HOADONBANHANG(MaHoaDon)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_CTBan_SanPham FOREIGN KEY (MaSanPham)
        REFERENCES dbo.SANPHAM(MaSanPham)
        ON UPDATE NO ACTION ON DELETE NO ACTION
);
GO

/* =========================
   HÓA ĐƠN MUA + CHI TIẾT
   ========================= */

-- Hóa đơn mua hàng (nhập hàng)
IF OBJECT_ID('dbo.HOADONMUAHANG','U') IS NULL
CREATE TABLE dbo.HOADONMUAHANG (
    MaHoaDon   INT IDENTITY(1,1) PRIMARY KEY,
    NgayNhap   DATE          NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    NhaCungCap NVARCHAR(100) NULL,     -- tùy chọn có bảng NCC riêng
    TongTien   DECIMAL(18,2) NULL
);

-- Chi tiết hóa đơn mua
IF OBJECT_ID('dbo.CHITIETHOADONMUA','U') IS NULL
CREATE TABLE dbo.CHITIETHOADONMUA (
    MaCTMua    INT IDENTITY(1,1) PRIMARY KEY,
    MaHoaDon   INT           NOT NULL,                      -- FK -> HOADONMUAHANG
    MaSanPham  INT           NOT NULL,                      -- FK -> SANPHAM
    SoLuong    INT           NOT NULL CHECK (SoLuong > 0),
    DonGia     DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    ThanhTien  AS (SoLuong * DonGia) PERSISTED,

    CONSTRAINT FK_CTMua_HoaDon FOREIGN KEY (MaHoaDon)
        REFERENCES dbo.HOADONMUAHANG(MaHoaDon)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_CTMua_SanPham FOREIGN KEY (MaSanPham)
        REFERENCES dbo.SANPHAM(MaSanPham)
        ON UPDATE NO ACTION ON DELETE NO ACTION
);
GO

/* =========================
   CHỈ MỤC GỢI Ý
   ========================= */
CREATE INDEX IX_HDBH_Ngay ON dbo.HOADONBANHANG(NgayNhap);
CREATE INDEX IX_HDBH_Khach ON dbo.HOADONBANHANG(MaKhachHang);
CREATE INDEX IX_CTBan_HoaDon ON dbo.CHITIETHOADONBAN(MaHoaDon);
CREATE INDEX IX_CTBan_SanPham ON dbo.CHITIETHOADONBAN(MaSanPham);

CREATE INDEX IX_HDMH_Ngay ON dbo.HOADONMUAHANG(NgayNhap);
CREATE INDEX IX_CTMua_HoaDon ON dbo.CHITIETHOADONMUA(MaHoaDon);
CREATE INDEX IX_CTMua_SanPham ON dbo.CHITIETHOADONMUA(MaSanPham);
GO





DROP INDEX IX_HDBH_Ngay ON dbo.HOADONBANHANG;
GO

CREATE INDEX IX_HDBH_Ngay ON dbo.HOADONBANHANG(NgayNhap);
GO
