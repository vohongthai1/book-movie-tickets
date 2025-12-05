<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="OnlineCinema.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="dashboard-header">
        <h1>Dashboard</h1>
        <p>Chào mừng bạn trở lại! Đây là tổng quan về hệ thống.</p>
    </div>
    
    <!-- Statistics Cards -->
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-header">
                <div class="stat-icon blue">
                    <i class="fas fa-film"></i>
                </div>
            </div>
            <div class="stat-value">
                <asp:Label ID="lblTongPhim" runat="server" Text="0"></asp:Label>
            </div>
            <div class="stat-label">Tổng số phim</div>
            <div class="stat-change positive">
                <i class="fas fa-arrow-up"></i> Đang chiếu
            </div>
        </div>
        
        <div class="stat-card">
            <div class="stat-header">
                <div class="stat-icon green">
                    <i class="fas fa-calendar-check"></i>
                </div>
            </div>
            <div class="stat-value">
                <asp:Label ID="lblTongLichChieu" runat="server" Text="0"></asp:Label>
            </div>
            <div class="stat-label">Lịch chiếu hôm nay</div>
            <div class="stat-change positive">
                <i class="fas fa-arrow-up"></i> Đã sắp xếp
            </div>
        </div>
        
        <div class="stat-card">
            <div class="stat-header">
                <div class="stat-icon orange">
                    <i class="fas fa-ticket-alt"></i>
                </div>
            </div>
            <div class="stat-value">
                <asp:Label ID="lblTongVe" runat="server" Text="0"></asp:Label>
            </div>
            <div class="stat-label">Vé đã đặt hôm nay</div>
            <div class="stat-change positive">
                <i class="fas fa-arrow-up"></i> +12% so với hôm qua
            </div>
        </div>
        
        <div class="stat-card">
            <div class="stat-header">
                <div class="stat-icon red">
                    <i class="fas fa-money-bill-wave"></i>
                </div>
            </div>
            <div class="stat-value">
                <asp:Label ID="lblDoanhThu" runat="server" Text="0"></asp:Label> VNĐ
            </div>
            <div class="stat-label">Doanh thu hôm nay</div>
            <div class="stat-change positive">
                <i class="fas fa-arrow-up"></i> +8% so với hôm qua
            </div>
        </div>
    </div>
    
    <!-- Recent Activities -->
    <div class="content-card">
        <div class="card-header">
            <h2 class="card-title">Hoạt động gần đây</h2>
            <a href="QuanLy_Ve.aspx" class="btn btn-secondary">Xem tất cả</a>
        </div>
        
        <asp:GridView ID="gvRecentBookings" runat="server" CssClass="modern-table" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="MaVe" HeaderText="Mã vé" />
                <asp:BoundField DataField="TenPhim" HeaderText="Tên phim" />
                <asp:BoundField DataField="HoTen" HeaderText="Khách hàng" />
                <asp:BoundField DataField="SoLuongVe" HeaderText="Số lượng" />
                <asp:BoundField DataField="TongTien" HeaderText="Tổng tiền" DataFormatString="{0:N0} VNĐ" />
                <asp:BoundField DataField="NgayDat" HeaderText="Ngày đặt" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <span class='<%# GetStatusClass(Eval("TrangThai")) %>'>
                            <%# GetStatusText(Eval("TrangThai")) %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div class="empty-state">
                    <i class="fas fa-inbox fa-3x"></i>
                    <p>Chưa có hoạt động nào gần đây</p>
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
    
    <style>
        .modern-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 1rem;
        }
        
        .modern-table th {
            background-color: #f7fafc;
            padding: 1rem;
            text-align: left;
            font-weight: 600;
            color: #1a2433;
            border-bottom: 2px solid #e2e8f0;
        }
        
        .modern-table td {
            padding: 1rem;
            border-bottom: 1px solid #e2e8f0;
            color: #4a5568;
        }
        
        .modern-table tr:hover {
            background-color: #f7fafc;
        }
        
        .status-badge {
            padding: 0.4rem 0.8rem;
            border-radius: 20px;
            font-size: 0.85rem;
            font-weight: 500;
        }
        
        .status-success {
            background-color: #c6f6d5;
            color: #22543d;
        }
        
        .status-pending {
            background-color: #fef5e7;
            color: #744210;
        }
        
        .status-cancelled {
            background-color: #fed7d7;
            color: #742a2a;
        }
        
        .empty-state {
            text-align: center;
            padding: 3rem;
            color: #a0aec0;
        }
        
        .empty-state i {
            margin-bottom: 1rem;
        }
        
        .empty-state p {
            font-size: 1.1rem;
        }
    </style>
</asp:Content>