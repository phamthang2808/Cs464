using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FE
{
    public partial class GD_ThongKe : UserControl
    {
        public SeriesCollection ColumnSeries { get; set; }
        public SeriesCollection PieSeries { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public GD_ThongKe()
        {
            InitializeComponent();
            ColumnSeries = new SeriesCollection();
            PieSeries = new SeriesCollection();
            YFormatter = value => value.ToString("N0") + " đ";
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dpThang.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn tháng để thống kê.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime selectedDate = dpThang.SelectedDate.Value;
            int thang = selectedDate.Month;
            int nam = selectedDate.Year;

            ThongKeTheoThang(thang, nam);
        }

        private void ThongKeTheoThang(int thang, int nam)
        {
            using (var db = new QL_SP_Entities1())
            {
                // Lấy danh sách hóa đơn BÁN trong tháng/năm
                var hoaDons = db.HOADON
                    .Where(hd => hd.NgayLap.HasValue &&
                                 hd.NgayLap.Value.Month == thang &&
                                 hd.NgayLap.Value.Year == nam &&
                                 (hd.LoaiHoaDon != null && hd.LoaiHoaDon.Trim().ToUpper() == "BÁN"))
                    .ToList();

                if (!hoaDons.Any())
                {
                    MessageBox.Show($"Không có hóa đơn bán nào trong tháng {thang}/{nam}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    ColumnSeries.Clear();
                    PieSeries.Clear();
                    DataContext = null;
                    DataContext = this;
                    return;
                }

                // Lấy chi tiết hóa đơn
                var chiTiet = hoaDons.SelectMany(hd => hd.CHITIETHOADON ?? Enumerable.Empty<CHITIETHOADON>()).ToList();

                // Nhóm và tính tổng doanh thu theo sản phẩm
                var thongKe = chiTiet
                    .GroupBy(ct => new { ct.SANPHAM.MaSanPham, ct.SANPHAM.TenSanPham })
                    .Select(g => new
                    {
                        TenSanPham = g.Key.TenSanPham,
                        TongDoanhThu = g.Sum(ct => ct.ThanhTien)
                    })
                    .OrderByDescending(x => x.TongDoanhThu)
                    .ToList();

                // Cập nhật biểu đồ cột (Chuyển decimal sang double)
                ColumnSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = $"Doanh thu tháng {thang}/{nam}",
                        Values = new ChartValues<double>(thongKe.Select(tk => (double)tk.TongDoanhThu)),
                        DataLabels = true
                    }
                };

                Labels = thongKe.Select(tk => tk.TenSanPham).ToArray();

                // Cập nhật biểu đồ tròn (Chuyển decimal sang double)
                PieSeries = new SeriesCollection();
                foreach (var item in thongKe)
                {
                    PieSeries.Add(new PieSeries
                    {
                        Title = item.TenSanPham,
                        Values = new ChartValues<double> { (double)item.TongDoanhThu },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{chartPoint.Y:N0} đ ({chartPoint.Participation:P})"
                    });
                }

                // Cập nhật UI
                DataContext = null;
                DataContext = this;
            }
        }
    }
}