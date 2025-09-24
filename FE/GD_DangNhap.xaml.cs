using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FE
{
    /// <summary>
    /// Interaction logic for GiaoDienDangNhap.xaml
    /// </summary>
    public partial class GiaoDienDangNhap : Window
    {
        // Khởi tạo đối tượng Database Context
        QL_SP_Entities db = new QL_SP_Entities();

        public GiaoDienDangNhap()
        {
            InitializeComponent();
        }

        private void Btn_DangNhap(object sender, RoutedEventArgs e)
        {
            string username = txtTenDangNhap.Text;

            // Lấy mật khẩu từ đúng control đang hiển thị
            string password = (txtMatKhau.Visibility == Visibility.Visible) ? txtMatKhau.Password : txtHienThiMatKhau.Text;

            // Kiểm tra thông tin đăng nhập trong database
            var account = db.TAIKHOAN.FirstOrDefault(a => a.TenTaiKhoan == username && a.MatKhau == password);

            if (account != null)
            {
                // Đăng nhập thành công
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // Mở giao diện chính 
                GD_TrangChu main_window = new GD_TrangChu();
                main_window.Show();
                this.Close();
            }
            else
            {
                // Đăng nhập thất bại
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Btn_DangKy(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ đăng ký
            GD_DangKy dangky_window = new GD_DangKy();
            dangky_window.Show();
            this.Close();
        }

        private void Btn_HienMk(object sender, RoutedEventArgs e)
        {
            // Kiểm tra trạng thái của PasswordBox để quyết định hành động
            if (txtMatKhau.Visibility == Visibility.Visible)
            {
                // PasswordBox đang hiển thị, chuyển sang hiển thị TextBox
                txtHienThiMatKhau.Text = txtMatKhau.Password;
                txtMatKhau.Visibility = Visibility.Collapsed;
                txtHienThiMatKhau.Visibility = Visibility.Visible;
                ((Button)sender).Content = "🔒"; // Thay đổi icon
            }
            else
            {
                // TextBox đang hiển thị, chuyển sang hiển thị PasswordBox
                txtMatKhau.Password = txtHienThiMatKhau.Text;
                txtHienThiMatKhau.Visibility = Visibility.Collapsed;
                txtMatKhau.Visibility = Visibility.Visible;
                ((Button)sender).Content = "👁"; // Thay đổi icon
            }
        }
    }
}