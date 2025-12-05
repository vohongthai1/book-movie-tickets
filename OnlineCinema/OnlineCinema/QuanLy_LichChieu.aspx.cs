using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnlineCinema
{
    public partial class QuanLy_LichChieu : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra vai trò quản trị viên
            if (Session["VaiTro"] == null || Session["VaiTro"].ToString() != "QuanTriVien")
            {
                Response.Redirect("DangKy_DangNhap.aspx?msg=Vui lòng đăng nhập với tư cách quản trị viên!");
                return;
            }

            if (!IsPostBack)
            {
                LoadDropdowns();
                LoadSchedules();
            }
        }

        private void LoadDropdowns()
        {
            // Load danh sách phim
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string queryPhim = "SELECT MaPhim, TenPhim FROM Phim";
                using (SqlCommand cmd = new SqlCommand(queryPhim, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dtPhim = new DataTable();
                        adapter.Fill(dtPhim);
                        ddlPhim.DataSource = dtPhim;
                        ddlPhim.DataTextField = "TenPhim";
                        ddlPhim.DataValueField = "MaPhim";
                        ddlPhim.DataBind();
                        ddlPhim.Items.Insert(0, new ListItem("Chọn phim", ""));
                    }
                }
            }

            // Load danh sách phòng chiếu
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string queryPhong = "SELECT MaPhong, TenPhong FROM PhongChieu";
                using (SqlCommand cmd = new SqlCommand(queryPhong, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dtPhong = new DataTable();
                        adapter.Fill(dtPhong);
                        ddlPhong.DataSource = dtPhong;
                        ddlPhong.DataTextField = "TenPhong";
                        ddlPhong.DataValueField = "MaPhong";
                        ddlPhong.DataBind();
                        ddlPhong.Items.Insert(0, new ListItem("Chọn phòng", ""));
                    }
                }
            }
        }

        private void LoadSchedules()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT lc.MaLichChieu, lc.MaPhim, p.TenPhim, lc.MaPhong, pc.TenPhong, lc.ThoiGianBatDau, lc.GiaVe
                        FROM LichChieu lc
                        JOIN Phim p ON lc.MaPhim = p.MaPhim
                        JOIN PhongChieu pc ON lc.MaPhong = pc.MaPhong";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                rptSchedules.DataSource = dt;
                                rptSchedules.DataBind();
                                lblEmptySchedules.Visible = false;
                            }
                            else
                            {
                                lblEmptySchedules.Visible = true;
                                rptSchedules.DataSource = null;
                                rptSchedules.DataBind();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi tải danh sách lịch chiếu: {ex.Message}');", true);
            }
        }

        protected void btnAddSchedule_Click(object sender, EventArgs e)
        {
            lblFormTitle.Text = "Thêm Lịch Chiếu Mới";
            ClearForm();
            pnlAddEditSchedule.Visible = true;
            ViewState["MaLichChieu"] = null; // Đặt null để biết là thêm mới
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int maLichChieu = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT lc.MaPhim, lc.MaPhong, lc.ThoiGianBatDau, lc.GiaVe
                    FROM LichChieu lc
                    WHERE lc.MaLichChieu = @MaLichChieu";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ddlPhim.SelectedValue = reader["MaPhim"].ToString();
                            ddlPhong.SelectedValue = reader["MaPhong"].ToString();
                            txtThoiGianBatDau.Text = Convert.ToDateTime(reader["ThoiGianBatDau"]).ToString("yyyy-MM-ddTHH:mm");
                            txtGiaVe.Text = reader["GiaVe"].ToString();
                        }
                    }
                }
            }

            lblFormTitle.Text = "Sửa Lịch Chiếu";
            pnlAddEditSchedule.Visible = true;
            ViewState["MaLichChieu"] = maLichChieu;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                int maPhim = Convert.ToInt32(ddlPhim.SelectedValue);
                int maPhong = Convert.ToInt32(ddlPhong.SelectedValue);
                DateTime thoiGianBatDau = Convert.ToDateTime(txtThoiGianBatDau.Text);
                decimal giaVe = Convert.ToDecimal(txtGiaVe.Text);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query;
                    if (ViewState["MaLichChieu"] == null) // Thêm mới
                    {
                        query = @"
                            INSERT INTO LichChieu (MaPhim, MaPhong, ThoiGianBatDau, GiaVe)
                            VALUES (@MaPhim, @MaPhong, @ThoiGianBatDau, @GiaVe)";
                    }
                    else // Sửa
                    {
                        query = @"
                            UPDATE LichChieu
                            SET MaPhim = @MaPhim, MaPhong = @MaPhong, ThoiGianBatDau = @ThoiGianBatDau, GiaVe = @GiaVe
                            WHERE MaLichChieu = @MaLichChieu";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPhim", maPhim);
                        cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                        cmd.Parameters.AddWithValue("@ThoiGianBatDau", thoiGianBatDau);
                        cmd.Parameters.AddWithValue("@GiaVe", giaVe);
                        if (ViewState["MaLichChieu"] != null)
                        {
                            cmd.Parameters.AddWithValue("@MaLichChieu", ViewState["MaLichChieu"]);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lưu lịch chiếu thành công!');", true);
                LoadSchedules();
                pnlAddEditSchedule.Visible = false;
                ClearForm();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi lưu lịch chiếu: {ex.Message}');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int maLichChieu = Convert.ToInt32(btn.CommandArgument);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Kiểm tra xem lịch chiếu có vé đã đặt không
                    string checkQuery = "SELECT COUNT(*) FROM Ve WHERE MaLichChieu = @MaLichChieu AND TrangThai = 1";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không thể xóa lịch chiếu vì đã có vé đặt!');", true);
                            return;
                        }
                    }

                    // Xóa lịch chiếu
                    string query = "DELETE FROM LichChieu WHERE MaLichChieu = @MaLichChieu";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Xóa lịch chiếu thành công!');", true);
                LoadSchedules();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi xóa lịch chiếu: {ex.Message}');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlAddEditSchedule.Visible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            ddlPhim.SelectedIndex = 0;
            ddlPhong.SelectedIndex = 0;
            txtThoiGianBatDau.Text = "";
            txtGiaVe.Text = "";
        }
    }
}