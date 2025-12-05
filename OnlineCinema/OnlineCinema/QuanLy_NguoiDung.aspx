<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLy_NguoiDung.aspx.cs" Inherits="OnlineCinema.QuanLy_NguoiDung" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/ql-nguoidung.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="manage-user-section">
        <div class="container">
            <h2>Quản Lý Người Dùng</h2>

            <!-- Nút thêm người dùng -->
            <div class="add-user-btn">
                <asp:Button ID="btnAddUser" runat="server" Text="Thêm Người Dùng" CssClass="btn-add" OnClick="btnAddUser_Click" />
            </div>

            <!-- Form thêm/sửa người dùng (ẩn mặc định) -->
            <asp:Panel ID="pnlAddEditUser" runat="server" Visible="false" CssClass="user-form">
                <h3><asp:Label ID="lblFormTitle" runat="server" Text="Thêm Người Dùng Mới"></asp:Label></h3>
                <div class="form-group">
                    <label>Họ Tên:</label>
                    <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvHoTen" runat="server" ControlToValidate="txtHoTen"
                        ErrorMessage="Vui lòng nhập họ tên!" CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Email:</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                        ErrorMessage="Vui lòng nhập email!" CssClass="error-message" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                        ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" ErrorMessage="Email không hợp lệ!"
                        CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Số Điện Thoại:</label>
                    <asp:TextBox ID="txtSoDienThoai" runat="server" CssClass="form-control" TextMode="Phone"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSoDienThoai" runat="server" ControlToValidate="txtSoDienThoai"
                        ErrorMessage="Vui lòng nhập số điện thoại!" CssClass="error-message" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revSoDienThoai" runat="server" ControlToValidate="txtSoDienThoai"
                        ValidationExpression="^\d{10}$" ErrorMessage="Số điện thoại phải có 10 chữ số!"
                        CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Mật Khẩu:</label>
                    <asp:TextBox ID="txtMatKhau" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvMatKhau" runat="server" ControlToValidate="txtMatKhau"
                        ErrorMessage="Vui lòng nhập mật khẩu!" CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn-save" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn-cancel" OnClick="btnCancel_Click" CausesValidation="false" />
                </div>
            </asp:Panel>

            <!-- Bảng danh sách người dùng -->
            <asp:Panel ID="pnlUserList" runat="server" CssClass="user-list">
                <asp:Repeater ID="rptUsers" runat="server">
                    <HeaderTemplate>
                        <table class="user-table">
                            <thead>
                                <tr>
                                    <th>Mã Khách Hàng</th>
                                    <th>Họ Tên</th>
                                    <th>Email</th>
                                    <th>Số Điện Thoại</th>
                                    <th>Ngày Đăng Ký</th>
                                    <th>Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("MaKH") %></td>
                            <td><%# Server.HtmlEncode(Eval("HoTen").ToString()) %></td>
                            <td><%# Server.HtmlEncode(Eval("Email").ToString()) %></td>
                            <td><%# Server.HtmlEncode(Eval("SoDienThoai").ToString()) %></td>
                            <td><%# Eval("NgayDangKy", "{0:dd/MM/yyyy}") %></td>
                            <td>
                                <asp:Button ID="btnEdit" runat="server" Text="Sửa" CommandArgument='<%# Eval("MaKH") %>' OnClick="btnEdit_Click" CssClass="btn-edit" />
                                <asp:Button ID="btnDelete" runat="server" Text="Xóa" CommandArgument='<%# Eval("MaKH") %>' OnClick="btnDelete_Click" CssClass="btn-delete" OnClientClick="return confirm('Bạn có chắc muốn xóa người dùng này?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblEmptyUsers" runat="server" Text="Không có người dùng nào." Visible="false" CssClass="empty-message"></asp:Label>
            </asp:Panel>
        </div>
    </section>
</asp:Content>