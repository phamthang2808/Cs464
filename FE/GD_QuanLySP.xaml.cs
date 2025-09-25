using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation; // thêm
using System.Windows.Shapes;
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

            var existing = db.SANPHAM.FirstOrDefault(s => s.MaSanPham == ma);
            if (existing != null)
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại, vui lòng nhập mã khác!");
                return;
            }

            var local = db.SANPHAM.Local.FirstOrDefault(s => s.MaSanPham == ma);
            if (local != null)
            {
                db.Entry(local).State = System.Data.Entity.EntityState.Detached;
            }

            try
            {
                SANPHAM sp = new SANPHAM
                {
                    MaSanPham = ma,
                    TenSanPham = txtTenSP.Text.Trim(),
                    SoLuongTon = int.Parse(txtSoLuong.Text),
                    DonVi = txtDonVi.Text.Trim(),
                    GiaBan = decimal.Parse(txtGia.Text)
                };

                db.SANPHAM.Add(sp);
                db.SaveChanges();

                MessageBox.Show("Thêm sản phẩm thành công");
                LoadData();
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

            try
            {
                SANPHAM sp = db.SANPHAM.Find(sp_chon.MaSanPham);
                if (sp != null)
                {
                    sp.TenSanPham = txtTenSP.Text.Trim();
                    sp.SoLuongTon = int.Parse(txtSoLuong.Text);
                    sp.DonVi = txtDonVi.Text.Trim();
                    sp.GiaBan = decimal.Parse(txtGia.Text);

                    db.SaveChanges();
                    MessageBox.Show($"Sửa thông tin sản phẩm {sp_chon.TenSanPham} thành công");
                    LoadData();
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

            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa sản phẩm này?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    SANPHAM sp = db.SANPHAM.Find(sp_chon.MaSanPham);
                    if (sp != null)
                    {
                        db.SANPHAM.Remove(sp);
                        db.SaveChanges();
                        MessageBox.Show("Xóa sản phẩm thành công");
                        LoadData();
                        SetTextBoxEditable(txtMaSP, false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
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

            var sp = dgSanPham.Items.Cast<SANPHAM>()
                            .FirstOrDefault(s => s.MaSanPham == maTim);

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
            SetTextBoxEditable(txtMaSP, false);
            txtMaSP.Text = "";
            txtTenSP.Text = "";
            txtSoLuong.Text = "";
            txtDonVi.Text = "";
            txtGia.Text = "";
            dgSanPham.SelectedItem = null;
            dgSanPham.ScrollIntoView(0);
        }

        private void DgSanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SANPHAM sp = dgSanPham.SelectedItem as SANPHAM;
            if (sp == null)
            {
                txtMaSP.Text = "";
                txtTenSP.Text = "";
                txtSoLuong.Text = "";
                txtDonVi.Text = "";
                txtGia.Text = "";
                return;
            }

            txtMaSP.Text = sp.MaSanPham;
            txtTenSP.Text = sp.TenSanPham;
            txtSoLuong.Text = sp.SoLuongTon.ToString();
            txtDonVi.Text = sp.DonVi;
            txtGia.Text = sp.GiaBan.ToString();

            if (dgSanPham.SelectedItem != null)
            {
                SetTextBoxEditable(txtMaSP, true);
            }
            else
            {
                SetTextBoxEditable(txtMaSP, false);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void SetTextBoxEditable(TextBox textBox, bool editable)
        {
            if (editable)
            {
                textBox.IsReadOnly = true;
                textBox.Background = Brushes.LightGray;
                textBox.Foreground = Brushes.DarkGray;
            }
            else
            {
                textBox.IsReadOnly = false;
                textBox.Background = Brushes.White;
                textBox.Foreground = Brushes.Black;
            }
        }
    }
}
