-- Bước 1: (Tùy chọn) Xóa các bảng cũ nếu chúng tồn tại
-- Thứ tự xóa rất quan trọng để tránh lỗi khóa ngoại.
DROP TABLE IF EXISTS CHITIETHOADON;
DROP TABLE IF EXISTS HOADON;
DROP TABLE IF EXISTS SANPHAM;
DROP TABLE IF EXISTS THONGTINDAILY;
DROP TABLE IF EXISTS TAIKHOAN;


-- Bước 2: Tạo lại các bảng theo đúng sơ đồ mới

-- Tạo bảng TAIKHOAN (Độc lập)
CREATE TABLE TAIKHOAN (
    TenTaiKhoan VARCHAR(50) PRIMARY KEY,
    MatKhau NVARCHAR(255)
);

-- Tạo bảng THONGTINDAILY (Không còn liên kết với TAIKHOAN)
CREATE TABLE THONGTINDAILY (
    MaDaily INT PRIMARY KEY IDENTITY(1,1),
    TenDaily NVARCHAR(255),
    DiaChi NVARCHAR(255),
    SoDienThoai VARCHAR(20)
);

-- Tạo bảng SANPHAM
CREATE TABLE SANPHAM (
    MaSanPham VARCHAR(50) PRIMARY KEY,
    TenSanPham NVARCHAR(255),
    SoLuongTon INT,
    DonVi NVARCHAR(50),
    GiaBan DECIMAL(18, 2)
);

-- Tạo bảng HOADON
CREATE TABLE HOADON (
    MaHoaDon INT PRIMARY KEY IDENTITY(1,1),
    NgayLap DATE,
    TongTien DECIMAL(18, 2),
    LoaiHoaDon NVARCHAR(50),
    MaDaily INT,
    FOREIGN KEY (MaDaily) REFERENCES THONGTINDAILY(MaDaily)
);

-- Tạo bảng CHITIETHOADON
CREATE TABLE CHITIETHOADON (
    MaChiTietHoaDon INT PRIMARY KEY IDENTITY(1,1),
    MaHoaDon INT,
    MaSanPham VARCHAR(50),
    SoLuong INT,
    DonGia DECIMAL(18, 2),
    FOREIGN KEY (MaHoaDon) REFERENCES HOADON(MaHoaDon),
    FOREIGN KEY (MaSanPham) REFERENCES SANPHAM(MaSanPham)
);

INSERT INTO TAIKHOAN (TenTaiKhoan, MatKhau)
VALUES
('admin', '123456'),
('nhanvien', 'matkhau123'),
('nguyenvana', 'password01');

INSERT INTO THONGTINDAILY (TenDaily, DiaChi, SoDienThoai)
VALUES
(N'Cửa Hàng Áo Quần T-Fashion', N'123 Đường Nguyễn Trãi, Q.1, TP.HCM', '0901234567'),
(N'Shop Quần Áo Đẹp', N'456 Đường Lê Lợi, Q. Bình Thạnh, TP.HCM', '0912345678');

INSERT INTO SANPHAM (MaSanPham, TenSanPham, SoLuongTon, DonVi, GiaBan)
VALUES
('SP001', N'Áo phông nam Cotton', 150, N'Cái', 120000.00),
('SP002', N'Quần jeans nữ Slimfit', 80, N'Cái', 350000.00),
('SP003', N'Váy suông họa tiết', 65, N'Cái', 280000.00),
('SP004', N'Áo khoác bomber', 40, N'Cái', 450000.00);

-- Hóa đơn thuộc về đại lý có MaDaily = 1 (T-Fashion)
INSERT INTO HOADON (NgayLap, TongTien, LoaiHoaDon, MaDaily)
VALUES
('2025-09-24', 980000.00, N'Bán', 1),
('2025-09-23', 500000.00, N'Nhập', 1);

-- Hóa đơn thuộc về đại lý có MaDaily = 2 (Shop Quần Áo Đẹp)
INSERT INTO HOADON (NgayLap, TongTien, LoaiHoaDon, MaDaily)
VALUES
('2025-09-24', 630000.00, N'Bán', 2);

-- Chi tiết cho Hóa đơn 1 (MaHoaDon = 1)
INSERT INTO CHITIETHOADON (MaHoaDon, MaSanPham, SoLuong, DonGia)
VALUES
(1, 'SP002', 1, 350000.00), -- 1 Quần jeans
(1, 'SP003', 1, 280000.00), -- 1 Váy suông
(1, 'SP001', 3, 120000.00);  -- 3 Áo phông

-- Chi tiết cho Hóa đơn 2 (MaHoaDon = 2)
INSERT INTO CHITIETHOADON (MaHoaDon, MaSanPham, SoLuong, DonGia)
VALUES
(2, 'SP004', 10, 400000.00); -- 10 Áo khoác nhập với giá 400k/cái

-- Chi tiết cho Hóa đơn 3 (MaHoaDon = 3)
INSERT INTO CHITIETHOADON (MaHoaDon, MaSanPham, SoLuong, DonGia)
VALUES
(3, 'SP001', 5, 120000.00), -- 5 Áo phông
(3, 'SP002', 1, 350000.00);  -- 1 Quần jeans