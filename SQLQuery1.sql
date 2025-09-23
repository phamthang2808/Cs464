-- Tạo bảng THONG TIN DAI LY
CREATE TABLE THONGTINDAILY (
    MaDaiLy NVARCHAR(50) PRIMARY KEY,
    TenDaiLy NVARCHAR(255),
    DiaChi NVARCHAR(255),
    SoDienThoai NVARCHAR(20)
);

-- Tạo bảng TAI KHOAN
-- Liên kết với THONG TIN DAI LY
CREATE TABLE TAIKHOAN (
    TenTaiKhoan NVARCHAR(50) PRIMARY KEY,
    MatKhau NVARCHAR(255) NOT NULL,
    MaDaiLy NVARCHAR(50),
    FOREIGN KEY (MaDaiLy) REFERENCES THONGTINDAILY(MaDaiLy)
);

-- Tạo bảng SAN PHAM
CREATE TABLE SANPHAM (
    MaSanPham NVARCHAR(50) PRIMARY KEY,
    TenSanPham NVARCHAR(255) NOT NULL,
    SoLuongTon INT CHECK (SoLuongTon >= 0),
    DonVi NVARCHAR(50),
    GiaBan DECIMAL(18, 2) CHECK (GiaBan >= 0)
);

-- Tạo bảng HOA DON
-- Liên kết với THONG TIN DAI LY
CREATE TABLE HOADON (
    MaHoaDon NVARCHAR(50) PRIMARY KEY,
    NgayLap DATE,
    TongTien DECIMAL(18, 2) CHECK (TongTien >= 0),
    LoaiHoaDon NVARCHAR(50) NOT NULL, -- Ví dụ: 'BanHang' hoặc 'MuaHang'
    MaDaiLy NVARCHAR(50),
    FOREIGN KEY (MaDaiLy) REFERENCES THONGTINDAILY(MaDaiLy)
);

-- Tạo bảng CHI TIET HOA DON
-- Liên kết với HOA DON và SAN PHAM
CREATE TABLE CHITIETHOADON (
    MaChiTietHoaDon INT PRIMARY KEY IDENTITY(1,1),
    MaHoaDon NVARCHAR(50),
    MaSanPham NVARCHAR(50),
    SoLuong INT CHECK (SoLuong > 0),
    DonGia DECIMAL(18, 2) CHECK (DonGia >= 0),
    FOREIGN KEY (MaHoaDon) REFERENCES HOADON(MaHoaDon),
    FOREIGN KEY (MaSanPham) REFERENCES SANPHAM(MaSanPham)
);

INSERT INTO THONGTINDAILY (MaDaiLy, TenDaiLy, DiaChi, SoDienThoai)
VALUES
    ('DL001', N'Đại Lý Minh Tâm', N'123 Đường Nguyễn Huệ, Quận 1, TP.HCM', '0901234567'),
    ('DL002', N'Đại Lý Hoàng Yến', N'456 Đường Lê Lợi, Quận 1, TP.HCM', '0917654321');

INSERT INTO TAIKHOAN (TenTaiKhoan, MatKhau, MaDaiLy)
VALUES
    ('minhtam', 'matkhau123', 'DL001'),
    ('hoangyen', 'matkhau456', 'DL002');

INSERT INTO SANPHAM (MaSanPham, TenSanPham, SoLuongTon, DonVi, GiaBan)
VALUES
    ('SP001', N'Áo thun nam', 150, N'Cái', 120000.00),
    ('SP002', N'Quần jean nữ', 80, N'Cái', 350000.00),
    ('SP003', N'Váy maxi hoa', 65, N'Cái', 480000.00),
    ('SP004', N'Áo sơ mi trắng', 200, N'Cái', 250000.00),
    ('SP005', N'Quần short kaki', 100, N'Cái', 180000.00);

INSERT INTO HOADON (MaHoaDon, NgayLap, TongTien, LoaiHoaDon, MaDaiLy)
VALUES
    ('HD001', '2025-09-20', 1170000.00, N'BanHang', 'DL001'),
    ('HD002', '2025-09-21', 650000.00, N'MuaHang', 'DL002');

INSERT INTO CHITIETHOADON (MaHoaDon, MaSanPham, SoLuong, DonGia)
VALUES
    ('HD001', 'SP001', 3, 120000.00),
    ('HD001', 'SP002', 2, 350000.00),
    ('HD002', 'SP004', 1, 250000.00),
    ('HD002', 'SP005', 2, 180000.00);