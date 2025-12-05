<%@ Page Title="Thanh Toán" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="thanhtoan.aspx.cs" Inherits="OnlineCinema.thanhtoan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .checkout-section { padding: 20px 0; }
        .summary-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
        .summary-table th, .summary-table td { border: 1px solid #ddd; padding: 10px 12px; text-align: left; }
        .summary-table th { background-color: #f9f9f9; }
        .checkout-total { text-align: right; font-size: 1.5em; font-weight: bold; color: #dc3545; margin: 20px 0; }
        
        .payment-container { display: flex; gap: 30px; background: #fdfdfd; border: 1px solid #eee; padding: 20px; border-radius: 8px; }
        .payment-info { flex: 1.5; }
        .payment-qr { flex: 1; text-align: center; background: #f8f9fa; padding: 20px; border-radius: 8px; }

        .bank-details { border: 2px dashed #007bff; background: #fff; padding: 20px; border-radius: 8px; }
        .bank-details h4 { margin-top: 0; color: #007bff; }
        .bank-details p { margin: 5px 0; }
        .bank-details .transfer-note {
            font-size: 1.2em; font-weight: bold; color: #dc3545;
            background: #fff; padding: 10px; border: 1px solid #ddd;
            border-radius: 4px; display: block; text-align: center;
        }
        .note-group { margin-top: 20px; }
        .note-group label { display: block; margin-bottom: 5px; font-weight: bold; }
        .note-group textarea { width: 95%; min-height: 80px; padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
        .qr-code { max-width: 250px; border: 1px solid #ddd; padding: 5px; }
        .payment-qr h4 { margin-top: 0; }
        .btn-confirm-payment { padding: 12px 25px; font-size: 1.1em; background-color: #28a745; color: white; border: none; cursor: pointer; border-radius: 5px; float: right; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="checkout-section">
        <div class="container">
            <h2>Thanh Toán Đơn Hàng</h2>
            
            <asp:Panel ID="pnlSummary" runat="server">
                <h3>Tóm Tắt Đơn Hàng</h3>
                <asp:Repeater ID="rptCartSummary" runat="server">
                    <HeaderTemplate>
                        <table class="summary-table">
                            <thead><tr><th>Phim</th><th>Rạp / Phòng</th><th>Thời Gian</th><th>Ghế (Mã)</th><th>Số Lượng</th><th>Tổng</th></tr></thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("TenPhim") %></td>
                            <td><%# Eval("TenPhong") %></td>
                            <td><%# Convert.ToDateTime(Eval("ThoiGianBatDau")).ToString("dd/MM/yyyy HH:mm") %></td>
                            <td><%# Server.HtmlEncode(Eval("MaGhe").ToString()) %></td>
                            <td><%# Eval("SoLuongVe") %></td>
                            <td><%# Convert.ToDecimal(Eval("TongTien")).ToString("N0") %> VNĐ</td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>

            <div class="payment-container">
                <div class="payment-info">
                    <asp:Panel ID="pnlBankTransferDetails" runat="server" CssClass="bank-details">
                        <h4>Thông tin tài khoản nhận</h4>
                        <p>Vui lòng chuyển khoản chính xác thông tin dưới đây:</p>
                        <p><asp:Label ID="lblBankInfo" runat="server" Text="Đang tải..."></asp:Label></p>
                        <hr />
                        <p><strong>Nội dung chuyển khoản (Bắt buộc):</strong></p>
                        <asp:TextBox ID="txtTransferNote" runat="server" ReadOnly="true" CssClass="transfer-note"></asp:TextBox>
                    </asp:Panel>
                    
                    <div class="note-group">
                        <label for="<%= txtOrderNote.ClientID %>">Ghi chú đơn hàng (nếu có)</label>
                        <asp:TextBox ID="txtOrderNote" runat="server" TextMode="MultiLine" Rows="3" CssClass="note-group textarea"></asp:TextBox>
                    </div>
                </div>

                <div class="payment-qr">
                     <h4>Quét mã VietQR để thanh toán</h4>
                     <p><i>(Bao gồm chính xác số tiền và nội dung)</i></p>
                     <asp:Image ID="imgQR" runat="server" CssClass="qr-code" ImageUrl="~/images/maQR.png" />
                     <p style="margin-top: 15px; color: #555;">Sau khi chuyển khoản thành công, vui lòng nhấn nút "Xác nhận" bên dưới.</p>
                </div>
            </div>
            
            <div class="checkout-total">
                <span>Tổng cộng:</span>
                <asp:Label ID="lblTotal" runat="server" Text="0 VNĐ"></asp:Label>
            </div>

            <asp:Button ID="btnConfirmPayment" runat="server" Text="Tôi đã chuyển khoản / Xác nhận đặt vé" 
                CssClass="btn-confirm-payment" OnClick="btnConfirmPayment_Click" 
                OnClientClick="this.disabled=true; this.value='Đang xử lý...';" UseSubmitBehavior="false" />
        </div>
    </section>
</asp:Content>