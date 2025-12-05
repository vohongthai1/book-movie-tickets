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
    public partial class thanhtoan : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private string orderCode; // Mã đơn hàng duy nhất
        private decimal totalAmount; // Tổng tiền

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("DangKy_DangNhap.aspx?msg=Vui lòng đăng nhập");
                return;
            }
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                Response.Redirect("GioHang.aspx?msg=Giỏ hàng trống");
                return;
            }

            totalAmount = cart.Sum(c => c.TongTien);

            if (!IsPostBack)
            {
                // Tạo Mã Đơn Hàng duy nhất
                orderCode = $"RAPCHIM-{DateTime.Now:ddMM}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";
                Session["CurrentOrderCode"] = orderCode;

                LoadCartSummary();
                SetupBankTransferDetails();
            }
        }

        private void LoadCartSummary()
        {
            var cart = Session["Cart"] as List<CartItem>;
            rptCartSummary.DataSource = cart;
            rptCartSummary.DataBind();
            lblTotal.Text = totalAmount.ToString("N0") + " VNĐ";
        }

        private void SetupBankTransferDetails()
        {
            orderCode = Session["CurrentOrderCode"]?.ToString();
            if (string.IsNullOrEmpty(orderCode)) { orderCode = "LOIDONHANG"; }

            // --- THAY THẾ BẰNG THÔNG TIN CỦA BẠN ---
            string bankId = "970436"; // Techcombank
            string accountNo = "034XXXX278"; // STK của bạn
            string accountName = "HOANG BAO QUANG"; // Tên của bạn
            // ------------------------------------

            string amount = totalAmount.ToString("F0");
            string note = orderCode.Replace("-", "");

            string qrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-compact2.png?amount={amount}&addInfo={note}&accountName={accountName}";

            imgQR.ImageUrl = qrUrl;
            lblBankInfo.Text = $"Ngân hàng: Techcombank (TCB)<br />Số tài khoản: {accountNo}<br />Chủ tài khoản: {accountName}"; // Nhớ sửa tên NH
            txtTransferNote.Text = note;
        }

        protected void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            int maKH = Convert.ToInt32(Session["UserID"]);
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any()) { /*... (kiểm tra lại) ...*/ return; }

            string phuongThucTT = "Chuyển khoản ngân hàng"; // Cố định
            string ghiChu = txtOrderNote.Text;
            string maDonHang = Session["CurrentOrderCode"]?.ToString();
            if (string.IsNullOrEmpty(maDonHang)) { /*... (báo lỗi) ...*/ return; }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    foreach (var item in cart)
                    {
                        string[] maGheList = item.MaGhe.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();

                        foreach (string maGheStr in maGheList)
                        {
                            if (!int.TryParse(maGheStr, out int maGhe)) continue;

                            // Kiểm tra ghế xem đã bị giữ (1) hoặc đang chờ (2) chưa
                            string checkQuery = "SELECT COUNT(*) FROM Ve WHERE MaLichChieu = @MaLichChieu AND MaGhe = @MaGhe AND (TrangThai = 1 OR TrangThai = 2)";
                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@MaLichChieu", item.MaLichChieu);
                                checkCmd.Parameters.AddWithValue("@MaGhe", maGhe);
                                int count = (int)checkCmd.ExecuteScalar();
                                if (count > 0)
                                {
                                    throw new Exception($"Một trong các ghế bạn chọn đã bị giữ hoặc đã bán. Vui lòng chọn lại.");
                                }
                            }

                            // === LOGIC CỐT LÕI ===
                            // INSERT VÉ VỚI TRẠNG THÁI = 2 (CHỜ THANH TOÁN)
                            string query = @"
                                INSERT INTO Ve (MaLichChieu, MaKH, SoLuongVe, MaGhe, NgayDat, TrangThai, MaDonHang, GhiChu, PhuongThucTT)
                                VALUES (@MaLichChieu, @MaKH, 1, @MaGhe, GETDATE(), 2, @MaDonHang, @GhiChu, @PhuongThucTT)";

                            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@MaLichChieu", item.MaLichChieu);
                                cmd.Parameters.AddWithValue("@MaKH", maKH);
                                cmd.Parameters.AddWithValue("@MaGhe", maGhe);
                                cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                                cmd.Parameters.AddWithValue("@GhiChu", ghiChu);
                                cmd.Parameters.AddWithValue("@PhuongThucTT", phuongThucTT);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    Session["Cart"] = null;
                    Session["CurrentOrderCode"] = null;

                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        $"alert('Đơn hàng của bạn đã được ghi nhận. Vui lòng chờ chúng tôi xác nhận thanh toán.'); window.location='VeDaDat.aspx';", true);
                }
                catch (Exception ex)
                {
                    try { transaction.Rollback(); } catch { }
                    ClientScript.RegisterStartupScript(this.GetType(), "re-enable_button", "document.getElementById('" + btnConfirmPayment.ClientID + "').disabled = false; document.getElementById('" + btnConfirmPayment.ClientID + "').value = 'Tôi đã chuyển khoản / Xác nhận đặt vé';", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi đặt vé: {ex.Message.Replace("'", "")}');", true);
                }
            }
        }
    }
}