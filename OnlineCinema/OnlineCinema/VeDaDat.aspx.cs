using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;
using OnlineCinema.Models; // Giả sử bạn có

namespace OnlineCinema
{
    public partial class VeDaDat : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("DangKy_DangNhap.aspx?msg=Vui lòng đăng nhập");
                return;
            }
            if (!IsPostBack)
            {
                hdCurrentTab.Value = "1";
                btnSapChieu.CssClass = "tab-btn active";
                LoadTickets();
            }
        }

        protected void btnTab_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            hdCurrentTab.Value = btn.CommandArgument;
            btnSapChieu.CssClass = "tab-btn";
            btnDaXem.CssClass = "tab-btn";
            btnDaHuy.CssClass = "tab-btn";
            btn.CssClass = "tab-btn active";
            LoadTickets();
        }

        private void LoadTickets()
        {
            try
            {
                int maKH = Convert.ToInt32(Session["UserID"]);
                string tabState = hdCurrentTab.Value;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT v.MaVe, p.TenPhim, pc.TenPhong, lc.ThoiGianBatDau, 
                               v.TrangThai AS TrangThaiVe, g.SoGhe
                        FROM Ve v
                        INNER JOIN LichChieu lc ON v.MaLichChieu = lc.MaLichChieu
                        LEFT JOIN Phim p ON lc.MaPhim = p.MaPhim
                        LEFT JOIN PhongChieu pc ON lc.MaPhong = pc.MaPhong
                        LEFT JOIN Ghe g ON v.MaGhe = g.MaGhe
                        WHERE v.MaKH = @MaKH
                    ");

                    // === CẬP NHẬT LOGIC LỌC TAB ===
                    if (tabState == "1") // Tab: Sắp chiếu / Đang chờ
                    {
                        // Hiển thị cả vé Đã xác nhận (1) và vé Chờ (2), miễn là chưa chiếu
                        queryBuilder.Append(" AND (v.TrangThai = 1 OR v.TrangThai = 2) AND lc.ThoiGianBatDau > GETDATE()");
                    }
                    else if (tabState == "2") // Tab: Đã xem
                    {
                        // Chỉ hiển thị vé Đã xác nhận (1) VÀ đã chiếu
                        queryBuilder.Append(" AND v.TrangThai = 1 AND lc.ThoiGianBatDau <= GETDATE()");
                    }
                    else // (tabState == "0") - Tab: Đã hủy
                    {
                        queryBuilder.Append(" AND v.TrangThai = 0");
                    }

                    queryBuilder.Append(" ORDER BY lc.ThoiGianBatDau DESC");

                    using (SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn))
                    {
                        cmd.Parameters.AddWithValue("@MaKH", maKH);
                        conn.Open();
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd)) { adapter.Fill(dt); }

                        if (dt.Rows.Count > 0)
                        {
                            rptTickets.DataSource = dt;
                            rptTickets.DataBind();
                            pnlTickets.Visible = true;
                            lblEmptyTickets.Visible = false;
                        }
                        else
                        {
                            rptTickets.DataSource = null; rptTickets.DataBind();
                            pnlTickets.Visible = true;
                            lblEmptyTickets.Visible = true;
                            lblEmptyTickets.Text = "Bạn không có vé nào trong mục này.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi tải vé: {ex.Message}');", true);
            }
        }

        // === HÀM CẬP NHẬT: THÊM TRẠNG THÁI "CHỜ" ===
        public string GetTrangThaiVe(int trangThai, DateTime thoiGianBatDau)
        {
            if (trangThai == 0)
            {
                return "<span class='status-da-huy'>Đã hủy / Từ chối</span>";
            }
            if (trangThai == 2)
            {
                return "<span class='status-cho-thanh-toan'>Đang chờ xác nhận</span>";
            }

            // Nếu trangThai = 1
            if (thoiGianBatDau > DateTime.Now)
            {
                return "<span class='status-da-thanh-toan'>Đã xác nhận</span>";
            }
            else
            {
                return "<span class'status-da-xem'>Đã xem</span>";
            }
        }

        // === HÀM HỦY VÉ (Giữ nguyên, chỉ hủy vé TrangThai = 1) ===
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int maVe = Convert.ToInt32(btn.CommandArgument);
                int maKH = Convert.ToInt32(Session["UserID"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Chỉ cho phép hủy vé đã xác nhận (TrangThai = 1)
                    string query = "UPDATE Ve SET TrangThai = 0 WHERE MaVe = @MaVe AND MaKH = @MaKH AND TrangThai = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaVe", maVe);
                        cmd.Parameters.AddWithValue("@MaKH", maKH);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Hủy vé thất bại! (Chỉ hủy được vé đã xác nhận và chưa chiếu)');", true);
                            return;
                        }
                    }
                }
                LoadTickets();
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Hủy vé thành công!');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi hủy vé: {ex.Message}');", true);
            }
        }
    }
}