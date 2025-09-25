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
using System.Windows.Navigation; // Add this using directive
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace FE
{
    /// <summary>
    /// Interaction logic for GiaodienDaiLy.xaml
    /// </summary>
    public partial class GiaodienDaiLy : UserControl // Changed from Window to UserControl
    {
        public GiaodienDaiLy()
        {
            InitializeComponent();
        }
        QL_SP_Entities1 db = new QL_SP_Entities1();

        public void loadDaiLy()
        {
            dgDaiLy.ItemsSource = db.THONGTINDAILY.ToList();
        }


        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            string ma = txtMaDaiLy.Text.Trim();

            if (string.IsNullOrEmpty(ma))
            {
                MessageBox.Show("Vui lòng nhập mã đại lý");
                return;
            }

            var existing = db.THONGTINDAILY.FirstOrDefault(d => d.MaDaily == ma);
            if (existing != null)
            {
                MessageBox.Show("Mã đại lý đã tồn tại, vui lòng nhập mã khác!");
                return;
            }
            var local = db.THONGTINDAILY.Local.FirstOrDefault(d => d.MaDaily == ma);
            if (local != null)
            {
                db.Entry(local).State = System.Data.Entity.EntityState.Detached;
            }

            THONGTINDAILY tt_daily = new THONGTINDAILY
            {
                MaDaily = ma,
                TenDaily = txtTenDaiLy.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                SoDienThoai = txtSDT.Text.Trim()
            };

            db.THONGTINDAILY.Add(tt_daily);
            db.SaveChanges();

            MessageBox.Show("Thêm đại lý thành công");
            loadDaiLy();
        }


        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            THONGTINDAILY daily_chon = dgDaiLy.SelectedItem as THONGTINDAILY;
            if (daily_chon == null)
            {
                MessageBox.Show("Hãy chọn đại lý cần sửa thông tin");
            }
            else
            {
                THONGTINDAILY daily = db.THONGTINDAILY.Find(daily_chon.MaDaily);
                if (daily != null)
                {
                    daily.TenDaily = txtTenDaiLy.Text;
                    daily.DiaChi = txtDiaChi.Text;
                    daily.SoDienThoai = txtSDT.Text;

                    db.SaveChanges();
                    MessageBox.Show($"Sửa thông tin đại lý {daily_chon.TenDaily} thành công");
                    loadDaiLy();
                }
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            THONGTINDAILY daily_chon = dgDaiLy.SelectedItem as THONGTINDAILY;
            if (daily_chon == null)
            {
                MessageBox.Show("Hãy chọn đại lý cần xóa");
            }
            else
            {
                var result = MessageBox.Show(
                        "Bạn có chắc muốn xóa sản phẩm này?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    THONGTINDAILY daily = db.THONGTINDAILY.Find(daily_chon.MaDaily);

                    if (daily.HOADON.Any())
                    {
                        MessageBox.Show("Không thể xóa vì đại lý này còn hóa đơn liên quan.");
                        return;
                    }
                    db.THONGTINDAILY.Remove(daily);
                    db.SaveChanges();
                    MessageBox.Show("Xóa đại lý thành công");
                    loadDaiLy();
                    SetTextBoxEditable(txtMaDaiLy, false);
                }
            }
        }

        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            string maTim = Interaction.InputBox("Nhập mã đại lý cần tìm:", "Tìm kiếm", "");

            if (string.IsNullOrWhiteSpace(maTim))
            {
                MessageBox.Show("Bạn chưa nhập mã đại lý");
                return;
            }

            var daily = dgDaiLy.Items.Cast<THONGTINDAILY>()
                            .FirstOrDefault(dl => dl.MaDaily == maTim);

            if (daily != null)
            {
                dgDaiLy.SelectedItem = daily;
                dgDaiLy.ScrollIntoView(daily);
            }
            else
            {
                MessageBox.Show("Không tìm thấy đại lý có mã: " + maTim);
            }
        }


        private void DgDaiLy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            THONGTINDAILY daily = dgDaiLy.SelectedItem as THONGTINDAILY;
            if (daily == null)
            {
                txtMaDaiLy.Text = "";
                txtTenDaiLy.Text = "";
                txtDiaChi.Text = "";
                txtSDT.Text = "";
                return;
            }

            txtMaDaiLy.Text = daily.MaDaily;
            txtTenDaiLy.Text = daily.TenDaily;
            txtDiaChi.Text = daily.DiaChi;
            txtSDT.Text = daily.SoDienThoai;
            if (dgDaiLy.SelectedItem != null)
            {
                SetTextBoxEditable(txtMaDaiLy, true);
            }
            else
            {
                SetTextBoxEditable(txtMaDaiLy, false);
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            loadDaiLy();
            SetTextBoxEditable(txtMaDaiLy, false);
            txtMaDaiLy.Text = "";
            txtTenDaiLy.Text = "";
            txtDiaChi.Text = "";
            txtSDT.Text = "";
            dgDaiLy.SelectedItem = null;
            dgDaiLy.ScrollIntoView(0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadDaiLy();
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