using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnlineCinema
{
    public partial class QuanLy_NguoiDung : System.Web.UI.Page
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
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT MaKH, HoTen, Email, SoDienThoai, NgayDangKy FROM KhachHang ORDER BY NgayDangKy DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                rptUsers.DataSource = dt;
                                rptUsers.DataBind();
                                lblEmptyUsers.Visible = false;
                            }
                            else
                            {
                                lblEmptyUsers.Visible = true;
                                rptUsers.DataSource = null;
                                rptUsers.DataBind();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi tải danh sách người dùng: {ex.Message}');", true);
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            lblFormTitle.Text = "Thêm Người Dùng Mới";
            ClearForm();
            pnlAddEditUser.Visible = true;
            ViewState["MaKH"] = null;
            rfvMatKhau.Enabled = true;
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int maKH = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT HoTen, Email, SoDienThoai FROM KhachHang WHERE MaKH = @MaKH";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtHoTen.Text = reader["HoTen"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtSoDienThoai.Text = reader["SoDienThoai"].ToString();
                        }
                    }
                }
            }

            lblFormTitle.Text = "Sửa Người Dùng";
            pnlAddEditUser.Visible = true;
            ViewState["MaKH"] = maKH;
            rfvMatKhau.Enabled = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                string hoTen = txtHoTen.Text.Trim();
                string email = txtEmail.Text.Trim();
                string soDienThoai = txtSoDienThoai.Text.Trim();
                string matKhau = txtMatKhau.Text;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra email và số điện thoại trùng lặp
                    if (ViewState["MaKH"] == null) // Khi thêm mới
                    {
                        string checkQuery = "SELECT COUNT(*) FROM KhachHang WHERE Email = @Email";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Email", email);
                            if ((int)checkCmd.ExecuteScalar() > 0)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Email đã được sử dụng!');", true);
                                return;
                            }
                        }

                        checkQuery = "SELECT COUNT(*) FROM KhachHang WHERE SoDienThoai = @SoDienThoai";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                            if ((int)checkCmd.ExecuteScalar() > 0)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Số điện thoại đã được sử dụng!');", true);
                                return;
                            }
                        }
                    }
                    else // Khi sửa
                    {
                        string checkQuery = "SELECT COUNT(*) FROM KhachHang WHERE Email = @Email AND MaKH != @MaKH";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Email", email);
                            checkCmd.Parameters.AddWithValue("@MaKH", ViewState["MaKH"]);
                            if ((int)checkCmd.ExecuteScalar() > 0)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Email đã được sử dụng bởi người dùng khác!');", true);
                                return;
                            }
                        }

                        checkQuery = "SELECT COUNT(*) FROM KhachHang WHERE SoDienThoai = @SoDienThoai AND MaKH != @MaKH";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                            checkCmd.Parameters.AddWithValue("@MaKH", ViewState["MaKH"]);
                            if ((int)checkCmd.ExecuteScalar() > 0)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Số điện thoại đã được sử dụng bởi người dùng khác!');", true);
                                return;
                            }
                        }
                    }

                    string query;
                    if (ViewState["MaKH"] == null) // Thêm mới
                    {
                        query = @"
                            INSERT INTO KhachHang (HoTen, Email, SoDienThoai, MatKhau, NgayDangKy)
                            VALUES (@HoTen, @Email, @SoDienThoai, @MatKhau, GETDATE())";
                    }
                    else // Sửa
                    {
                        query = @"
                            UPDATE KhachHang
                            SET HoTen = @HoTen, Email = @Email, SoDienThoai = @SoDienThoai";
                        if (!string.IsNullOrEmpty(matKhau))
                        {
                            query += ", MatKhau = @MatKhau";
                        }
                        query += " WHERE MaKH = @MaKH";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", hoTen);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                        if (!string.IsNullOrEmpty(matKhau))
                        {
                            cmd.Parameters.AddWithValue("@MatKhau", matKhau);
                        }
                        if (ViewState["MaKH"] != null)
                        {
                            cmd.Parameters.AddWithValue("@MaKH", ViewState["MaKH"]);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lưu người dùng thành công!');", true);
                LoadUsers();
                pnlAddEditUser.Visible = false;
                ClearForm();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi lưu người dùng: {ex.Message}');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn == null || string.IsNullOrEmpty(btn.CommandArgument))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lỗi: Không tìm thấy mã khách hàng!');", true);
                return;
            }

            if (!int.TryParse(btn.CommandArgument, out int maKH))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lỗi: Mã khách hàng không hợp lệ!');", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ===== BƯỚC 1: XÓA TẤT CẢ VÉ CỦA KHÁCH HÀNG =====
                    string deleteVeQuery = "DELETE FROM Ve WHERE MaKH = @MaKH";
                    using (SqlCommand deleteVeCmd = new SqlCommand(deleteVeQuery, conn))
                    {
                        deleteVeCmd.Parameters.AddWithValue("@MaKH", maKH);
                        int deletedTickets = deleteVeCmd.ExecuteNonQuery();

                        if (deletedTickets > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Đã xóa {deletedTickets} vé của khách hàng MaKH={maKH}");
                        }
                    }

                    // ===== BƯỚC 2: XÓA KHÁCH HÀNG =====
                    string deleteKHQuery = "DELETE FROM KhachHang WHERE MaKH = @MaKH";
                    using (SqlCommand deleteKHCmd = new SqlCommand(deleteKHQuery, conn))
                    {
                        deleteKHCmd.Parameters.AddWithValue("@MaKH", maKH);
                        int rowsAffected = deleteKHCmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không tìm thấy người dùng để xóa!');", true);
                            return;
                        }
                    }

                    // ===== THÀNH CÔNG =====
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Xóa người dùng thành công!');", true);
                    LoadUsers();
                }
            }
            catch (SqlException sqlEx)
            {
                // Xử lý lỗi SQL cụ thể
                if (sqlEx.Number == 547) // Foreign key constraint violation
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('Không thể xóa người dùng này vì còn dữ liệu liên quan. Vui lòng liên hệ quản trị viên!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        $"alert('Lỗi SQL khi xóa người dùng: {sqlEx.Message}');", true);
                }
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('Lỗi xóa người dùng: {ex.Message}');", true);
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlAddEditUser.Visible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            txtHoTen.Text = "";
            txtEmail.Text = "";
            txtSoDienThoai.Text = "";
            txtMatKhau.Text = "";
        }
    }
}