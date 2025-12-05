using System;
using System.Web.UI;

namespace OnlineCinema
{
    public partial class Admin : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra xem người dùng đã đăng nhập và có quyền admin không
            if (Session["UserID"] == null || Session["VaiTro"] == null || Session["VaiTro"].ToString() != "QuanTriVien")
            {
                Response.Redirect("DangKy_DangNhap.aspx?msg=Bạn cần đăng nhập với tài khoản quản trị viên");
                return;
            }

            if (!IsPostBack)
            {
                UpdateAdminInfo();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("DangKy_DangNhap.aspx?msg=Đăng xuất thành công");
        }

        private void UpdateAdminInfo()
        {
            if (Session["UserEmail"] != null)
            {
                lblAdminEmail.Text = Session["UserEmail"].ToString();
                lblAdminName.Text = Session["UserName"] != null ? Session["UserName"].ToString() : "Admin";
            }
        }
    }
}