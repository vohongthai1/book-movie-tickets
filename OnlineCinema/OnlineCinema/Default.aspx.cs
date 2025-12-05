using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using OnlineCinema.Models;
using System.Linq;

namespace OnlineCinema
{
    public partial class Default : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    LoadMovies();
                    UpdateCartCount();
                    UpdateUIBasedOnRole();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Đã xảy ra lỗi: {ex.Message}');", true);
                }
            }
        }

        // Thêm method để kiểm tra vai trò quản trị viên
        protected bool IsAdmin()
        {
            return Session["VaiTro"] != null && Session["VaiTro"].ToString() == "QuanTriVien";
        }

        // Thêm method để cập nhật UI dựa trên vai trò
        private void UpdateUIBasedOnRole()
        {
            if (IsAdmin())
            {
                divCartIcon.Visible = false;
            }
            else
            {
                divCartIcon.Visible = true;
            }
        }

        private void LoadMovies()
        {
            // Giữ nguyên mã hiện tại
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT p.MaPhim, p.TenPhim, p.TheLoai, p.ThoiLuong, p.AnhBia, 
                                    MIN(l.GiaVe) AS GiaVe
                                    FROM Phim p
                                    LEFT JOIN LichChieu l ON p.MaPhim = l.MaPhim";
                    string condition = "";
                    if (!string.IsNullOrEmpty(txtSearch.Text))
                    {
                        condition += " WHERE p.TenPhim LIKE '%' + @Search + '%'";
                    }
                    if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                    {
                        condition += string.IsNullOrEmpty(condition) ? " WHERE " : " AND ";
                        condition += "p.TheLoai = @Category";
                    }
                    query += condition;
                    query += " GROUP BY p.MaPhim, p.TenPhim, p.TheLoai, p.ThoiLuong, p.AnhBia";
                    string sortBy = ddlSortBy.SelectedValue;
                    string[] validSorts = { "p.TenPhim", "MIN(l.GiaVe)", "p.ThoiLuong", "p.NgayKhoiChieu DESC" };
                    if (!Array.Exists(validSorts, s => s == sortBy))
                    {
                        sortBy = "p.TenPhim";
                    }
                    query += " ORDER BY " + sortBy;
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(txtSearch.Text))
                            cmd.Parameters.AddWithValue("@Search", txtSearch.Text);
                        if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                            cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            rptPhimDangChieu.DataSource = dt;
                            rptPhimDangChieu.DataBind();
                            if (!string.IsNullOrEmpty(txtSearch.Text))
                            {
                                pnlSearchResults.Visible = true;
                                litSearchQuery.Text = Server.HtmlEncode(txtSearch.Text);
                                litResultCount.Text = $"Tìm thấy {dt.Rows.Count} kết quả.";
                            }
                            else
                            {
                                pnlSearchResults.Visible = false;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi cơ sở dữ liệu: {ex.Message}');", true);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Vui lòng nhập từ khóa tìm kiếm!');", true);
            }

            LoadMovies();
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMovies();
        }

        protected void ddlSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMovies();
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu là quản trị viên thì không cho phép thêm vào giỏ
            if (IsAdmin())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Quản trị viên không thể thêm vé vào giỏ hàng!');", true);
                return;
            }

            Button btn = (Button)sender;
            string maLichChieu = btn.CommandArgument;

            // Kiểm tra đăng nhập (có thể bỏ comment nếu muốn bắt buộc đăng nhập)
            //if (Session["UserID"] == null)
            //{
            //    Response.Redirect("DangKy_DangNhap.aspx?msg=Vui lòng đăng nhập để chọn ghế&returnUrl=ChiTietPhim.aspx?MaPhim=" + Request.QueryString["MaPhim"]);
            //    return;
            //}

            // Kiểm tra vé còn lại (có thể bỏ comment nếu muốn kiểm tra chặt chẽ)
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT (pc.SucChua - COALESCE(SUM(v.SoLuongVe), 0)) AS SoVeConLai
            FROM LichChieu lc
            INNER JOIN PhongChieu pc ON lc.MaPhong = pc.MaPhong
            LEFT JOIN Ve v ON lc.MaLichChieu = v.MaLichChieu AND v.TrangThai = 1
            WHERE lc.MaLichChieu = @MaLichChieu
            GROUP BY pc.SucChua";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
                    object result = cmd.ExecuteScalar();
                    if (result == null || Convert.ToInt32(result) <= 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không còn vé cho lịch chiếu này!');", true);
                        return;
                    }
                }
            }

            // Chuyển đến trang chọn ghế
            Response.Redirect("ChonGhe.aspx?MaLichChieu=" + maLichChieu);
        }

        protected void lnkCart_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu là quản trị viên thì không cho truy cập giỏ hàng
            if (IsAdmin())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Quản trị viên không có quyền truy cập giỏ hàng!');", true);
                return;
            }
            Response.Redirect("GioHang.aspx");
        }

        private void UpdateCartCount()
        {
            try
            {
                // Chỉ cập nhật cart count nếu không phải quản trị viên
                if (!IsAdmin())
                {
                    var cart = Session["Cart"] as List<CartItem>;
                    cartCount.InnerText = cart != null ? cart.Sum(c => c.SoLuongVe).ToString() : "0";
                }
                else
                {
                    cartCount.InnerText = "0";
                }
            }
            catch (Exception ex)
            {
                cartCount.InnerText = "0";
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi cập nhật giỏ hàng: {ex.Message}');", true);
            }
        }

        private string GetMovieName(int maPhim)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT TenPhim FROM Phim WHERE MaPhim = @MaPhim";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPhim", maPhim);
                        object result = cmd.ExecuteScalar();
                        return result != null ? result.ToString() : "Không xác định";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Không xác định";
            }
        }
    }
}