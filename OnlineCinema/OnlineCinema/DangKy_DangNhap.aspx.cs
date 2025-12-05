using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace OnlineCinema
{
    public partial class DangKy_DangNhap : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    // Nếu đã đăng nhập, chuyển đến trang phù hợp
                    if (Session["VaiTro"] != null && Session["VaiTro"].ToString() == "QuanTriVien")
                    {
                        Response.Redirect("Dashboard.aspx");
                    }
                    else
                    {
                        Response.Redirect("Default.aspx");
                    }
                }

                string msg = Request.QueryString["msg"];
                if (!string.IsNullOrEmpty(msg))
                {
                    ShowNotification(msg, msg.StartsWith("Lỗi") ? "error" : "success");
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string email = txtLoginEmail.Text.Trim();
            string password = txtLoginPass.Text;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra trong bảng QuanTriVien trước
                    string queryAdmin = "SELECT MaQTV FROM QuanTriVien WHERE Email = @Email AND MatKhau = @MatKhau";
                    using (SqlCommand cmdAdmin = new SqlCommand(queryAdmin, conn))
                    {
                        cmdAdmin.Parameters.AddWithValue("@Email", email);
                        cmdAdmin.Parameters.AddWithValue("@MatKhau", password);
                        object adminResult = cmdAdmin.ExecuteScalar();
                        if (adminResult != null)
                        {
                            // Đăng nhập thành công với tư cách quản trị viên
                            int userId = Convert.ToInt32(adminResult);
                            Session["UserID"] = userId;
                            Session["UserName"] = email;
                            Session["UserEmail"] = email;
                            Session["VaiTro"] = "QuanTriVien";
                            ShowNotification("Đăng nhập quản trị viên thành công!", "success");
                            ScriptManager.RegisterStartupScript(this, GetType(), "redirectScript",
                                "setTimeout(function() { window.location.href = 'Dashboard.aspx'; }, 500);", true);
                            return;
                        }
                    }

                    // Nếu không phải quản trị viên, kiểm tra trong bảng KhachHang
                    string queryUser = "SELECT MaKH, HoTen, Email FROM KhachHang WHERE (Email = @Email OR SoDienThoai = @Email) AND MatKhau = @MatKhau";
                    using (SqlCommand cmdUser = new SqlCommand(queryUser, conn))
                    {
                        cmdUser.Parameters.AddWithValue("@Email", email);
                        cmdUser.Parameters.AddWithValue("@MatKhau", password);
                        using (SqlDataReader reader = cmdUser.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = Convert.ToInt32(reader["MaKH"]);
                                string userName = reader["HoTen"].ToString();
                                string userEmail = reader["Email"].ToString();

                                Session["UserID"] = userId;
                                Session["UserName"] = userName;
                                Session["UserEmail"] = userEmail;
                                Session["VaiTro"] = "KhachHang";

                                ShowNotification("Đăng nhập thành công!", "success");
                                ScriptManager.RegisterStartupScript(this, GetType(), "redirectScript",
                                    "setTimeout(function() { window.location.href = 'Default.aspx'; }, 500);", true);
                            }
                            else
                            {
                                ShowNotification("Lỗi: Email/SĐT hoặc mật khẩu không đúng!", "error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification("Lỗi: " + ex.Message, "error");
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string email = txtRegEmail.Text.Trim();
            string password = txtRegPass.Text;
            string hoTen = txtRegHoTen.Text.Trim();
            string soDienThoai = txtRegPhone.Text.Trim();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM KhachHang WHERE Email = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            ShowNotification("Lỗi: Email đã được sử dụng!", "error");
                            return;
                        }
                    }

                    checkQuery = "SELECT COUNT(*) FROM KhachHang WHERE SoDienThoai = @SoDienThoai";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            ShowNotification("Lỗi: Số điện thoại đã được sử dụng!", "error");
                            return;
                        }
                    }

                    string insertQuery = "INSERT INTO KhachHang (Email, MatKhau, HoTen, SoDienThoai, NgayDangKy) " +
                                         "VALUES (@Email, @MatKhau, @HoTen, @SoDienThoai, GETDATE()); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@MatKhau", password);
                        insertCmd.Parameters.AddWithValue("@HoTen", hoTen);
                        insertCmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);

                        int newUserId = Convert.ToInt32(insertCmd.ExecuteScalar());

                        Session["UserID"] = newUserId;
                        Session["UserName"] = hoTen;
                        Session["UserEmail"] = email;
                        Session["VaiTro"] = "KhachHang";

                        ShowNotification("Đăng ký thành công! Chuyển sang đăng nhập...", "success");
                        ScriptManager.RegisterStartupScript(this, GetType(), "switchTab",
                            "setTimeout(function() { document.getElementById('btnLoginTab').click(); }, 100);", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification("Lỗi: " + ex.Message, "error");
            }
        }

        private void ShowNotification(string message, string type)
        {
            pnlNotification.Visible = true;
            pnlNotification.CssClass = "notification-banner notification-" + type;
            litNotification.Text = message;
        }
    }
}