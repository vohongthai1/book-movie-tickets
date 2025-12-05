using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace OnlineCinema
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardData();
                LoadRecentBookings();
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Tổng số phim
                    string queryPhim = "SELECT COUNT(*) FROM Phim";
                    using (SqlCommand cmd = new SqlCommand(queryPhim, conn))
                    {
                        lblTongPhim.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Lịch chiếu hôm nay
                    string queryLichChieu = "SELECT COUNT(*) FROM LichChieu WHERE CAST(ThoiGianBatDau AS DATE) = CAST(GETDATE() AS DATE)";
                    using (SqlCommand cmd = new SqlCommand(queryLichChieu, conn))
                    {
                        lblTongLichChieu.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Vé đã đặt hôm nay
                    string queryVe = "SELECT COUNT(*) FROM Ve WHERE CAST(NgayDat AS DATE) = CAST(GETDATE() AS DATE) AND TrangThai = 1";
                    using (SqlCommand cmd = new SqlCommand(queryVe, conn))
                    {
                        lblTongVe.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Doanh thu hôm nay
                    string queryDoanhThu = @"
                        SELECT ISNULL(SUM(lc.GiaVe * v.SoLuongVe), 0)
                        FROM Ve v
                        INNER JOIN LichChieu lc ON v.MaLichChieu = lc.MaLichChieu
                        WHERE CAST(v.NgayDat AS DATE) = CAST(GETDATE() AS DATE) 
                        AND v.TrangThai = 1";
                    using (SqlCommand cmd = new SqlCommand(queryDoanhThu, conn))
                    {
                        decimal doanhThu = Convert.ToDecimal(cmd.ExecuteScalar());
                        lblDoanhThu.Text = doanhThu.ToString("N0");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error loading dashboard data: " + ex.Message);
            }
        }

        private void LoadRecentBookings()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT TOP 10
                            v.MaVe,
                            p.TenPhim,
                            kh.HoTen,
                            v.SoLuongVe,
                            (lc.GiaVe * v.SoLuongVe) AS TongTien,
                            v.NgayDat,
                            v.TrangThai
                        FROM Ve v
                        INNER JOIN LichChieu lc ON v.MaLichChieu = lc.MaLichChieu
                        INNER JOIN Phim p ON lc.MaPhim = p.MaPhim
                        INNER JOIN KhachHang kh ON v.MaKH = kh.MaKH
                        ORDER BY v.NgayDat DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvRecentBookings.DataSource = dt;
                        gvRecentBookings.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading recent bookings: " + ex.Message);
            }
        }

        protected string GetStatusClass(object trangThai)
        {
            if (trangThai == null) return "status-badge";

            int status = Convert.ToInt32(trangThai);
            switch (status)
            {
                case 0:
                    return "status-badge status-pending";
                case 1:
                    return "status-badge status-success";
                case 2:
                    return "status-badge status-cancelled";
                default:
                    return "status-badge";
            }
        }

        protected string GetStatusText(object trangThai)
        {
            if (trangThai == null) return "Không xác định";

            int status = Convert.ToInt32(trangThai);
            switch (status)
            {
                case 0:
                    return "Trong giỏ hàng";
                case 1:
                    return "Đã đặt";
                case 2:
                    return "Đã hủy";
                default:
                    return "Không xác định";
            }
        }
    }
}