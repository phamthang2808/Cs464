using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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

        public Window_ChiTietHoaDon(string maHD) : this()
        {
            maHoaDon = maHD;
            LoadHoaDonVaChiTiet();
        }

        private decimal TinhTongTien(List<CHITIETHOADON> chiTietHoaDons)
        {
            return chiTietHoaDons.Sum(ct => (ct.SoLuong ?? 0) * (ct.DonGia ?? 0));
        }


        private void LoadSanPham()
        {
            using (var db = new QL_SP_Entities1())
            {
                var dsSP = db.SANPHAM.ToList();
                cbTenMatHang.ItemsSource = dsSP;
                cbTenMatHang.DisplayMemberPath = "TenSanPham";
                cbTenMatHang.SelectedValuePath = "MaSanPham";
            }
        }

        private void LoadHoaDonVaChiTiet()
        {
            using (var db = new QL_SP_Entities1())
            {
                currentHoaDon = db.HOADON
                                  .Include("CHITIETHOADON.SANPHAM")
                                  .FirstOrDefault(h => h.MaHoaDon == maHoaDon);

                if (currentHoaDon == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                dsChiTiet = currentHoaDon.CHITIETHOADON.ToList();

                txtMaHoaDon.Text = currentHoaDon.MaHoaDon;
                dtpNgayLap.SelectedDate = currentHoaDon.NgayLap;
                txtTongTien.Text = currentHoaDon.TongTien?.ToString("N0") ?? "0";

                dgChiTietHoaDon.ItemsSource = dsChiTiet;
            }
        }

        private void btnThemChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (cbTenMatHang.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtSoLuong.Text) ||
                string.IsNullOrWhiteSpace(txtDonViTinh.Text) ||
                string.IsNullOrWhiteSpace(txtGia.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin mặt hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtGia.Text, out decimal donGia) || donGia <= 0)
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

                    var chiTietMoi = new CHITIETHOADON()
                    {
                        MaChiTietHoaDon = Guid.NewGuid().ToString(),
                        MaHoaDon = maHoaDon,
                        MaSanPham = sanPham.MaSanPham,
                        SoLuong = soLuong,
                        DonGia = donGia
                    };

                    db.CHITIETHOADON.Add(chiTietMoi);

                    currentHoaDon = db.HOADON.First(h => h.MaHoaDon == maHoaDon);

                    // Lấy lại danh sách chi tiết sau khi thêm mới
                    var chiTietMoiList = db.CHITIETHOADON.Where(ct => ct.MaHoaDon == maHoaDon).ToList();

                    // Tính lại tổng tiền bằng hàm TinhTongTien
                    currentHoaDon.TongTien = TinhTongTien(chiTietMoiList);

                    db.SaveChanges();

                    MessageBox.Show("Thêm chi tiết sản phẩm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadHoaDonVaChiTiet();
                    XoaONhapLieu();
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
                string.IsNullOrWhiteSpace(txtDonViTinh.Text) ||
                string.IsNullOrWhiteSpace(txtGia.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin mặt hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtGia.Text, out decimal donGia) || donGia <= 0)
            {
                MessageBox.Show("Giá phải là số lớn hơn 0.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var chiTiet = (CHITIETHOADON)dgChiTietHoaDon.SelectedItem;

            using (var db = new QL_SP_Entities1())
            {
                try
                {
                    var entity = db.CHITIETHOADON.FirstOrDefault(ct => ct.MaChiTietHoaDon == chiTiet.MaChiTietHoaDon);
                    if (entity == null)
                    {
                        MessageBox.Show("Không tìm thấy chi tiết sản phẩm để sửa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string maSP = cbTenMatHang.SelectedValue.ToString();

                    var sanPhamMoi = db.SANPHAM.FirstOrDefault(sp => sp.MaSanPham == maSP);
                    if (sanPhamMoi == null)
                    {
                        MessageBox.Show("Sản phẩm không tồn tại trong cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    entity.MaSanPham = sanPhamMoi.MaSanPham;
                    entity.SoLuong = soLuong;
                    entity.DonGia = donGia;

                    currentHoaDon = db.HOADON.First(h => h.MaHoaDon == maHoaDon);

                    // Lấy lại danh sách chi tiết sau khi sửa
                    var chiTietMoiList = db.CHITIETHOADON.Where(ct => ct.MaHoaDon == maHoaDon).ToList();

                    // Tính lại tổng tiền
                    currentHoaDon.TongTien = TinhTongTien(chiTietMoiList);

                    db.SaveChanges();

                    MessageBox.Show("Sửa chi tiết sản phẩm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadHoaDonVaChiTiet();
                    XoaONhapLieu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa chi tiết sản phẩm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

                            currentHoaDon = db.HOADON.First(h => h.MaHoaDon == maHoaDon);

                            // Lấy lại danh sách chi tiết sau khi xóa
                            var chiTietMoiList = db.CHITIETHOADON.Where(ct => ct.MaHoaDon == maHoaDon).ToList();

                            // Tính lại tổng tiền
                            currentHoaDon.TongTien = TinhTongTien(chiTietMoiList);

                            db.SaveChanges();

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


        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DgChiTietHoaDon_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgChiTietHoaDon.SelectedItem is CHITIETHOADON selected)
            {
                // Gán ComboBox theo MaSanPham
                cbTenMatHang.SelectedValue = selected.MaSanPham;

                txtSoLuong.Text = selected.SoLuong?.ToString() ?? "";
                txtDonViTinh.Text = selected.SANPHAM?.DonVi ?? "";
                txtGia.Text = selected.DonGia?.ToString("N0") ?? "";
            }
        }
    }
}
