using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using OnlineCinema.Models;

namespace OnlineCinema
{
    public partial class ChonGhe : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string maLichChieu = Request.QueryString["MaLichChieu"];
                string maVe = Request.QueryString["MaVe"];
                if (string.IsNullOrEmpty(maLichChieu) || !int.TryParse(maLichChieu, out int lichChieuId))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Mã lịch chiếu không hợp lệ.'); window.location='Default.aspx';", true);
                    return;
                }

                try
                {
                    LoadShowtimeDetails(maLichChieu);
                    LoadSeats(maLichChieu, maVe);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Đã xảy ra lỗi: {ex.Message}'); window.location='Default.aspx';", true);
                }
            }
        }

        private void LoadShowtimeDetails(string maLichChieu)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT p.TenPhim, pc.TenPhong, lc.ThoiGianBatDau, lc.GiaVe,
                           (pc.SucChua - COALESCE(SUM(v.SoLuongVe), 0)) AS SoVeConLai
                    FROM LichChieu lc
                    INNER JOIN Phim p ON lc.MaPhim = p.MaPhim
                    INNER JOIN PhongChieu pc ON lc.MaPhong = pc.MaPhong
                    LEFT JOIN Ve v ON lc.MaLichChieu = v.MaLichChieu AND v.TrangThai = 1
                    WHERE lc.MaLichChieu = @MaLichChieu
                    GROUP BY p.TenPhim, pc.TenPhong, lc.ThoiGianBatDau, lc.GiaVe, pc.SucChua";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblTenPhim.Text = Server.HtmlEncode(reader["TenPhim"].ToString());
                            lblTenPhong.Text = Server.HtmlEncode(reader["TenPhong"].ToString());
                            lblThoiGian.Text = Convert.ToDateTime(reader["ThoiGianBatDau"]).ToString("dd/MM/yyyy HH:mm");
                            lblGiaVe.Text = string.Format("{0:N0}", reader["GiaVe"]);
                            lblSoVeConLai.Text = reader["SoVeConLai"].ToString();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lịch chiếu không tồn tại.'); window.location='Default.aspx';", true);
                        }
                    }
                }
            }
        }

        private void LoadSeats(string maLichChieu, string maVe)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT g.MaGhe, g.SoGhe, CASE WHEN v.MaGhe IS NOT NULL THEN 1 ELSE 0 END AS TrangThai
                    FROM Ghe g
                    LEFT JOIN Ve v ON g.MaGhe = v.MaGhe AND v.MaLichChieu = @MaLichChieu AND v.TrangThai = 1
                    WHERE g.MaPhong IN (SELECT MaPhong FROM LichChieu WHERE MaLichChieu = @MaLichChieu)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
                    conn.Open();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        rptSeats.DataSource = dt;
                        rptSeats.DataBind();
                        pnlSeats.Visible = true;
                        lblNoSeats.Visible = false;

                        // Load selected seats from Session if editing
                        if (!string.IsNullOrEmpty(maVe))
                        {
                            var cart = Session["Cart"] as List<CartItem>;
                            if (cart != null)
                            {
                                var item = cart.FirstOrDefault(c => c.MaVe.ToString() == maVe);
                                if (item != null)
                                {
                                    lblSelectedSeats.Text = string.IsNullOrEmpty(item.MaGhe) ? "Chưa chọn ghế" : item.MaGhe;
                                    btnAddToCart.Enabled = !string.IsNullOrEmpty(item.MaGhe);
                                }
                            }
                        }
                    }
                    else
                    {
                        pnlSeats.Visible = false;
                        lblNoSeats.Visible = true;
                        btnAddToCart.Enabled = false;
                    }
                }
            }
        }

        protected void rptSeats_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var btnSeat = e.Item.FindControl("btnSeat") as Button;
                var cart = Session["Cart"] as List<CartItem>;
                string maVe = Request.QueryString["MaVe"];
                if (cart != null && !string.IsNullOrEmpty(maVe))
                {
                    var item = cart.FirstOrDefault(c => c.MaVe.ToString() == maVe);
                    if (item != null && btnSeat != null && item.MaGhe.Contains(btnSeat.CommandArgument))
                    {
                        btnSeat.CssClass = "seat seat-selected";
                    }
                }
            }
        }

        protected void btnSeat_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                string maGhe = btn.CommandArgument;
                string maLichChieu = Request.QueryString["MaLichChieu"];
                string maVe = Request.QueryString["MaVe"];
                var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

                CartItem item;
                if (!string.IsNullOrEmpty(maVe))
                {
                    item = cart.FirstOrDefault(c => c.MaVe.ToString() == maVe);
                    if (item == null)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Vé không tồn tại trong giỏ hàng.');", true);
                        return;
                    }
                }
                else
                {
                    item = cart.FirstOrDefault(c => c.MaLichChieu.ToString() == maLichChieu);
                    if (item == null)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = @"
                                SELECT p.TenPhim, pc.TenPhong, lc.ThoiGianBatDau, lc.GiaVe
                                FROM LichChieu lc
                                INNER JOIN Phim p ON lc.MaPhim = p.MaPhim
                                INNER JOIN PhongChieu pc ON lc.MaPhong = pc.MaPhong
                                WHERE lc.MaLichChieu = @MaLichChieu";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        item = new CartItem
                                        {
                                            MaVe = cart.Any() ? cart.Max(c => c.MaVe) + 1 : 1,
                                            MaLichChieu = Convert.ToInt32(maLichChieu),
                                            TenPhim = reader["TenPhim"].ToString(),
                                            TenPhong = reader["TenPhong"].ToString(),
                                            ThoiGianBatDau = Convert.ToDateTime(reader["ThoiGianBatDau"]),
                                            GiaVe = Convert.ToDecimal(reader["GiaVe"]),
                                            SoLuongVe = 1,
                                            MaGhe = ""
                                        };
                                        cart.Add(item);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lịch chiếu không tồn tại.');", true);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                // Toggle seat selection
                var selectedSeats = item.MaGhe.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
                if (selectedSeats.Contains(maGhe))
                {
                    selectedSeats.Remove(maGhe);
                    btn.CssClass = btn.CssClass.Replace("seat-selected", "seat-available");
                }
                else
                {
                    selectedSeats.Add(maGhe);
                    btn.CssClass = btn.CssClass.Replace("seat-available", "seat-selected");
                }
                item.MaGhe = string.Join(",", selectedSeats);
                item.SoLuongVe = selectedSeats.Count;

                Session["Cart"] = cart;
                lblSelectedSeats.Text = string.IsNullOrEmpty(item.MaGhe) ? "Chưa chọn ghế" : item.MaGhe;
                btnAddToCart.Enabled = selectedSeats.Any();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi khi chọn ghế: {ex.Message}');", true);
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            try
            {
                string maLichChieu = Request.QueryString["MaLichChieu"];
                var cart = Session["Cart"] as List<CartItem>;
                var item = cart?.FirstOrDefault(c => c.MaLichChieu.ToString() == maLichChieu);
                if (item == null || string.IsNullOrEmpty(item.MaGhe))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Vui lòng chọn ít nhất một ghế.');", true);
                    return;
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Thêm vé vào giỏ hàng thành công!');", true);
                Response.Redirect("GioHang.aspx");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi khi thêm vào giỏ hàng: {ex.Message}');", true);
            }
        }
    }
}