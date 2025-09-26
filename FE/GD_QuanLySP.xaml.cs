using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualBasic; // để dùng Interaction.InputBox

namespace FE
{
    /// <summary>
    /// Interaction logic for QL_SanPham.xaml
    /// </summary>
    public partial class QL_SanPham : UserControl
    {
        public QL_SanPham()
        {
            InitializeComponent();
        }

        QL_SP_Entities1 db = new QL_SP_Entities1();

        public void LoadData()
        {
            dgSanPham.ItemsSource = db.SANPHAM.ToList();
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            string ma = txtMaSP.Text.Trim();
            if (string.IsNullOrEmpty(ma))
            {
                MessageBox.Show("Vui lòng nhập mã sản phẩm");
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong))
            {
                MessageBox.Show("Số lượng không hợp lệ");
                return;
            }

            if (!decimal.TryParse(txtGiaMua.Text, out decimal giaMua))
            {
                MessageBox.Show("Giá mua không hợp lệ");
                return;
            }

            if (!decimal.TryParse(txtGiaBan.Text, out decimal giaBan))
            {
                MessageBox.Show("Giá bán không hợp lệ");
                return;
            }

            var existing = db.SANPHAM.FirstOrDefault(s => s.MaSanPham == ma);
            if (existing != null)
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại, vui lòng nhập mã khác!");
                return;
            }

            try
            {
                SANPHAM sp = new SANPHAM
                {
                    MaSanPham = ma,
                    TenSanPham = txtTenSP.Text.Trim(),
                    SoLuongTon = soLuong,
                    DonVi = txtDonVi.Text.Trim(),
                    GiaMua = giaMua,
                    GiaBan = giaBan
                };

                db.SANPHAM.Add(sp);
                db.SaveChanges();

                MessageBox.Show("Thêm sản phẩm thành công");
                LoadData();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            SANPHAM sp_chon = dgSanPham.SelectedItem as SANPHAM;
            if (sp_chon == null)
            {
                MessageBox.Show("Hãy chọn sản phẩm cần sửa");
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong))
            {
                MessageBox.Show("Số lượng không hợp lệ");
                return;
            }

            if (!decimal.TryParse(txtGiaMua.Text, out decimal giaMua))
            {
                MessageBox.Show("Giá mua không hợp lệ");
                return;
            }

            if (!decimal.TryParse(txtGiaBan.Text, out decimal giaBan))
            {
                MessageBox.Show("Giá bán không hợp lệ");
                return;
            }

            try
            {
                SANPHAM sp = db.SANPHAM.Find(sp_chon.MaSanPham);
                if (sp != null)
                {
                    sp.TenSanPham = txtTenSP.Text.Trim();
                    sp.SoLuongTon = soLuong;
                    sp.DonVi = txtDonVi.Text.Trim();
                    sp.GiaMua = giaMua;
                    sp.GiaBan = giaBan;

                    db.SaveChanges();
                    MessageBox.Show($"Sửa thông tin sản phẩm {sp_chon.TenSanPham} thành công");
                    LoadData();
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message);
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            SANPHAM sp_chon = dgSanPham.SelectedItem as SANPHAM;
            if (sp_chon == null)
            {
                MessageBox.Show("Hãy chọn sản phẩm cần xóa");
                return;
            }

            var chiTiet = db.CHITIETHOADON.Where(c => c.MaSanPham == sp_chon.MaSanPham).ToList();
            if (chiTiet.Any())
            {
                MessageBox.Show("Không thể xóa vì sản phẩm này đã có trong chi tiết hóa đơn!");
                return;
            }

            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa sản phẩm này?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    db.SANPHAM.Remove(sp_chon);
                    db.SaveChanges();
                    MessageBox.Show("Xóa sản phẩm thành công");
                    LoadData();
                    ResetForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.InnerException?.Message);
                }
            }
        }


        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            string maTim = Interaction.InputBox("Nhập mã sản phẩm cần tìm:", "Tìm kiếm", "");

            if (string.IsNullOrWhiteSpace(maTim))
            {
                MessageBox.Show("Bạn chưa nhập mã sản phẩm");
                return;
            }

            var sp = db.SANPHAM.FirstOrDefault(s => s.MaSanPham == maTim);

            if (sp != null)
            {
                dgSanPham.SelectedItem = sp;
                dgSanPham.ScrollIntoView(sp);
            }
            else
            {
                MessageBox.Show("Không tìm thấy sản phẩm có mã: " + maTim);
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ResetForm();
        }

        private void DgSanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SANPHAM sp = dgSanPham.SelectedItem as SANPHAM;
            if (sp == null)
            {
                ResetForm();
                return;
            }

            txtMaSP.Text = sp.MaSanPham;
            txtTenSP.Text = sp.TenSanPham;
            txtSoLuong.Text = sp.SoLuongTon.ToString();
            txtDonVi.Text = sp.DonVi;
            txtGiaMua.Text = sp.GiaMua.ToString();
            txtGiaBan.Text = sp.GiaBan.ToString();

            // Khi chọn sản phẩm thì không cho sửa mã
            SetTextBoxEditable(txtMaSP, false);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void SetTextBoxEditable(TextBox textBox, bool editable)
        {
            textBox.IsReadOnly = !editable;
            textBox.Background = editable ? Brushes.White : Brushes.LightGray;
            textBox.Foreground = editable ? Brushes.Black : Brushes.DarkGray;
        }

        private void ResetForm()
        {
            txtMaSP.Text = "";
            txtTenSP.Text = "";
            txtSoLuong.Text = "";
            txtDonVi.Text = "";
            txtGiaMua.Text = "";
            txtGiaBan.Text = "";
            dgSanPham.SelectedItem = null;

            // Cho phép nhập lại mã mới
            SetTextBoxEditable(txtMaSP, true);
        }
    }
}
