<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLy_LichChieu.aspx.cs" Inherits="OnlineCinema.QuanLy_LichChieu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/ql-lichchieu.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="manage-schedule-section">
        <div class="container">
            <h2>Quản Lý Lịch Chiếu</h2>

            <!-- Nút thêm lịch chiếu -->
            <div class="add-schedule-btn">
                <asp:Button ID="btnAddSchedule" runat="server" Text="Thêm Lịch Chiếu" CssClass="btn-add" OnClick="btnAddSchedule_Click" />
            </div>

            <!-- Form thêm/sửa lịch chiếu (ẩn mặc định) -->
            <asp:Panel ID="pnlAddEditSchedule" runat="server" Visible="false" CssClass="schedule-form">
                <h3><asp:Label ID="lblFormTitle" runat="server" Text="Thêm Lịch Chiếu Mới"></asp:Label></h3>
                <div class="form-group">
                    <label>Phim:</label>
                    <asp:DropDownList ID="ddlPhim" runat="server" CssClass="form-control" AutoPostBack="false"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvPhim" runat="server" ControlToValidate="ddlPhim"
                        InitialValue="" ErrorMessage="Vui lòng chọn phim!" CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Phòng Chiếu:</label>
                    <asp:DropDownList ID="ddlPhong" runat="server" CssClass="form-control" AutoPostBack="false"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvPhong" runat="server" ControlToValidate="ddlPhong"
                        InitialValue="" ErrorMessage="Vui lòng chọn phòng chiếu!" CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Thời Gian Bắt Đầu:</label>
                    <asp:TextBox ID="txtThoiGianBatDau" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvThoiGianBatDau" runat="server" ControlToValidate="txtThoiGianBatDau"
                        ErrorMessage="Vui lòng chọn thời gian bắt đầu!" CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Giá Vé (VNĐ):</label>
                    <asp:TextBox ID="txtGiaVe" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                    <asp:RangeValidator ID="rvGiaVe" runat="server" ControlToValidate="txtGiaVe"
                        MinimumValue="0.01" MaximumValue="99999999.99" Type="Double" ErrorMessage="Giá vé phải từ 0.01 đến 99,999,999 VNĐ!"
                        CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn-save" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn-cancel" OnClick="btnCancel_Click" CausesValidation="false" />
                </div>
            </asp:Panel>

            <!-- Bảng danh sách lịch chiếu -->
            <asp:Panel ID="pnlScheduleList" runat="server" CssClass="schedule-list">
                <asp:Repeater ID="rptSchedules" runat="server">
                    <HeaderTemplate>
                        <table class="schedule-table">
                            <thead>
                                <tr>
                                    <th>Mã Phim</th>
                                    <th>Tên Phim</th>
                                    <th>Mã Phòng</th>
                                    <th>Tên Phòng</th>
                                    <th>Thời Gian Bắt Đầu</th>
                                    <th>Giá Vé (VNĐ)</th>
                                    <th>Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("MaPhim") %></td>
                            <td><%# Server.HtmlEncode(Eval("TenPhim").ToString()) %></td>
                            <td><%# Eval("MaPhong") %></td>
                            <td><%# Server.HtmlEncode(Eval("TenPhong") != null ? Eval("TenPhong").ToString() : "N/A") %></td>
                            <td><%# Eval("ThoiGianBatDau", "{0:dd/MM/yyyy HH:mm}") %></td>
                            <td><%# string.Format("{0:N2}", Eval("GiaVe")) %></td>
                            <td>
                                <asp:Button ID="btnEdit" runat="server" Text="Sửa" CommandArgument='<%# Eval("MaLichChieu") %>' OnClick="btnEdit_Click" CssClass="btn-edit" />
                                <asp:Button ID="btnDelete" runat="server" Text="Xóa" CommandArgument='<%# Eval("MaLichChieu") %>' OnClick="btnDelete_Click" CssClass="btn-delete" OnClientClick="return confirm('Bạn có chắc muốn xóa lịch chiếu này?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblEmptySchedules" runat="server" Text="Không có lịch chiếu nào." Visible="false" CssClass="empty-message"></asp:Label>
            </asp:Panel>
        </div>
    </section>
</asp:Content>