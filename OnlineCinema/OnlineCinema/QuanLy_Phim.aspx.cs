using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnlineCinema
{
    public partial class QuanLy_Phim : System.Web.UI.Page
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
                LoadMovies();
            }
        }

        private void LoadMovies()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT MaPhim, TenPhim, TheLoai, ThoiLuong, MoTa, NgayKhoiChieu, AnhBia, DaoDien FROM Phim";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                rptMovies.DataSource = dt;
                                rptMovies.DataBind();
                                lblEmptyMovies.Visible = false;
                            }
                            else
                            {
                                lblEmptyMovies.Visible = true;
                                rptMovies.DataSource = null;
                                rptMovies.DataBind();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi tải danh sách phim: {ex.Message}');", true);
            }
        }

        protected void btnAddMovie_Click(object sender, EventArgs e)
        {
            lblFormTitle.Text = "Thêm Phim Mới";
            ClearForm();
            pnlAddEditMovie.Visible = true;
            ViewState["MaPhim"] = null; // Đặt null để biết là thêm mới
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int maPhim = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TenPhim, TheLoai, ThoiLuong, MoTa, NgayKhoiChieu, AnhBia, DaoDien FROM Phim WHERE MaPhim = @MaPhim";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhim", maPhim);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTenPhim.Text = reader["TenPhim"].ToString();
                            txtTheLoai.Text = reader["TheLoai"] != DBNull.Value ? reader["TheLoai"].ToString() : "";
                            txtThoiLuong.Text = reader["ThoiLuong"] != DBNull.Value ? reader["ThoiLuong"].ToString() : "";
                            txtMoTa.Text = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : "";
                            txtNgayKhoiChieu.Text = reader["NgayKhoiChieu"] != DBNull.Value ? Convert.ToDateTime(reader["NgayKhoiChieu"]).ToString("yyyy-MM-dd") : "";
                            txtDaoDien.Text = reader["DaoDien"] != DBNull.Value ? reader["DaoDien"].ToString() : "";
                            hfAnhBia.Value = reader["AnhBia"] != DBNull.Value ? reader["AnhBia"].ToString() : "";
                        }
                    }
                }
            }

            lblFormTitle.Text = "Sửa Phim";
            pnlAddEditMovie.Visible = true;
            ViewState["MaPhim"] = maPhim;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                string tenPhim = txtTenPhim.Text.Trim();
                string theLoai = txtTheLoai.Text.Trim();
                int? thoiLuong = string.IsNullOrEmpty(txtThoiLuong.Text) ? (int?)null : Convert.ToInt32(txtThoiLuong.Text);
                string moTa = txtMoTa.Text.Trim();
                DateTime ngayKhoiChieu = Convert.ToDateTime(txtNgayKhoiChieu.Text);
                string daoDien = txtDaoDien.Text.Trim();
                string anhBia = hfAnhBia.Value;

                // Xử lý upload ảnh bìa
                if (fuAnhBia.HasFile)
                {
                    string fileName = Path.GetFileName(fuAnhBia.FileName);
                    string filePath = Server.MapPath("images/") + fileName;
                    fuAnhBia.SaveAs(filePath);
                    anhBia = "images/" + fileName;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query;
                    if (ViewState["MaPhim"] == null) // Thêm mới
                    {
                        query = @"
                            INSERT INTO Phim (TenPhim, TheLoai, ThoiLuong, MoTa, NgayKhoiChieu, AnhBia, DaoDien)
                            VALUES (@TenPhim, @TheLoai, @ThoiLuong, @MoTa, @NgayKhoiChieu, @AnhBia, @DaoDien)";
                    }
                    else // Sửa
                    {
                        query = @"
                            UPDATE Phim
                            SET TenPhim = @TenPhim, TheLoai = @TheLoai, ThoiLuong = @ThoiLuong, MoTa = @MoTa,
                                NgayKhoiChieu = @NgayKhoiChieu, AnhBia = @AnhBia, DaoDien = @DaoDien
                            WHERE MaPhim = @MaPhim";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenPhim", tenPhim);
                        cmd.Parameters.AddWithValue("@TheLoai", (object)theLoai ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ThoiLuong", (object)thoiLuong ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MoTa", (object)moTa ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NgayKhoiChieu", ngayKhoiChieu);
                        cmd.Parameters.AddWithValue("@AnhBia", (object)anhBia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DaoDien", (object)daoDien ?? DBNull.Value);
                        if (ViewState["MaPhim"] != null)
                        {
                            cmd.Parameters.AddWithValue("@MaPhim", ViewState["MaPhim"]);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lưu phim thành công!');", true);
                LoadMovies();
                pnlAddEditMovie.Visible = false;
                ClearForm();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi lưu phim: {ex.Message}');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int maPhim = Convert.ToInt32(btn.CommandArgument);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Kiểm tra xem phim có lịch chiếu không
                    string checkQuery = "SELECT COUNT(*) FROM LichChieu WHERE MaPhim = @MaPhim";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@MaPhim", maPhim);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không thể xóa phim vì đã có lịch chiếu!');", true);
                            return;
                        }
                    }

                    // Xóa phim
                    string query = "DELETE FROM Phim WHERE MaPhim = @MaPhim";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPhim", maPhim);
                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Xóa phim thành công!');", true);
                LoadMovies();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi xóa phim: {ex.Message}');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlAddEditMovie.Visible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            txtTenPhim.Text = "";
            txtTheLoai.Text = "";
            txtThoiLuong.Text = "";
            txtMoTa.Text = "";
            txtNgayKhoiChieu.Text = "";
            txtDaoDien.Text = "";
            hfAnhBia.Value = "";
        }
    }
}