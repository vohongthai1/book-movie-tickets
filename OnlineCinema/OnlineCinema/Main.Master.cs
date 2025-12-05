using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using OnlineCinema.Models;

namespace OnlineCinema
{
    public partial class Main : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateUserInterface();
                UpdateCartCount();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect(Request.RawUrl);
        }

        private void UpdateUserInterface()
        {
            if (Session["UserID"] != null && Session["UserName"] != null)
            {
                phLoginRegister.Visible = false;
                phUserProfile.Visible = true;
                lblUserName.Text = Session["UserName"].ToString();

                // Kiểm tra vai trò từ Session
                bool isAdmin = Session["VaiTro"] != null && Session["VaiTro"].ToString() == "QuanTriVien";
                if (isAdmin)
                {
                    lnkCart.Visible = false;
                    pnlUserNav.Visible = false; // Ẩn menu người dùng thường
                    pnlAdminNav.Visible = true; // Hiển thị menu quản trị viên
                }
                else
                {
                    lnkCart.Visible = true;
                    pnlUserNav.Visible = true; // Hiển thị menu người dùng thường
                    pnlAdminNav.Visible = false; // Ẩn menu quản trị viên
                }
            }
            else
            {
                phLoginRegister.Visible = true;
                phUserProfile.Visible = false;
                pnlUserNav.Visible = true; // Hiển thị menu người dùng thường
                pnlAdminNav.Visible = false; // Ẩn menu quản trị viên
            }
        }

        private void UpdateCartCount()
        {
            try
            {
                var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
                int totalItems = cart.Sum(item => item.SoLuongVe);
                lblCartCount.Text = totalItems > 0 ? totalItems.ToString() : "";
            }
            catch
            {
                lblCartCount.Text = "";
            }
        }
    }
}