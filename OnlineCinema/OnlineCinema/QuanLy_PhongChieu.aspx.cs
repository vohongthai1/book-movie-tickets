using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnlineCinema
{
    public partial class QuanLy_PhongChieu : System.Web.UI.Page
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
                LoadRooms();
            }
        }

        private void LoadRooms()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT MaPhong, TenPhong, SucChua FROM PhongChieu";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                rptRooms.DataSource = dt;
                                rptRooms.DataBind();
                                lblEmptyRooms.Visible = false;
                            }
                            else
                            {
                                lblEmptyRooms.Visible = true;
                                rptRooms.DataSource = null;
                                rptRooms.DataBind();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi tải danh sách phòng chiếu: {ex.Message}');", true);
            }
        }

        protected void btnAddRoom_Click(object sender, EventArgs e)
        {
            lblFormTitle.Text = "Thêm Phòng Chiếu Mới";
            ClearForm();
            pnlAddEditRoom.Visible = true;
            ViewState["MaPhong"] = null; // Đặt null để biết là thêm mới
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int maPhong = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TenPhong, SucChua FROM PhongChieu WHERE MaPhong = @MaPhong";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTenPhong.Text = reader["TenPhong"].ToString();
                            txtSucChua.Text = reader["SucChua"].ToString();
                        }
                    }
                }
            }

            lblFormTitle.Text = "Sửa Phòng Chiếu";
            pnlAddEditRoom.Visible = true;
            ViewState["MaPhong"] = maPhong;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                string tenPhong = txtTenPhong.Text.Trim();
                int sucChua = Convert.ToInt32(txtSucChua.Text);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query;
                    if (ViewState["MaPhong"] == null) // Thêm mới
                    {
                        query = @"
                            INSERT INTO PhongChieu (TenPhong, SucChua)
                            VALUES (@TenPhong, @SucChua)";
                    }
                    else // Sửa
                    {
                        query = @"
                            UPDATE PhongChieu
                            SET TenPhong = @TenPhong, SucChua = @SucChua
                            WHERE MaPhong = @MaPhong";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenPhong", tenPhong);
                        cmd.Parameters.AddWithValue("@SucChua", sucChua);
                        if (ViewState["MaPhong"] != null)
                        {
                            cmd.Parameters.AddWithValue("@MaPhong", ViewState["MaPhong"]);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lưu phòng chiếu thành công!');", true);
                LoadRooms();
                pnlAddEditRoom.Visible = false;
                ClearForm();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi lưu phòng chiếu: {ex.Message}');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int maPhong = Convert.ToInt32(btn.CommandArgument);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Kiểm tra xem phòng chiếu có lịch chiếu không
                    string checkQuery = "SELECT COUNT(*) FROM LichChieu WHERE MaPhong = @MaPhong";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@MaPhong", maPhong);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không thể xóa phòng chiếu vì đã có lịch chiếu!');", true);
                            return;
                        }
                    }

                    // Xóa phòng chiếu
                    string query = "DELETE FROM PhongChieu WHERE MaPhong = @MaPhong";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Xóa phòng chiếu thành công!');", true);
                LoadRooms();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi xóa phòng chiếu: {ex.Message}');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlAddEditRoom.Visible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            txtTenPhong.Text = "";
            txtSucChua.Text = "";
        }
    }
}