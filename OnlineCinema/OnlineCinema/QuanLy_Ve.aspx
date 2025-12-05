<%@ Page Title="Quản Lý Vé" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLy_Ve.aspx.cs" Inherits="OnlineCinema.QuanLy_Ve" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/ql-ve.css" rel="stylesheet" />
    <style>
        /* --- Tổng quan --- */
        .manage-ticket-section .container h2 { margin-bottom: 20px; border-bottom: 2px solid #f0f0f0; padding-bottom: 10px; }

        /* --- 1. Bộ lọc --- */
        .filter-section { background-color: #f9f9f9; padding: 20px; border-radius: 8px; margin-bottom: 25px; display: flex; flex-wrap: wrap; gap: 15px 20px; align-items: flex-end; border: 1px solid #e9e9e9; }
        .filter-group { display: flex; flex-direction: column; min-width: 180px; flex-grow: 1; }
        .filter-group label { margin-bottom: 5px; font-weight: bold; font-size: 14px; color: #333; }
        .filter-group .form-control { padding: 9px 12px; border: 1px solid #ddd; border-radius: 4px; font-size: 14px; transition: border-color 0.2s, box-shadow 0.2s; }
        .filter-group .form-control:focus { outline: none; border-color: #007bff; box-shadow: 0 0 0 3px rgba(0,123,255,0.15); }
        .btn-filter { padding: 10px 20px; background-color: #007bff; color: white; border: none; border-radius: 4px; cursor: pointer; font-size: 14px; height: 38px; transition: background-color 0.2s; }
        .btn-filter:hover { background-color: #0056b3; }

        /* --- 2. Bảng (Table) --- */
        .ticket-table { width: 100%; border-collapse: collapse; margin-top: 20px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); border-radius: 8px; overflow: hidden; }
        .ticket-table th, .ticket-table td { border-bottom: 1px solid #eee; padding: 14px 16px; text-align: left; vertical-align: middle; font-size: 14px; }
        .ticket-table thead th { background-color: #f8f9fa; font-weight: 600; color: #333; border-top: 1px solid #eee; border-bottom-width: 2px; }
        .ticket-table tbody tr:nth-child(even) { background-color: #fdfdfd; }
        .ticket-table tbody tr:hover { background-color: #f1f8ff; }

        /* --- 3. Trạng thái (Pills) --- */
        .status-pill { display: inline-block; padding: 4px 12px; font-size: 12px; font-weight: 600; border-radius: 20px; text-transform: uppercase; letter-spacing: 0.5px; }
        .status-cho-thanh-toan { color: #a66300; background-color: #fff4e0; border: 1px solid #ffe8c2; }
        .status-sap-chieu { color: #1a6b32; background-color: #e3f9e8; border: 1px solid #c7f2d0; }
        .status-da-huy { color: #a82a2a; background-color: #fde8e8; border: 1px solid #fbcaca; }
        .status-da-chieu { color: #495057; background-color: #f1f3f5; border: 1px solid #e0e0e0; }

        /* --- 4. Nút thao tác (Actions) --- */
        .btn-approve, .btn-reject, .btn-delete { border: none; padding: 6px 12px; cursor: pointer; border-radius: 4px; font-size: 13px; font-weight: 600; margin: 2px; transition: all 0.2s ease-in-out; }
        .btn-approve { background-color: #e3f9e8; color: #1a6b32; }
        .btn-approve:hover { background-color: #c7f2d0; color: #1a6b32; transform: translateY(-1px); }
        .btn-reject { background-color: #fde8e8; color: #a82a2a; }
        .btn-reject:hover { background-color: #fbcaca; color: #a82a2a; transform: translateY(-1px); }
        .btn-delete { background-color: #fff4e0; color: #a66300; }
        .btn-delete:hover { background-color: #ffe8c2; color: #a66300; transform: translateY(-1px); }
        
        /* --- Thông báo rỗng --- */
        .empty-message { text-align: center; padding: 40px; font-size: 1.1em; color: #777; background: #f9f9f9; border-radius: 8px; margin-top: 20px; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="manage-ticket-section">
        <div class="container">
            <h2>Quản Lý Vé</h2>

            <asp:Panel ID="pnlFilters" runat="server" CssClass="filter-section">
                <div class="filter-group">
                    <label>Tìm kiếm (SĐT, Email, Mã vé, Mã ĐH)</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Nhập từ khóa..."></asp:TextBox>
                </div>
                
                <div class="filter-group">
                    <label for="ddlTrangThai">Lọc theo trạng thái</label>
                    <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="btnFilter_Click">
                        <asp:ListItem Value="-1">-- Tất cả trạng thái --</asp:ListItem>
                        <asp:ListItem Value="2" Selected="True">Chờ thanh toán</asp:ListItem>
                        <asp:ListItem Value="1">Đã xác nhận</asp:ListItem>
                        <asp:ListItem Value="0">Đã hủy / Từ chối</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="filter-group">
                    <label>Lọc theo thể loại</label>
                    <asp:DropDownList ID="ddlTheLoai" runat="server" CssClass="form-control" 
                        DataTextField="TheLoai" DataValueField="TheLoai" AppendDataBoundItems="true">
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label>Từ ngày (suất chiếu)</label>
                    <asp:TextBox ID="txtTuNgay" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
                <div class="filter-group">
                    <label>Đến ngày (suất chiếu)</label>
                    <asp:TextBox ID="txtDenNgay" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
                    <asp:Button ID="btnFilter" runat="server" Text="Lọc" CssClass="btn-filter" OnClick="btnFilter_Click" />
            </asp:Panel>

            <asp:Panel ID="pnlTicketList" runat="server" CssClass="ticket-list">
                <asp:Repeater ID="rptTickets" runat="server">
                    <HeaderTemplate>
                        <table class="ticket-table">
                            <thead>
                                <tr>
                                    <th>Mã Vé</th>
                                    <th>Mã Đơn Hàng</th>
                                    <th>Khách Hàng</th>
                                    <th>Phim</th>
                                    <th>Thời Gian</th>
                                    <th>Ghế Ngồi</th>
                                    <th>Ghi Chú</th>
                                    <th>Tổng Tiền</th>
                                    <th>Trạng Thái</th>
                                    <th>Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("MaVe") %></td>
                            <td><%# Eval("MaDonHang") %></td>
                            <td><%# Server.HtmlEncode(Eval("HoTen") != null ? Eval("HoTen").ToString() : "N/A") %></td>
                            <td><%# Server.HtmlEncode(Eval("TenPhim") != null ? Eval("TenPhim").ToString() : "N/A") %></td>
                            <td><%# Eval("ThoiGianBatDau", "{0:dd/MM/yyyy HH:mm}") %></td>
                            <td><%# Server.HtmlEncode(Eval("SoGhe") != null ? Eval("SoGhe").ToString() : "N/A") %></td>
                            <td><%# Eval("GhiChu") %></td>
                            <td><%# Eval("TongTien", "{0:N0}") %> VNĐ</td>
                            <td>
                                <%# GetTrangThaiVe(Convert.ToInt32(Eval("TrangThaiVe")), Convert.ToDateTime(Eval("ThoiGianBatDau"))) %>
                            </td>
                            <td>
                                <%-- ĐÃ SỬA LỖI CANH HÀNG VÀ VALIDATION --%>
                                <asp:Button ID="btnApprove" runat="server" Text="Duyệt" 
                                    CommandArgument='<%# Eval("MaDonHang") %>' 
                                    OnClick="btnApprove_Click" CssClass="btn-approve" 
                                    Visible='<%# Convert.ToInt32(Eval("TrangThaiVe")) == 2 %>'
                                    OnClientClick="return confirm('Bạn có chắc muốn DUYỆT đơn hàng này?');" 
                                    CausesValidation="false" /><asp:Button ID="btnReject" runat="server" Text="Từ chối" 
                                    CommandArgument='<%# Eval("MaDonHang") %>' 
                                    OnClick="btnReject_Click" CssClass="btn-reject" 
                                    Visible='<%# Convert.ToInt32(Eval("TrangThaiVe")) == 2 %>'
                                    OnClientClick="return confirm('Bạn có chắc muốn TỪ CHỐI đơn hàng này?');" 
                                    CausesValidation="false" /><asp:Button ID="btnCancel" runat="server" Text="Hủy Vé" 
                                    CommandArgument='<%# Eval("MaVe") %>' 
                                    OnClick="btnCancel_Click" CssClass="btn-delete" 
                                    Visible='<%# Convert.ToInt32(Eval("TrangThaiVe")) == 1 && Convert.ToDateTime(Eval("ThoiGianBatDau")) > DateTime.Now %>'
                                    OnClientClick="return confirm('Bạn có chắc muốn HỦY vé đã xác nhận này?');" 
                                    CausesValidation="false" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblEmptyTickets" runat="server" Text="Không có vé nào." Visible="false" CssClass="empty-message"></asp:Label>
            </asp:Panel>
        </div>
    </section>
</asp:Content>