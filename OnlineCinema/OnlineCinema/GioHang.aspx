<%@ Page Title="Giỏ Hàng" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="GioHang.aspx.cs" Inherits="OnlineCinema.GioHang" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/gio-hang.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="cart-section">
        <div class="container">
            <h2>Giỏ Hàng</h2>
            <asp:Panel ID="pnlCart" runat="server" CssClass="cart-list">
                <asp:Repeater ID="rptCart" runat="server">
                    <HeaderTemplate>
                        <table class="cart-table">
                            <thead>
                                <tr>
                                    <th>Phim</th>
                                    <th>Phòng Chiếu</th>
                                    <th>Thời Gian</th>
                                    <th>Số Lượng</th>
                                    <th>Giá Vé</th>
                                    <th>Tổng</th>
                                    <th>Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Server.HtmlEncode(Eval("TenPhim").ToString()) %></td>
                            <td><%# Server.HtmlEncode(Eval("TenPhong").ToString()) %></td>
                            <td><%# Eval("ThoiGianBatDau", "{0:dd/MM/yyyy HH:mm}") %></td>
                            <td><%# Eval("SoLuongVe") %></td>
                            <td><%# Eval("GiaVe", "{0:N0}") %> VNĐ</td>
                            <td><%# Eval("TongTien", "{0:N0}") %> VNĐ</td>
                            <td>
                                <asp:Button ID="btnEdit" runat="server" Text="Sửa" 
                                            CommandArgument='<%# Eval("MaVe") + "," + Eval("MaLichChieu") %>' 
                                            OnClick="btnEdit_Click" CssClass="btn-edit" />
                                <asp:Button ID="btnDelete" runat="server" Text="Xóa" 
                                            CommandArgument='<%# Eval("MaVe") %>' 
                                            OnClick="btnDelete_Click" CssClass="btn-delete" 
                                            OnClientClick="return confirm('Bạn có chắc muốn xóa?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblEmptyCart" runat="server" Text="Giỏ hàng trống." Visible="false" CssClass="empty-cart"></asp:Label>
            </asp:Panel>
            <div class="cart-summary">
                <p><strong>Tổng cộng:</strong> <asp:Label ID="lblTotal" runat="server" Text="0 VNĐ"></asp:Label></p>
                <%-- Nút này đã có sẵn, chúng ta chỉ thay đổi logic trong file .cs --%>
                <asp:Button ID="btnCheckout" runat="server" Text="Thanh Toán" CssClass="btn-checkout" OnClick="btnCheckout_Click" Enabled="false" />
            </div>
        </div>
    </section>
</asp:Content>