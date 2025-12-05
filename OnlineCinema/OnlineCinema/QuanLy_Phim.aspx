<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLy_Phim.aspx.cs" Inherits="OnlineCinema.QuanLy_Phim" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/ql-phim.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="manage-movie-section">
        <div class="container">
            <h2>Quản Lý Phim</h2>

            <!-- Nút thêm phim -->
            <div class="add-movie-btn">
                <asp:Button ID="btnAddMovie" runat="server" Text="Thêm Phim" CssClass="btn-add" OnClick="btnAddMovie_Click" />
            </div>

            <!-- Form thêm/sửa phim (ẩn mặc định) -->
            <asp:Panel ID="pnlAddEditMovie" runat="server" Visible="false" CssClass="movie-form">
                <h3><asp:Label ID="lblFormTitle" runat="server" Text="Thêm Phim Mới"></asp:Label></h3>
                <div class="form-group">
                    <label>Tên Phim:</label>
                    <asp:TextBox ID="txtTenPhim" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTenPhim" runat="server" ControlToValidate="txtTenPhim"
                        ErrorMessage="Vui lòng nhập tên phim!" CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Thể Loại:</label>
                    <asp:TextBox ID="txtTheLoai" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Thời Lượng (phút):</label>
                    <asp:TextBox ID="txtThoiLuong" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                    <asp:RangeValidator ID="rvThoiLuong" runat="server" ControlToValidate="txtThoiLuong"
                        MinimumValue="1" MaximumValue="300" Type="Integer" ErrorMessage="Thời lượng phải từ 1 đến 300 phút!"
                        CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Ngày Khởi Chiếu:</label>
                    <asp:TextBox ID="txtNgayKhoiChieu" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNgayKhoiChieu" runat="server" ControlToValidate="txtNgayKhoiChieu"
                        ErrorMessage="Vui lòng chọn ngày khởi chiếu!" CssClass="error-message" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Đạo Diễn:</label>
                    <asp:TextBox ID="txtDaoDien" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Mô Tả:</label>
                    <asp:TextBox ID="txtMoTa" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Ảnh Bìa:</label>
                    <asp:FileUpload ID="fuAnhBia" runat="server" CssClass="form-control" />
                    <asp:HiddenField ID="hfAnhBia" runat="server" />
                </div>
                <div class="form-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn-save" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn-cancel" OnClick="btnCancel_Click" CausesValidation="false" />
                </div>
            </asp:Panel>

            <!-- Bảng danh sách phim -->
            <asp:Panel ID="pnlMovieList" runat="server" CssClass="movie-list">
                <asp:Repeater ID="rptMovies" runat="server">
                    <HeaderTemplate>
                        <table class="movie-table">
                            <thead>
                                <tr>
                                    <th>Tên Phim</th>
                                    <th>Thể Loại</th>
                                    <th>Thời Lượng</th>
                                    <th>Ngày Khởi Chiếu</th>
                                    <th>Đạo Diễn</th>
                                    <th>Ảnh Bìa</th>
                                    <th>Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Server.HtmlEncode(Eval("TenPhim").ToString()) %></td>
                            <td><%# Server.HtmlEncode(Eval("TheLoai") != null ? Eval("TheLoai").ToString() : "N/A") %></td>
                            <td><%# Eval("ThoiLuong") != DBNull.Value ? Eval("ThoiLuong").ToString() + " phút" : "N/A" %></td>
                            <td><%# Eval("NgayKhoiChieu", "{0:dd/MM/yyyy}") %></td>
                            <td><%# Server.HtmlEncode(Eval("DaoDien") != null ? Eval("DaoDien").ToString() : "N/A") %></td>
                            <td>
                                <asp:Image ID="imgAnhBia" runat="server" ImageUrl='<%# Eval("AnhBia") %>' CssClass="movie-poster" AlternateText="Ảnh bìa" />
                            </td>
                            <td>
                                <asp:Button ID="btnEdit" runat="server" Text="Sửa" CommandArgument='<%# Eval("MaPhim") %>' OnClick="btnEdit_Click" CssClass="btn-edit" />
                                <asp:Button ID="btnDelete" runat="server" Text="Xóa" CommandArgument='<%# Eval("MaPhim") %>' OnClick="btnDelete_Click" CssClass="btn-delete" OnClientClick="return confirm('Bạn có chắc muốn xóa phim này?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblEmptyMovies" runat="server" Text="Không có phim nào." Visible="false" CssClass="empty-message"></asp:Label>
            </asp:Panel>
        </div>
    </section>
</asp:Content>