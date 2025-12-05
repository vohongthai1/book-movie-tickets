<%@ Page Title="Chi Tiết Phim" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ChiTietPhim.aspx.cs" Inherits="OnlineCinema.ChiTietPhim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta name="description" content="Xem chi tiết phim tại OnlineCinema. Đặt vé nhanh chóng và dễ dàng!" />
    <meta name="keywords" content="chi tiết phim, đặt vé phim, OnlineCinema" />
    <link href="css/chi-tiet-phim.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <section class="movie-detail-section">
                <div class="container">
                    <div class="movie-detail">
                        <div class="movie-detail-image">
                            <asp:Image ID="imgAnhBia" runat="server" CssClass="movie-image" />
                        </div>
                        <div class="movie-detail-info">
                            <h1><asp:Label ID="lblTenPhim" runat="server"></asp:Label></h1>
                            <p class="category"><strong>Thể loại:</strong> <asp:Label ID="lblTheLoai" runat="server"></asp:Label></p>
                            <p class="duration"><strong>Thời lượng:</strong> <asp:Label ID="lblThoiLuong" runat="server"></asp:Label> phút</p>
                            <p class="director"><strong>Đạo diễn:</strong> <asp:Label ID="lblDaoDien" runat="server"></asp:Label></p>
                            <p class="release-date"><strong>Ngày khởi chiếu:</strong> <asp:Label ID="lblNgayKhoiChieu" runat="server"></asp:Label></p>
                            <p class="description"><strong>Mô tả:</strong> <asp:Label ID="lblMoTa" runat="server"></asp:Label></p>
                        </div>
                    </div>
                </div>
            </section>

            <!-- Showtime Section -->
            <section class="showtime-section">
                <div class="container">
                    <h2>Lịch Chiếu</h2>
                    <asp:Panel ID="pnlShowtimes" runat="server" CssClass="showtime-list">

                        <asp:Repeater ID="rptShowtimes" runat="server">
                            <HeaderTemplate>
                                <table class="showtime-table">
                                    <thead>
                                        <tr>
                                            <th>Phòng Chiếu</th>
                                            <th>Thời Gian Bắt Đầu</th>
                                            <th>Giá Vé</th>
                                            <th>Số Vé Còn Lại</th>
                                            <%-- Chỉ hiển thị cột "Chọn" nếu không phải quản trị viên --%>
                                            <th runat="server" visible='<%# !IsAdmin() %>'>Chọn</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("TenPhong") %></td>
                                    <td><%# Eval("ThoiGianBatDau", "{0:dd/MM/yyyy HH:mm}") %></td>
                                    <td><%# Eval("GiaVe", "{0:N0}") %> VNĐ</td>
                                    <td><%# Eval("SoVeConLai") %></td>
                                    <%-- Chỉ hiển thị nút "Chọn ghế" nếu không phải quản trị viên --%>
                                    <td runat="server" visible='<%# !IsAdmin() %>'>
                                        <asp:Button ID="btnAddToCart" runat="server" Text="Chọn ghế" 
                                                    CommandArgument='<%# Eval("MaLichChieu") %>' 
                                                    OnClick="btnAddToCart_Click" 
                                                    CssClass="btn-add-cart" 
                                                    Enabled='<%# Convert.ToInt32(Eval("SoVeConLai")) > 0 %>'
                                                    Visible='<%# !IsAdmin() %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblNoShowtimes" runat="server" Text="Không có lịch chiếu nào hiện tại." Visible="false" CssClass="no-showtimes"></asp:Label>
                    </asp:Panel>
                </div>
            </section>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="loading-overlay">
                <div class="loading-spinner"></div>
                <span class="loading-text">Đang xử lý...</span>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>