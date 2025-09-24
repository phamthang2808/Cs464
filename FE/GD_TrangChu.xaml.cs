using System;
using System.Windows;
using System.Windows.Controls;

namespace FE
{
    public partial class GD_TrangChu : Window
    {
        public GD_TrangChu()
        {
            InitializeComponent();
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

        private void Btn_HoaDonBan_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GD_HoaDonBanHang();
        }

        private void Btn_HoaDonMua_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GD_HoaDonMuaHang();
        }

        private void Btn_ThongKe_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GD_ThongKe();
        }

        private void Btn_DaiLy_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GiaodienDaiLy();
        }
    }
}