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
using System.Windows.Shapes;

namespace FE
{
    /// <summary>
    /// Interaction logic for GD_DangKy.xaml
    /// </summary>
    public partial class GD_DangKy : Window
    {
        // Khởi tạo đối tượng Database Context
        QL_SP_Entities1 db = new QL_SP_Entities1();

        public GD_DangKy()
        {
            InitializeComponent();
        }

        private void Btn_DangKy(object sender, RoutedEventArgs e)
        {
            // Lấy giá trị mật khẩu từ đúng control đang hiển thị
            string username = txtTenDangKy.Text;
            string password = (txtMatKhauDK.Visibility == Visibility.Visible) ? txtMatKhauDK.Password : txtHienThiMatKhauDK.Text;

            // Kiểm tra các trường không được để trống
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tên đăng nhập và mật khẩu không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra xem tên đăng nhập đã tồn tại chưa
            var existingAccount = db.TAIKHOAN.FirstOrDefault(a => a.TenTaiKhoan == username);

            if (existingAccount != null)
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại! Vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Tạo một đối tượng TAIKHOAN mới
                TAIKHOAN newAccount = new TAIKHOAN();
                newAccount.TenTaiKhoan = username;
                newAccount.MatKhau = password;

                // Thêm đối tượng vào Entity Context
                db.TAIKHOAN.Add(newAccount);
                db.SaveChanges();

                MessageBox.Show("Đăng ký tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Sau khi đăng ký thành công, chuyển về giao diện đăng nhập
                GiaoDienDangNhap loginWindow = new GiaoDienDangNhap();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi đăng ký: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Btn_DangNhap(object sender, RoutedEventArgs e)
        {
            GiaoDienDangNhap main_window = new GiaoDienDangNhap();
            main_window.Show();
            this.Close();
        }

        private void Btn_HienMk_DK(object sender, RoutedEventArgs e)
        {
            if (txtMatKhauDK.Visibility == Visibility.Visible)
            {
                // PasswordBox đang hiển thị, chuyển sang hiển thị TextBox
                txtHienThiMatKhauDK.Text = txtMatKhauDK.Password;
                txtMatKhauDK.Visibility = Visibility.Collapsed;
                txtHienThiMatKhauDK.Visibility = Visibility.Visible;
                ((Button)sender).Content = "🔒";
            }
            else
            {
                // TextBox đang hiển thị, chuyển sang hiển thị PasswordBox
                txtMatKhauDK.Password = txtHienThiMatKhauDK.Text;
                txtHienThiMatKhauDK.Visibility = Visibility.Collapsed;
                txtMatKhauDK.Visibility = Visibility.Visible;
                ((Button)sender).Content = "👁";
            }
        }
    }
}