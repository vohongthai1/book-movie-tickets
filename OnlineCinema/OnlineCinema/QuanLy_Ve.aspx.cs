using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;

namespace OnlineCinema
{
    public partial class QuanLy_Ve : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["VaiTro"] == null || Session["VaiTro"].ToString() != "QuanTriVien")
            {
                Response.Redirect("DangKy_DangNhap.aspx?msg=Vui lòng đăng nhập với tư cách quản trị viên!");
                return;
            }

            if (!IsPostBack)
            {
                LoadTheLoaiFilter();

                // Mặc định chọn tab "Chờ thanh toán"
                if (ddlTrangThai.Items.FindByValue("2") != null)
                {
                    ddlTrangThai.SelectedValue = "2";
                }

                LoadTickets();
            }
        }

        private void LoadTheLoaiFilter()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT TheLoai FROM Phim WHERE TheLoai IS NOT NULL AND TheLoai <> '' ORDER BY TheLoai";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        ddlTheLoai.DataSource = dt;
                        ddlTheLoai.DataBind();
                        ddlTheLoai.Items.Insert(0, new ListItem("-- Tất cả thể loại --", "0"));
                    }
                }
            }
            catch (Exception ex)
            {
                // SỬA LỖI .NET 4.0: Dùng string.Format
                string errorMsg = string.Format("alert('Lỗi tải danh sách thể loại: {0}');", ex.Message.Replace("'", "").Replace("\r\n", " "));
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", errorMsg, true);
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadTickets();
        }

        // === HÀM NÂNG CẤP: LOAD DANH SÁCH VÉ (BẢN ĐẦY ĐỦ) ===
        private void LoadTickets()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder queryBuilder = new StringBuilder(@"
                        SELECT 
                            v.MaVe, kh.HoTen, kh.SoDienThoai, kh.Email, p.TenPhim, 
                            pc.TenPhong, lc.ThoiGianBatDau, v.SoLuongVe,
                            (v.SoLuongVe * lc.GiaVe) AS TongTien, 
                            v.MaGhe, v.TrangThai AS TrangThaiVe,
                            g.SoGhe, p.TheLoai,
                            v.MaDonHang, v.GhiChu
                        FROM Ve v
                        INNER JOIN LichChieu lc ON v.MaLichChieu = lc.MaLichChieu
                        LEFT JOIN Phim p ON lc.MaPhim = p.MaPhim
                        LEFT JOIN PhongChieu pc ON lc.MaPhong = pc.MaPhong
                        LEFT JOIN KhachHang kh ON v.MaKH = kh.MaKH
                        LEFT JOIN Ghe g ON v.MaGhe = g.MaGhe
                        WHERE 1=1
                    ");

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    // Lọc: Từ khóa tìm kiếm (THÊM MaDonHang)
                    if (!string.IsNullOrEmpty(txtSearch.Text))
                    {
                        queryBuilder.Append(" AND (CONVERT(VARCHAR(20), v.MaVe) LIKE @Search OR kh.SoDienThoai LIKE @Search OR kh.Email LIKE @Search OR v.MaDonHang LIKE @Search)");
                        parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
                    }

                    // Lọc: Thể Loại
                    if (ddlTheLoai.SelectedValue != "0")
                    {
                        queryBuilder.Append(" AND p.TheLoai = @TheLoai");
                        parameters.Add(new SqlParameter("@TheLoai", ddlTheLoai.SelectedValue));
                    }

                    // LỌC TRẠNG THÁI MỚI
                    if (ddlTrangThai.SelectedValue != "-1") // -1 là "Tất cả"
                    {
                        queryBuilder.Append(" AND v.TrangThai = @TrangThai");
                        parameters.Add(new SqlParameter("@TrangThai", Convert.ToInt32(ddlTrangThai.SelectedValue)));
                    }

                    // Lọc: Từ Ngày
                    if (!string.IsNullOrEmpty(txtTuNgay.Text))
                    {
                        queryBuilder.Append(" AND lc.ThoiGianBatDau >= @TuNgay");
                        parameters.Add(new SqlParameter("@TuNgay", Convert.ToDateTime(txtTuNgay.Text)));
                    }

                    // Lọc: Đến Ngày
                    if (!string.IsNullOrEmpty(txtDenNgay.Text))
                    {
                        DateTime denNgay = Convert.ToDateTime(txtDenNgay.Text).AddDays(1);
                        queryBuilder.Append(" AND lc.ThoiGianBatDau < @DenNgay");
                        parameters.Add(new SqlParameter("@DenNgay", denNgay));
                    }

                    queryBuilder.Append(" ORDER BY v.MaDonHang DESC, lc.ThoiGianBatDau DESC");

                    using (SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        conn.Open();
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd)) { adapter.Fill(dt); }
                        if (dt.Rows.Count > 0)
                        {
                            rptTickets.DataSource = dt;
                            rptTickets.DataBind();
                            pnlTicketList.Visible = true;
                            lblEmptyTickets.Visible = false;
                        }
                        else
                        {
                            rptTickets.DataSource = null;
                            rptTickets.DataBind();
                            pnlTicketList.Visible = true;
                            lblEmptyTickets.Visible = true;
                            lblEmptyTickets.Text = "Không có vé nào khớp với bộ lọc.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // SỬA LỖI .NET 4.0: Dùng string.Format
                string errorMsg = string.Format("alert('LỖI TẢI VÉ: {0}');", ex.Message.Replace("'", "").Replace("\r\n", " "));
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", errorMsg, true);
            }
        }

        // === HÀM CẬP NHẬT: THÊM TRẠNG THÁI "CHỜ" (Bản CSS Pill) ===
        public string GetTrangThaiVe(int trangThai, DateTime thoiGianBatDau)
        {
            if (trangThai == 0)
            {
                return "<span class='status-pill status-da-huy'>Đã hủy / Từ chối</span>";
            }
            if (trangThai == 2)
            {
                return "<span class='status-pill status-cho-thanh-toan'>Chờ thanh toán</span>";
            }

            // Nếu trangThai = 1
            if (thoiGianBatDau > DateTime.Now)
            {
                return "<span class='status-pill status-sap-chieu'>Đã xác nhận</span>";
            }
            else
            {
                return "<span class='status-pill status-da-chieu'>Đã xem</span>";
            }
        }

        // === HÀM MỚI: DUYỆT ĐƠN HÀNG ===
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string maDonHang = btn.CommandArgument;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Ve SET TrangThai = 1 WHERE MaDonHang = @MaDonHang AND TrangThai = 2";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // SỬA LỖI .NET 4.0: Dùng string.Format
                            string alertMsg = string.Format("alert('Duyệt thành công {0} vé cho đơn hàng {1}!');", rowsAffected, maDonHang);
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", alertMsg, true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không tìm thấy vé nào để duyệt.');", true);
                        }
                    }
                }
                LoadTickets(); // Tải lại danh sách
            }
            catch (Exception ex)
            {
                // SỬA LỖI .NET 4.0: Dùng string.Format
                string errorMsg = string.Format("alert('Lỗi khi duyệt vé: {0}');", ex.Message.Replace("'", "").Replace("\r\n", " "));
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", errorMsg, true);
            }
        }

        // === HÀM MỚI: TỪ CHỐI ĐƠN HÀNG ===
        protected void btnReject_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string maDonHang = btn.CommandArgument;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Ve SET TrangThai = 0 WHERE MaDonHang = @MaDonHang AND TrangThai = 2";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // SỬA LỖI .NET 4.0: Dùng string.Format
                            string alertMsg = string.Format("alert('Từ chối thành công {0} vé cho đơn hàng {1}!');", rowsAffected, maDonHang);
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", alertMsg, true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không tìm thấy vé nào để từ chối.');", true);
                        }
                    }
                }
                LoadTickets(); // Tải lại danh sách
            }
            catch (Exception ex)
            {
                // SỬA LỖI .NET 4.0: Dùng string.Format
                string errorMsg = string.Format("alert('Lỗi khi từ chối vé: {0}');", ex.Message.Replace("'", "").Replace("\r\n", " "));
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", errorMsg, true);
            }
        }

        // === HÀM HỦY VÉ ĐÃ XÁC NHẬN (Cập nhật) ===
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int maVe = Convert.ToInt32(btn.CommandArgument);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Ve SET TrangThai = 0 WHERE MaVe = @MaVe AND TrangThai = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaVe", maVe);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Hủy vé thất bại! (Có thể vé không còn hợp lệ)');", true);
                            return;
                        }
                    }
                }
                LoadTickets();
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Hủy vé thành công!');", true);
            }
            catch (Exception ex)
            {
                // SỬA LỖI .NET 4.0: Dùng string.Format
                string errorMsg = string.Format("alert('Lỗi hủy vé: {0}');", ex.Message.Replace("'", "").Replace("\r\n", " "));
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", errorMsg, true);
            }
        }
    }
}