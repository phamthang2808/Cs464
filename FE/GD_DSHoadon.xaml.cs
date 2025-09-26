using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using Microsoft.VisualBasic;

namespace FE
{
    public partial class GD_DSHoadon : UserControl
    {
        QL_SP_Entities1 db = new QL_SP_Entities1();

        public GD_DSHoadon()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadHD();
            loadDaiLy();
            loadLoaiHoaDon();
        }

        private void loadHD()
        {
            dgHoaDon.ItemsSource = db.HOADON
                .Include(h => h.THONGTINDAILY)
                .ToList();
        }

        private void loadDaiLy()
        {
            cboTenDaiLy.ItemsSource = db.THONGTINDAILY.ToList();
            cboTenDaiLy.DisplayMemberPath = "TenDaily";
            cboTenDaiLy.SelectedValuePath = "MaDaily";
            cboTenDaiLy.SelectedIndex = -1;
        }

        private void loadLoaiHoaDon()
        {
            cboLoaiHoaDon.ItemsSource = new List<string> { "Mua", "Bán" };
            cboLoaiHoaDon.SelectedIndex = -1;
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHoaDon.Text) ||
                cboLoaiHoaDon.SelectedItem == null ||
                cboTenDaiLy.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            string maHD = txtMaHoaDon.Text.Trim();

            if (db.HOADON.Any(h => h.MaHoaDon == maHD))
            {
                MessageBox.Show("Mã hóa đơn đã tồn tại!");
                return;
            }

            HOADON newHD = new HOADON
            {
                MaHoaDon = maHD,
                NgayLap = dtpNgayLap.SelectedDate ?? DateTime.Now,
                LoaiHoaDon = cboLoaiHoaDon.SelectedItem.ToString(),
                MaDaily = cboTenDaiLy.SelectedValue.ToString(),
                TongTien = 0
            };

            db.HOADON.Add(newHD);
            db.SaveChanges();
            loadHD();
            MessageBox.Show("Thêm hóa đơn thành công!");
            BtnReset_Click(null, null);
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgHoaDon.SelectedItem is HOADON selected)
            {
                var hd = db.HOADON.Find(selected.MaHoaDon);
                if (hd != null)
                {
                    hd.NgayLap = dtpNgayLap.SelectedDate ?? hd.NgayLap;
                    if (cboLoaiHoaDon.SelectedItem != null)
                        hd.LoaiHoaDon = cboLoaiHoaDon.SelectedItem.ToString();
                    if (cboTenDaiLy.SelectedValue != null)
                        hd.MaDaily = cboTenDaiLy.SelectedValue.ToString();

                    db.SaveChanges();
                    loadHD();
                    MessageBox.Show("Cập nhật thành công!");
                    BtnReset_Click(null, null);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần sửa!");
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgHoaDon.SelectedItem is HOADON selected)
            {
                var hd = db.HOADON.Include(h => h.CHITIETHOADON)
                                  .FirstOrDefault(h => h.MaHoaDon == selected.MaHoaDon);

                if (hd != null)
                {
                    if (hd.CHITIETHOADON.Any())
                    {
                        MessageBox.Show("Không thể xóa hóa đơn có chi tiết!");
                        return;
                    }

                    db.HOADON.Remove(hd);
                    db.SaveChanges();
                    loadHD();
                    MessageBox.Show("Xóa hóa đơn thành công!");
                    BtnReset_Click(null, null);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa!");
            }
        }

        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            string tenTim = Interaction.InputBox("Nhập tên đại lý cần tìm trong hóa đơn:", "Tìm kiếm theo tên đại lý", "");

            if (string.IsNullOrWhiteSpace(tenTim))
            {
                MessageBox.Show("Bạn chưa nhập tên đại lý.");
                return;
            }

            // Tìm hóa đơn đầu tiên có tên đại lý khớp
            var hoaDon = dgHoaDon.Items.Cast<HOADON>()
                              .FirstOrDefault(hd =>
                                  hd.THONGTINDAILY != null &&
                                  hd.THONGTINDAILY.TenDaily.IndexOf(tenTim, StringComparison.OrdinalIgnoreCase) >= 0);

            if (hoaDon != null)
            {
                dgHoaDon.SelectedItem = hoaDon;
                dgHoaDon.ScrollIntoView(hoaDon);
            }
            else
            {
                MessageBox.Show($"Không tìm thấy hóa đơn nào có đại lý tên: {tenTim}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            txtMaHoaDon.Clear();
            txtMaHoaDon.IsEnabled = true;
            dtpNgayLap.SelectedDate = null;
            cboLoaiHoaDon.SelectedIndex = -1;
            cboTenDaiLy.SelectedIndex = -1;
            dgHoaDon.SelectedIndex = -1;
        }

        private void DgHoaDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgHoaDon.SelectedItem is HOADON selected)
            {
                txtMaHoaDon.Text = selected.MaHoaDon;
                txtMaHoaDon.IsEnabled = false;
                dtpNgayLap.SelectedDate = selected.NgayLap;
                cboLoaiHoaDon.SelectedItem = selected.LoaiHoaDon;
                cboTenDaiLy.SelectedValue = selected.MaDaily;
            }
        }

        private void DgHoaDon_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgHoaDon.SelectedItem is HOADON selected)
            {
                var chiTietWindow = new Window_ChiTietHoaDon(selected.MaHoaDon);
                chiTietWindow.ShowDialog();
            }
        }

        private void BtnXemChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (dgHoaDon.SelectedItem is HOADON selected)
            {
                var chiTietWindow = new Window_ChiTietHoaDon(selected.MaHoaDon);
                chiTietWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xem.");
            }
        }
    }
}
