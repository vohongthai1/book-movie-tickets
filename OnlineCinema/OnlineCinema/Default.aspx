<%@ Page Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OnlineCinema.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta name="description" content="Đặt vé xem phim nhanh chóng và dễ dàng tại OnlineCinema. Khám phá các bộ phim đang chiếu và ưu đãi hấp dẫn!" />
    <meta name="keywords" content="đặt vé phim, phim đang chiếu, rạp chiếu phim, OnlineCinema" />
    <link href="css/default.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Hero Section -->
    <section class="hero-section">
        <div class="hero-content">
            <h1>Khám Phá Thế Giới Điện Ảnh</h1>
            <p>Đặt vé nhanh chóng, tận hưởng những bộ phim đỉnh cao tại OnlineCinema.</p>
            <a href="#movies" class="btn-hero">Đặt Vé Ngay</a>
        </div>
    </section>

    <!-- Tìm kiếm và Lọc -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <section class="search-filter-section">
                <div class="container">
                    <div class="search-filter">
                        <div class="search-box">
                            <div class="search-wrapper">
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Tìm kiếm phim..."></asp:TextBox>
                                <i class="fas fa-search search-icon"></i>
                            </div>
                            <asp:Button ID="btnSearch" runat="server" Text="Tìm" CssClass="btn-search" OnClick="btnSearch_Click" />
                        </div>
                        <div class="filter-box">
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="filter-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="-- Chọn thể loại --" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="Hành động" Text="Hành động"></asp:ListItem>
                                <asp:ListItem Value="Kinh dị" Text="Kinh dị"></asp:ListItem>
                                <asp:ListItem Value="Hài" Text="Hài"></asp:ListItem>
                                <asp:ListItem Value="Lãng mạn" Text="Lãng mạn"></asp:ListItem>
                                <asp:ListItem Value="Hoạt hình" Text="Hoạt hình"></asp:ListItem>
                                <asp:ListItem Value="Viễn tưởng" Text="Viễn tưởng"></asp:ListItem>
                                <asp:ListItem Value="Tâm lý" Text="Tâm lý"></asp:ListItem>
                                <asp:ListItem Value="Cổ trang" Text="Cổ trang"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:DropDownList ID="ddlSortBy" runat="server" CssClass="filter-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlSortBy_SelectedIndexChanged">
                                <asp:ListItem Value="p.TenPhim" Text="Sắp xếp theo tên"></asp:ListItem>
                                <asp:ListItem Value="MIN(l.GiaVe)" Text="Sắp xếp theo giá vé thấp nhất"></asp:ListItem>
                                <asp:ListItem Value="p.ThoiLuong" Text="Sắp xếp theo thời lượng"></asp:ListItem>
                                <asp:ListItem Value="p.NgayKhoiChieu DESC" Text="Mới nhất"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:Panel ID="pnlSearchResults" runat="server" Visible="false" CssClass="search-results">
                        <h2>Kết quả tìm kiếm: <asp:Literal ID="litSearchQuery" runat="server"></asp:Literal></h2>
                        <asp:Literal ID="litResultCount" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </section>

            <!-- Danh sách phim -->
            <section class="movies-section" id="movies">
                <div class="container">
                    <h2 class="section-title">Kho Phim</h2>
                    <div class="movies-grid">
                        <asp:Repeater ID="rptPhimDangChieu" runat="server">
                            <ItemTemplate>
                                <a href='<%# "ChiTietPhim.aspx?MaPhim=" + Eval("MaPhim") %>' class="movie-card">
                                    <div class="movie-image">
                                        <img src='<%# Eval("AnhBia") %>' alt='<%# Eval("TenPhim") %>' />
                                        <asp:Panel ID="pnlHot" runat="server" Visible='<%# Eval("ThoiLuong").ToString() == "112" %>' CssClass="hot-tag">HOT</asp:Panel>
                                    </div>
                                    <div class="movie-details">
                                        <h3><%# Eval("TenPhim") %></h3>
                                        <p>Thể loại: <%# Eval("TheLoai") %></p>
                                        <div class="movie-info">
                                            <span>Thời lượng: <%# Eval("ThoiLuong") %> phút</span>
                                            <span>Giá vé: <%# string.Format("{0:N0}", Eval("GiaVe")) %> VNĐ</span>
                                        </div>
                                        <!-- Chỉ hiển thị nút "Thêm vào giỏ" nếu không phải quản trị viên -->
                                        <asp:Button ID="btnAddToCart" runat="server" Text="Thêm vào giỏ" 
                                                  CommandArgument='<%# Eval("MaPhim") %>' 
                                                  OnClick="btnAddToCart_Click" 
                                                  CssClass="btn-add-cart"
                                                  Visible='<%# !IsAdmin() %>' />
                                    </div>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
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

    <!-- Giỏ hàng mini - Chỉ hiển thị cho người dùng thường -->
    <div class="cart-icon" runat="server" id="divCartIcon" visible='<%# !IsAdmin() %>'>
        <asp:LinkButton ID="lnkCart" runat="server" OnClick="lnkCart_Click" CssClass="cart-button">
            <i class="fas fa-shopping-cart"></i>
            <span id="cartCount" runat="server" class="cart-badge">0</span>
        </asp:LinkButton>
    </div>
</asp:Content>