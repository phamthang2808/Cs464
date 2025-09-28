using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FE
{
    public partial class GD_TrangChu : Window
    {
        public GD_TrangChu()
        {
            InitializeComponent();
            LoadTrangChuContent();
        }

        private void LoadTrangChuContent()
        {
            Image homeImage = new Image();
            homeImage.Source = new BitmapImage(new Uri("/Images/trangchu.png", UriKind.Relative));
            homeImage.Stretch = Stretch.Fill;
            homeImage.Margin = new Thickness(0);

            MainContent.Content = homeImage;
        }

        private void Btn_TrangChu_Click(object sender, RoutedEventArgs e)
        {
            GD_TrangChu trangchu_window = new GD_TrangChu();
            trangchu_window.Show();
            this.Close();
        }

        private void Btn_SanPham_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new QL_SanPham();
        }

        private void Btn_ThongKe_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GD_ThongKe();
        }

        private void Btn_DaiLy_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GiaodienDaiLy();
        }

        private void BtnHoaDon_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GD_DSHoadon();
        }
    }
}