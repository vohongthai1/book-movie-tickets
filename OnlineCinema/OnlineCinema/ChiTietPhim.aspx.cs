using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnlineCinema
{
    public partial class ChiTietPhim : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string maPhim = Request.QueryString["MaPhim"];
                if (string.IsNullOrEmpty(maPhim))
                {
                    Response.Redirect("Default.aspx");
                    return;
                }

                LoadMovieDetails(maPhim);
                LoadShowtimes(maPhim);
            }
        }

        // Thêm method để kiểm tra vai trò quản trị viên
        protected bool IsAdmin()
        {
            return Session["VaiTro"] != null && Session["VaiTro"].ToString() == "QuanTriVien";
        }

        private void LoadMovieDetails(string maPhim)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT TenPhim, TheLoai, ThoiLuong, MoTa, NgayKhoiChieu, AnhBia, DaoDien FROM Phim WHERE MaPhim = @MaPhim";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhim", maPhim);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblTenPhim.Text = reader["TenPhim"].ToString();
                            lblTheLoai.Text = reader["TheLoai"].ToString();
                            lblThoiLuong.Text = reader["ThoiLuong"].ToString();
                            lblMoTa.Text = reader["MoTa"].ToString();
                            lblNgayKhoiChieu.Text = Convert.ToDateTime(reader["NgayKhoiChieu"]).ToString("dd/MM/yyyy");
                            lblDaoDien.Text = reader["DaoDien"].ToString();
                            imgAnhBia.ImageUrl = reader["AnhBia"].ToString();
                        }
                        else
                        {
                            Response.Redirect("Default.aspx");
                        }
                    }
                }
            }
        }

        private void LoadShowtimes(string maPhim)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT lc.MaLichChieu, pc.TenPhong, lc.ThoiGianBatDau, lc.GiaVe, pc.SucChua,
                   (pc.SucChua - COALESCE(SUM(v.SoLuongVe), 0)) AS SoVeConLai
            FROM LichChieu lc
            INNER JOIN PhongChieu pc ON lc.MaPhong = pc.MaPhong
            LEFT JOIN Ve v ON lc.MaLichChieu = v.MaLichChieu AND v.TrangThai = 1
            WHERE lc.MaPhim = @MaPhim
            GROUP BY lc.MaLichChieu, pc.TenPhong, lc.ThoiGianBatDau, lc.GiaVe, pc.SucChua
            ORDER BY lc.ThoiGianBatDau ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // ép kiểu đúng với DB
                    if (int.TryParse(maPhim, out int maPhimInt))
                        cmd.Parameters.Add("@MaPhim", SqlDbType.Int).Value = maPhimInt;
                    else
                        cmd.Parameters.Add("@MaPhim", SqlDbType.NVarChar).Value = maPhim;

                    conn.Open();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        rptShowtimes.DataSource = dt;
                        rptShowtimes.DataBind();
                        pnlShowtimes.Visible = true;
                        lblNoShowtimes.Visible = false;
                    }
                    else
                    {
                        pnlShowtimes.Visible = false;
                        lblNoShowtimes.Visible = true;
                        lblNoShowtimes.Text = "Phim này hiện chưa có lịch chiếu.";
                    }
                }
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu là quản trị viên thì không cho phép chọn ghế
            if (IsAdmin())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Quản trị viên không thể chọn ghế!');", true);
                return;
            }

            Button btn = (Button)sender;
            string maLichChieu = btn.CommandArgument;

            //if (Session["UserID"] == null)
            //{
            //    // Redirect to login if not logged in
            //    Response.Redirect("DangKy_DangNhap.aspx?msg=Vui lòng đăng nhập để chọn ghế&returnUrl=ChiTietPhim.aspx?MaPhim=" + Request.QueryString["MaPhim"]);
            //    return;
            //}

            //// Check available tickets
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();
            //    string query = @"
            //        SELECT (pc.SucChua - COALESCE(SUM(v.SoLuongVe), 0)) AS SoVeConLai
            //        FROM LichChieu lc
            //        INNER JOIN PhongChieu pc ON lc.MaPhong = pc.MaPhong
            //        LEFT JOIN Ve v ON lc.MaLichChieu = v.MaLichChieu AND v.TrangThai = 1
            //        WHERE lc.MaLichChieu = @MaLichChieu
            //        GROUP BY pc.SucChua";
            //    using (SqlCommand cmd = new SqlCommand(query, conn))
            //    {
            //        cmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
            //        object result = cmd.ExecuteScalar();
            //        if (result == null || Convert.ToInt32(result) <= 0)
            //        {
            //            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không còn vé cho lịch chiếu này!');", true);
            //            return;
            //        }
            //    }
            //}

            // Redirect to seat selection page
            Response.Redirect("ChonGhe.aspx?MaLichChieu=" + maLichChieu);
        }
    }
}