<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLy_PhongChieu.aspx.cs" Inherits="OnlineCinema.QuanLy_PhongChieu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/ql-phongchieu.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="manage-room-section">
        <div class="container">
            <h2>Quản Lý Phòng Chiếu</h2>

            <!-- Nút thêm phòng chiếu -->
            <div class="add-room-btn">
                <asp:Button ID="btnAddRoom" runat="server" Text="Thêm Phòng Chiếu" CssClass="btn-add" OnClick="btnAddRoom_Click" />
            </div>

            <!-- Form thêm/sửa phòng chiếu (ẩn mặc định) -->
            <asp:Panel ID="pnlAddEditRoom" runat="server" Visible="false" CssClass="room-form">
                <h3><asp:Label ID="lblFormTitle" runat="server" Text="Thêm Phòng Chiếu Mới"></asp:Label></h3>
                <div class="form-group">
                    <label>Tên Phòng:</label>
                    <asp:TextBox ID="txtTenPhong" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTenPhong" runat="server" ControlToValidate="txtTenPhong"
                        ErrorMessage="Vui lòng nhập tên phòng!" CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Sức Chứa:</label>
                    <asp:TextBox ID="txtSucChua" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSucChua" runat="server" ControlToValidate="txtSucChua"
                        ErrorMessage="Vui lòng nhập sức chứa!" CssClass="error-message" Display="Dynamic" />
                    <asp:RangeValidator ID="rvSucChua" runat="server" ControlToValidate="txtSucChua"
                        MinimumValue="1" MaximumValue="1000" Type="Integer" ErrorMessage="Sức chứa phải từ 1 đến 1000!"
                        CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn-save" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn-cancel" OnClick="btnCancel_Click" CausesValidation="false" />
                </div>
            </asp:Panel>

            <!-- Bảng danh sách phòng chiếu -->
            <asp:Panel ID="pnlRoomList" runat="server" CssClass="room-list">
                <asp:Repeater ID="rptRooms" runat="server">
                    <HeaderTemplate>
                        <table class="room-table">
                            <thead>
                                <tr>
                                    <th>Mã Phòng</th>
                                    <th>Tên Phòng</th>
                                    <th>Sức Chứa</th>
                                    <th>Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("MaPhong") %></td>
                            <td><%# Server.HtmlEncode(Eval("TenPhong").ToString()) %></td>
                            <td><%# Eval("SucChua") %></td>
                            <td>
                                <asp:Button ID="btnEdit" runat="server" Text="Sửa" CommandArgument='<%# Eval("MaPhong") %>' OnClick="btnEdit_Click" CssClass="btn-edit" />
                                <asp:Button ID="btnDelete" runat="server" Text="Xóa" CommandArgument='<%# Eval("MaPhong") %>' OnClick="btnDelete_Click" CssClass="btn-delete" OnClientClick="return confirm('Bạn có chắc muốn xóa phòng chiếu này?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblEmptyRooms" runat="server" Text="Không có phòng chiếu nào." Visible="false" CssClass="empty-message"></asp:Label>
            </asp:Panel>
        </div>
    </section>
</asp:Content>