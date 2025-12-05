<%@ Page Title="Vé Đã Đặt" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="VeDaDat.aspx.cs" Inherits="OnlineCinema.VeDaDat" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/vedadat.css" rel="stylesheet" />
    <style>
        /* ... (CSS cho các tab giữ nguyên) ... */
        .ticket-tabs { margin-bottom: 20px; border-bottom: 2px solid #eee; padding-bottom: 5px; }
        .tab-btn { padding: 10px 18px; border: none; background-color: transparent; font-size: 16px; cursor: pointer; color: #555; border-bottom: 3px solid transparent; margin-bottom: -7px; }
        .tab-btn:hover { background-color: #f9f9f9; color: #000; }
        .tab-btn.active { font-weight: bold; color: #007bff; border-bottom-color: #007bff; }
        .btn-view { display: inline-block; padding: 6px 12px; background-color: #007bff; color: white; text-decoration: none; border-radius: 4px; margin-right: 5px; }
        .btn-view:hover { background-color: #0056b3; }
        
        /* CSS CHO TRẠNG THÁI MỚI */
        .status-cho-thanh-toan { color: #fd7e14; font-weight: bold; } /* Màu cam */
        .status-da-huy { color: #dc3545; font-weight: bold; } /* Màu đỏ */
        .status-da-thanh-toan { color: #28a745; font-weight: bold; } /* Màu xanh lá */
        .status-da-xem { color: #6c757d; } /* Màu xám */
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="booked-tickets-section">
        <div class="container">
            <h2>Vé Đã Đặt</h2>
            <div class="ticket-tabs">
                <asp:Button ID="btnSapChieu" runat="server" Text="Sắp chiếu / Đang chờ" CssClass="tab-btn active" 
                    OnClick="btnTab_Click" CommandArgument="1" CausesValidation="false" />
                <asp:Button ID="btnDaXem" runat="server" Text="Đã xem" CssClass="tab-btn" 
                    OnClick="btnTab_Click" CommandArgument="2" CausesValidation="false" />
                <asp:Button ID="btnDaHuy" runat="server" Text="Đã hủy" CssClass="tab-btn" 
                    OnClick="btnTab_Click" CommandArgument="0" CausesValidation="false" />
                <asp:HiddenField ID="hdCurrentTab" runat="server" Value="1" />
            </div>

            <asp:Panel ID="pnlTickets" runat="server" CssClass="ticket-list">
                <asp:Repeater ID="rptTickets" runat="server">
                    <HeaderTemplate>
                        <table class="ticket-table">
                            <thead><tr><th>Phim</th><th>Rạp / Phòng</th><th>Thời Gian</th><th>Ghế</th><th>Trạng Thái</th><th>Thao Tác</th></tr></thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("TenPhim") %></td>
                            <td><%# Eval("TenPhong") %></td>
                            <td><%# Eval("ThoiGianBatDau", "{0:dd/MM/yyyy HH:mm}") %></td>
                            <td><%# Eval("SoGhe") %></td>
                            <td>
                                <%# GetTrangThaiVe(Convert.ToInt32(Eval("TrangThaiVe")), Convert.ToDateTime(Eval("ThoiGianBatDau"))) %>
                            </td>
                            <td>
                                <%-- 
                                    LOGIC QUAN TRỌNG NHẤT:
                                    Chỉ hiển thị nút "Xem Vé" (để vào rạp) khi vé đã được admin duyệt (TrangThai = 1).
                                    Vé đang "Chờ thanh toán" (TrangThai = 2) sẽ KHÔNG thấy nút này.
                               
                                
                                <%-- Nút Hủy (Chỉ hiện khi vé đã xác nhận VÀ chưa chiếu) --%>
                                <asp:Button ID="btnCancel" runat="server" Text="Hủy Vé" 
                                    CommandArgument='<%# Eval("MaVe") %>' 
                                    OnClick="btnCancel_Click" CssClass="btn-cancel" 
                                    Visible='<%# Convert.ToInt32(Eval("TrangThaiVe")) == 1 && Convert.ToDateTime(Eval("ThoiGianBatDau")) > DateTime.Now %>'
                                    OnClientClick="return confirm('Bạn có chắc muốn hủy vé này?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblEmptyTickets" runat="server" Text="Bạn chưa có vé nào." Visible="false" CssClass="empty-tickets"></asp:Label>
            </asp:Panel>
        </div>
    </section>
</asp:Content>