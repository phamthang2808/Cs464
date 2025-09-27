using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Windows.Controls;
using System.Data.Entity.Validation;
using System.Globalization; // Thêm namespace để xử lý định dạng số

namespace FE
{
    public partial class Window_ChiTietHoaDon : Window
    {
        private string maHoaDon;
        private HOADON currentHoaDon;
        private List<CHITIETHOADON> dsChiTiet;

        public Window_ChiTietHoaDon()
        {
            InitializeComponent();
            LoadSanPham();
        }

        // KHẮC PHỤC LỖI: BỎ InitializeComponent() ở đây
        public Window_ChiTietHoaDon(string maHD) : this()
        {
            maHoaDon = maHD;
            LoadHoaDonVaChiTiet();
        }

        private decimal TinhTongTien(List<CHITIETHOADON> chiTietHoaDons)
        {
            return chiTietHoaDons.Sum(ct => ct.ThanhTien);
        }

        private void LoadSanPham()
        {
            using (var db = new QL_SP_Entities1())
            {
                var dsSP = db.SANPHAM.ToList();
                cbTenMatHang.ItemsSource = dsSP;
            }
        }

        private void LoadHoaDonVaChiTiet()
        {
            using (var db = new QL_SP_Entities1())
            {
                currentHoaDon = db.HOADON
                                     .Include(h => h.CHITIETHOADON.Select(ct => ct.SANPHAM))
                                     .FirstOrDefault(h => h.MaHoaDon == maHoaDon);

                if (currentHoaDon == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                dsChiTiet = currentHoaDon.CHITIETHOADON.ToList();

                decimal tongTienMoi = TinhTongTien(dsChiTiet);

                if (currentHoaDon.TongTien != tongTienMoi)
                {
                    currentHoaDon.TongTien = tongTienMoi;
                    db.SaveChanges();
                }

                txtMaHoaDon.Text = currentHoaDon.MaHoaDon;
                dtpNgayLap.SelectedDate = currentHoaDon.NgayLap;
                txtTongTien.Text = tongTienMoi.ToString("N0") ?? "0";

                dgChiTietHoaDon.ItemsSource = dsChiTiet;
                dgChiTietHoaDon.Items.Refresh();
            }
        }

        private void CapNhatTongTien()
        {
            using (var db = new QL_SP_Entities1())
            {
                var chiTietMoiList = db.CHITIETHOADON
                                           .Where(ct => ct.MaHoaDon == maHoaDon)
                                           .ToList();

                var hoaDonDeSua = db.HOADON.First(h => h.MaHoaDon == maHoaDon);

                hoaDonDeSua.TongTien = TinhTongTien(chiTietMoiList);

                db.SaveChanges();
            }
        }

        // KHẮC PHỤC LỖI: Đảm bảo Đơn giá và Đơn vị tính được tự động điền
        private void CbTenMatHang_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Nếu currentHoaDon là null thì không thể xác định LoaiHoaDon, sẽ không điền giá
            if (cbTenMatHang.SelectedItem is SANPHAM selectedSanPham && currentHoaDon != null)
            {
                txtDonViTinh.Text = selectedSanPham.DonVi ?? "";

                Nullable<decimal> donGiaTuDong = null;

                string loaiHoaDon = currentHoaDon.LoaiHoaDon?.ToUpper();

                if (loaiHoaDon == "BÁN")
                {
                    donGiaTuDong = selectedSanPham.GiaBan;
                }
                else if (loaiHoaDon == "MUA")
                {
                    donGiaTuDong = selectedSanPham.GiaMua;
                }

                // Hiển thị giá, sử dụng CultureInfo.InvariantCulture để tránh lỗi phân cách thập phân
                // Nếu giá là null, điền "0"
                txtGia.Text = donGiaTuDong?.ToString(CultureInfo.InvariantCulture) ?? "0";
            }
            else
            {
                txtDonViTinh.Text = "";
                txtGia.Text = "";
            }
        }

        private void btnThemChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (cbTenMatHang.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtSoLuong.Text) ||
                string.IsNullOrWhiteSpace(txtGia.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin mặt hàng (Tên, Số lượng, Giá).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // KHẮC PHỤC LỖI: Sử dụng TryParse với CultureInfo để xử lý dấu chấm/phẩy chính xác
            if (!decimal.TryParse(txtGia.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal donGia) || donGia <= 0)
            {
                MessageBox.Show("Giá phải là số lớn hơn 0.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new QL_SP_Entities1())
            {
                try
                {
                    string maSP = cbTenMatHang.SelectedValue.ToString();
                    var sanPham = db.SANPHAM.FirstOrDefault(sp => sp.MaSanPham == maSP);

                    if (sanPham == null)
                    {
                        MessageBox.Show("Sản phẩm không tồn tại trong cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var chiTietHienCo = db.CHITIETHOADON
                        .FirstOrDefault(ct => ct.MaHoaDon == maHoaDon && ct.MaSanPham == maSP);

                    int soLuongDaCo = chiTietHienCo?.SoLuong ?? 0;
                    int tongSoLuongMoi = soLuongDaCo + soLuong;

                    // KIỂM TRA TỒN KHO: Chỉ áp dụng cho hóa đơn BÁN (xuất kho)
                    if (currentHoaDon?.LoaiHoaDon?.ToUpper() == "BAN" && sanPham.SoLuongTon.HasValue && tongSoLuongMoi > sanPham.SoLuongTon.Value)
                    {
                        MessageBox.Show($"Số lượng vượt quá tồn kho. Tồn kho hiện tại: {sanPham.SoLuongTon}. Tổng số lượng sẽ là: {tongSoLuongMoi}. Vui lòng nhập số lượng nhỏ hơn hoặc bằng.", "Lỗi tồn kho", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (chiTietHienCo != null)
                    {
                        // Nếu đã tồn tại, cập nhật
                        chiTietHienCo.SoLuong = tongSoLuongMoi;
                        chiTietHienCo.DonGia = donGia;
                    }
                    else
                    {
                        // Thêm chi tiết mới
                        var chiTietMoi = new CHITIETHOADON()
                        {
                            MaChiTietHoaDon = Guid.NewGuid().ToString(),
                            MaHoaDon = maHoaDon,
                            MaSanPham = sanPham.MaSanPham,
                            SoLuong = soLuong,
                            DonGia = donGia
                        };
                        db.CHITIETHOADON.Add(chiTietMoi);
                    }

                    db.SaveChanges();

                    CapNhatTongTien();

                    MessageBox.Show("Thêm/Cập nhật chi tiết sản phẩm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadHoaDonVaChiTiet();
                    XoaONhapLieu();
                }
                // Thêm xử lý lỗi DbEntityValidationException để xem chi tiết lỗi ràng buộc
                catch (DbEntityValidationException ex)
                {
                    var errorMessage = "Lỗi xác thực dữ liệu:\n";
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            errorMessage += $"- Thuộc tính: {validationError.PropertyName}, Lỗi: {validationError.ErrorMessage}\n";
                        }
                    }
                    MessageBox.Show("Lỗi khi thêm chi tiết sản phẩm:\n" + errorMessage, "Lỗi Xác Thực", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm chi tiết sản phẩm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void btnSuaChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (dgChiTietHoaDon.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chi tiết sản phẩm cần sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbTenMatHang.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtSoLuong.Text) ||
                string.IsNullOrWhiteSpace(txtGia.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin mặt hàng (Tên, Số lượng, Giá).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // KHẮC PHỤC LỖI: Sử dụng TryParse với CultureInfo để xử lý dấu chấm/phẩy chính xác
            if (!decimal.TryParse(txtGia.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal donGia) || donGia <= 0)
            {
                MessageBox.Show("Giá phải là số lớn hơn 0.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var chiTietDaChon = (CHITIETHOADON)dgChiTietHoaDon.SelectedItem;

            using (var db = new QL_SP_Entities1())
            {
                try
                {
                    var entity = db.CHITIETHOADON.FirstOrDefault(ct => ct.MaChiTietHoaDon == chiTietDaChon.MaChiTietHoaDon);
                    if (entity == null)
                    {
                        MessageBox.Show("Không tìm thấy chi tiết sản phẩm để sửa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string maSPMoi = cbTenMatHang.SelectedValue.ToString();
                    var sanPhamMoi = db.SANPHAM.FirstOrDefault(sp => sp.MaSanPham == maSPMoi);

                    if (sanPhamMoi == null)
                    {
                        MessageBox.Show("Sản phẩm mới không tồn tại trong cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int soLuongCu = entity.SoLuong ?? 0;
                    int soLuongThayDoi = soLuong - soLuongCu;

                    // KIỂM TRA TỒN KHO KHI SỬA: Chỉ áp dụng cho hóa đơn BÁN
                    if (currentHoaDon?.LoaiHoaDon?.ToUpper() == "BAN" && sanPhamMoi.SoLuongTon.HasValue && (sanPhamMoi.SoLuongTon.Value - soLuongThayDoi) < 0)
                    {
                        MessageBox.Show($"Số lượng sau khi sửa sẽ vượt quá tồn kho. Tồn kho hiện tại: {sanPhamMoi.SoLuongTon}. Vui lòng nhập số lượng nhỏ hơn hoặc bằng.", "Lỗi tồn kho", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Cập nhật thông tin chi tiết hóa đơn
                    entity.MaSanPham = sanPhamMoi.MaSanPham;
                    entity.SoLuong = soLuong;
                    entity.DonGia = donGia;

                    db.SaveChanges();

                    CapNhatTongTien();

                    MessageBox.Show("Sửa chi tiết sản phẩm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadHoaDonVaChiTiet();
                    XoaONhapLieu();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessage = "Lỗi xác thực dữ liệu:\n";
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            errorMessage += $"- Thuộc tính: {validationError.PropertyName}, Lỗi: {validationError.ErrorMessage}\n";
                        }
                    }
                    MessageBox.Show("Lỗi khi sửa chi tiết sản phẩm:\n" + errorMessage, "Lỗi Xác Thực", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa chi tiết sản phẩm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void btnXoaChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (dgChiTietHoaDon.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chi tiết sản phẩm cần xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var chiTiet = (CHITIETHOADON)dgChiTietHoaDon.SelectedItem;

            if (MessageBox.Show("Bạn có chắc muốn xóa chi tiết này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new QL_SP_Entities1())
                {
                    try
                    {
                        var entity = db.CHITIETHOADON.FirstOrDefault(ct => ct.MaChiTietHoaDon == chiTiet.MaChiTietHoaDon);
                        if (entity != null)
                        {
                            db.CHITIETHOADON.Remove(entity);
                            db.SaveChanges();

                            CapNhatTongTien();

                            MessageBox.Show("Xóa chi tiết sản phẩm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadHoaDonVaChiTiet();
                            XoaONhapLieu();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy chi tiết để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa chi tiết sản phẩm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        private void XoaONhapLieu()
        {
            cbTenMatHang.SelectedIndex = -1;
            txtSoLuong.Text = "";
            txtDonViTinh.Text = "";
            txtGia.Text = "";
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DgChiTietHoaDon_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgChiTietHoaDon.SelectedItem is CHITIETHOADON selected)
            {
                // Gán SelectedValue để kích hoạt CbTenMatHang_SelectionChanged
                cbTenMatHang.SelectedValue = selected.MaSanPham;

                txtSoLuong.Text = selected.SoLuong?.ToString() ?? "";

                // Ghi đè lại Đơn giá và Đơn vị tính theo giá đã lưu trong chi tiết hóa đơn
                txtDonViTinh.Text = selected.SANPHAM?.DonVi ?? "";
                txtGia.Text = selected.DonGia?.ToString(CultureInfo.InvariantCulture) ?? "";
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            XoaONhapLieu();
            dgChiTietHoaDon.SelectedIndex = -1; // Bỏ chọn dòng đang chọn trong DataGrid (nếu có)
        }

    }
}