<%@ Page Title="Chọn Ghế" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ChonGhe.aspx.cs" Inherits="OnlineCinema.ChonGhe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/chon_ghe.css" rel="stylesheet" />
    <style>
        .seat-container {
            text-align: center;
            margin: 20px 0;
        }
        .seat-grid {
            display: inline-block;
            text-align: left;
        }
        .seat {
            width: 40px;
            height: 40px;
            margin: 5px;
            display: inline-block;
            text-align: center;
            line-height: 40px;
            border: 1px solid #ccc;
            cursor: pointer;
        }
        .seat-available {
            background-color: #28a745;
            color: white;
        }
        .seat-booked {
            background-color: #dc3545;
            color: white;
            cursor: not-allowed;
        }
        .seat-selected {
            background-color: #007bff;
            color: white;
        }
        .screen {
            width: 100%;
            height: 30px;
            background-color: #333;
            color: white;
            text-align: center;
            line-height: 30px;
            margin-bottom: 20px;
        }
        .showtime-info {
            margin-bottom: 20px;
        }
        .btn-confirm {
            padding: 10px 20px;
            background-color: #28a745;
            color: white;
            border: none;
            cursor: pointer;
        }
        .btn-confirm:disabled {
            background-color: #ccc;
            cursor: not-allowed;
        }
        .no-seats {
            color: red;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <section class="seat-selection-section">
                <div class="container">
                    <h2>Chọn Ghế</h2>
                    <div class="showtime-info">
                        <p><strong>Phim:</strong> <asp:Label ID="lblTenPhim" runat="server"></asp:Label></p>
                        <p><strong>Phòng chiếu:</strong> <asp:Label ID="lblTenPhong" runat="server"></asp:Label></p>
                        <p><strong>Thời gian:</strong> <asp:Label ID="lblThoiGian" runat="server"></asp:Label></p>
                        <p><strong>Giá vé:</strong> <asp:Label ID="lblGiaVe" runat="server"></asp:Label> VNĐ</p>
                        <p><strong>Số vé còn lại:</strong> <asp:Label ID="lblSoVeConLai" runat="server"></asp:Label></p>
                    </div>
                    <asp:Panel ID="pnlSeats" runat="server" CssClass="seat-container">
                        <div class="screen">Màn Hình</div>
                        <div class="seat-grid">
                            <asp:Repeater ID="rptSeats" runat="server" OnItemDataBound="rptSeats_ItemDataBound">
                                <ItemTemplate>
                                    <asp:Button ID="btnSeat" runat="server" 
                                                CssClass='<%# Eval("TrangThai").ToString() == "1" ? "seat seat-booked" : "seat seat-available" %>'
                                                Text='<%# Eval("SoGhe") %>'
                                                CommandArgument='<%# Eval("MaGhe") %>'
                                                OnClick="btnSeat_Click"
                                                Enabled='<%# Eval("TrangThai").ToString() == "1" ? false : true %>' />
                                    <%# (Container.ItemIndex + 1) % 10 == 0 ? "<br />" : "" %>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:Panel>
                    <asp:Label ID="lblNoSeats" runat="server" Text="Không có ghế nào khả dụng." Visible="false" CssClass="no-seats"></asp:Label>
                    <div>
                        <p><strong>Ghế đã chọn:</strong> <asp:Label ID="lblSelectedSeats" runat="server" Text="Chưa chọn ghế"></asp:Label></p>
                        <asp:Button ID="btnAddToCart" runat="server" Text="Thêm vào giỏ hàng" CssClass="btn-confirm" OnClick="btnAddToCart_Click" Enabled="false" />
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
</asp:Content>