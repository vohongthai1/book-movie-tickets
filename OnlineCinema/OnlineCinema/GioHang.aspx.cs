using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using OnlineCinema.Models;

namespace OnlineCinema
{
    public partial class GioHang : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCart();
            }
        }

        private void LoadCart()
        {
            try
            {
                var cart = Session["Cart"] as List<CartItem>;
                if (cart != null && cart.Any())
                {
                    rptCart.DataSource = cart;
                    rptCart.DataBind();
                    pnlCart.Visible = true;
                    lblEmptyCart.Visible = false;
                    btnCheckout.Enabled = true;
                    lblTotal.Text = cart.Sum(c => c.TongTien).ToString("N0") + " VNĐ";
                }
                else
                {
                    pnlCart.Visible = false;
                    lblEmptyCart.Visible = true;
                    btnCheckout.Enabled = false;
                    lblTotal.Text = "0 VNĐ";
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi tải giỏ hàng: {ex.Message}');", true);
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            // (Hàm này giữ nguyên như code cũ của bạn)
            try
            {
                Button btn = sender as Button;
                if (btn == null || string.IsNullOrEmpty(btn.CommandArgument)) return;
                string[] args = btn.CommandArgument.Split(',');
                if (args.Length != 2) return;
                if (!int.TryParse(args[0], out int maVe) || !int.TryParse(args[1], out int maLichChieu)) return;

                Response.Redirect($"ChonGhe.aspx?MaVe={maVe}&MaLichChieu={maLichChieu}");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi khi chỉnh sửa vé: {ex.Message}');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            // (Hàm này giữ nguyên như code cũ của bạn)
            try
            {
                Button btn = sender as Button;
                if (btn == null || string.IsNullOrEmpty(btn.CommandArgument)) return;
                if (!int.TryParse(btn.CommandArgument, out int maVe)) return;

                var cart = Session["Cart"] as List<CartItem>;
                if (cart == null || !cart.Any()) return;

                var item = cart.FirstOrDefault(c => c.MaVe == maVe);
                if (item != null)
                {
                    cart.Remove(item);
                    Session["Cart"] = cart;
                    LoadCart();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Xóa vé thành công!');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi xóa vé: {ex.Message}');", true);
            }
        }

        // === LOGIC MỚI CHO NÚT THANH TOÁN ===
        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra đăng nhập
            if (Session["UserID"] == null)
            {
                Response.Redirect("DangKy_DangNhap.aspx?msg=Vui lòng đăng nhập trước khi thanh toán");
                return;
            }

            // 2. Kiểm tra giỏ hàng có trống không
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Giỏ hàng trống. Vui lòng thêm vé trước khi thanh toán.');", true);
                return;
            }

            // 3. Chuyển hướng sang trang Thanh Toán
            Response.Redirect("thanhtoan.aspx");
        }
    }
}