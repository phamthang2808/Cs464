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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class QL_SanPham : UserControl
    {
        public QL_SanPham()
        {
            InitializeComponent();
            LoadData();
        }

        private QL_SP_Entities db = new QL_SP_Entities();

      

        private void LoadData()
        {
            dgSanPham.ItemsSource = db.SANPHAM.ToList();
        }

        private void btnThemMoi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sp = new SANPHAM
                {
                    MaSanPham = txtMaSP.Text,
                    TenSanPham = txtTenSP.Text,
                    SoLuongTon = int.Parse(txtSoLuong.Text),
                    DonVi = txtDonVi.Text,
                    GiaBan = decimal.Parse(txtGia.Text)
                };

                db.SANPHAM.Add(sp);
                db.SaveChanges();

                MessageBox.Show("Thêm sản phẩm thành công!");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }
        }

        private void btnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtMaSP.Text))
                {
                    string maSP = txtMaSP.Text;

                    var sp = db.SANPHAM.Find(maSP);
                    if (sp != null)
                    {
                        sp.TenSanPham = txtTenSP.Text;
                        sp.SoLuongTon = int.Parse(txtSoLuong.Text);
                        sp.DonVi = txtDonVi.Text;
                        sp.GiaBan = decimal.Parse(txtGia.Text);

                        db.SaveChanges();
                        MessageBox.Show("Cập nhật sản phẩm thành công!");
                        LoadData();
                    }
                }

                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm để cập nhật!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var selectedProduct = dgSanPham.SelectedItem as SANPHAM;
                if (selectedProduct != null)
                {
                    // Xác nhận trước khi xóa
                    var result = MessageBox.Show(
                        "Bạn có chắc muốn xóa sản phẩm này?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        var sp = db.SANPHAM.Find(selectedProduct.MaSanPham);
                        if (sp != null)
                        {
                            db.SANPHAM.Remove(sp);
                            db.SaveChanges();
                            MessageBox.Show("Xóa sản phẩm thành công!");
                            LoadData(); // Load lại danh sách sau khi xóa
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm để xóa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void dgSanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProduct = dgSanPham.SelectedItem as SANPHAM;
            if (selectedProduct != null)
            {
                txtMaSP.Text = selectedProduct.MaSanPham;
                txtTenSP.Text = selectedProduct.TenSanPham;
                txtSoLuong.Text = selectedProduct.SoLuongTon.ToString();
                txtDonVi.Text = selectedProduct.DonVi;
                txtGia.Text = selectedProduct.GiaBan.ToString();
            }
        }

       
    }
}
