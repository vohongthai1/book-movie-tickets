<%@ Page Title="Đăng nhập & Đăng ký" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="DangKy_DangNhap.aspx.cs" Inherits="OnlineCinema.DangKy_DangNhap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/login.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="auth-container">
        <div class="auth-sidebar">
            <h1>Chào mừng đến với OnlineCinema</h1>
            <p>Đăng nhập hoặc đăng ký để có thể trải nghiệm đầy đủ các dịch vụ của chúng tôi.</p>
            
            <div class="auth-features">
                <div class="feature-item">
                    <div class="feature-icon">
                        <i class="fas fa-ticket-alt"></i>
                    </div>
                    <div class="feature-text">Đặt vé nhanh chóng, tiện lợi</div>
                </div>
                
                <div class="feature-item">
                    <div class="feature-icon">
                        <i class="fas fa-percent"></i>  
                    </div>
                    <div class="feature-text">Nhận nhiều ưu đãi hấp dẫn</div>
                </div>
                
                <div class="feature-item">
                    <div class="feature-icon">
                        <i class="fas fa-history"></i>
                    </div>
                    <div class="feature-text">Quản lý lịch sử giao dịch</div>
                </div>
            </div>
        </div>
        
        <div class="auth-forms">
            <asp:Panel ID="pnlNotification" runat="server" Visible="false" CssClass="notification-banner">
                <asp:Literal ID="litNotification" runat="server"></asp:Literal>
            </asp:Panel>
            
            <div class="tab-buttons">
                <button type="button" class="tab-btn active" id="btnLoginTab">Đăng nhập</button>
                <button type="button" class="tab-btn" id="btnRegisterTab">Đăng ký</button>
            </div>
            
            <div class="form-content active" id="loginForm">
                <div class="form-group">
                    <label for="<%= txtLoginEmail.ClientID %>">Email</label>
                    <asp:TextBox ID="txtLoginEmail" runat="server" CssClass="form-control" placeholder="Nhập email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLoginEmail" runat="server" 
                        ControlToValidate="txtLoginEmail" 
                        ErrorMessage="Vui lòng nhập email" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationGroup="LoginGroup">
                    </asp:RequiredFieldValidator>
                </div>
                
                <div class="form-group">
                    <label for="<%= txtLoginPass.ClientID %>">Mật khẩu</label>
                    <asp:TextBox ID="txtLoginPass" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập mật khẩu"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLoginPass" runat="server" 
                        ControlToValidate="txtLoginPass" 
                        ErrorMessage="Vui lòng nhập mật khẩu" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationGroup="LoginGroup">
                    </asp:RequiredFieldValidator>
                </div>
                
                <asp:Button ID="btnLogin" runat="server" Text="Đăng nhập" CssClass="btn-auth" OnClick="btnLogin_Click" ValidationGroup="LoginGroup" />
                
                <div class="auth-footer">
                    <p>Quên mật khẩu? Khôi phục mật khẩu</p>
                </div>
            </div>
            
            <div class="form-content" id="registerForm">
                <div class="form-group">
                    <label for="<%= txtRegEmail.ClientID %>">Email</label>
                    <asp:TextBox ID="txtRegEmail" runat="server" CssClass="form-control" placeholder="Nhập email của bạn"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvRegEmail" runat="server" 
                        ControlToValidate="txtRegEmail" 
                        ErrorMessage="Vui lòng nhập email" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationGroup="RegisterGroup">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revRegEmail" runat="server" 
                        ControlToValidate="txtRegEmail" 
                        ErrorMessage="Email không hợp lệ" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ValidationGroup="RegisterGroup">
                    </asp:RegularExpressionValidator>
                </div>
                
                <div class="form-group">
                    <label for="<%= txtRegPass.ClientID %>">Mật khẩu</label>
                    <asp:TextBox ID="txtRegPass" runat="server" TextMode="Password" CssClass="form-control" placeholder="Tạo mật khẩu mới"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvRegPass" runat="server" 
                        ControlToValidate="txtRegPass" 
                        ErrorMessage="Vui lòng nhập mật khẩu" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationGroup="RegisterGroup">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revRegPass" runat="server" 
                        ControlToValidate="txtRegPass" 
                        ErrorMessage="Mật khẩu phải có ít nhất 6 ký tự" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationExpression=".{6,}"
                        ValidationGroup="RegisterGroup">
                    </asp:RegularExpressionValidator>
                </div>
                
                <div class="form-group">
                    <label for="<%= txtRegConfirmPass.ClientID %>">Xác nhận mật khẩu</label>
                    <asp:TextBox ID="txtRegConfirmPass" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập lại mật khẩu"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvRegConfirmPass" runat="server" 
                        ControlToValidate="txtRegConfirmPass" 
                        ErrorMessage="Vui lòng xác nhận mật khẩu" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationGroup="RegisterGroup">
                    </asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvRegConfirmPass" runat="server" 
                        ControlToValidate="txtRegConfirmPass" 
                        ControlToCompare="txtRegPass"
                        ErrorMessage="Mật khẩu không khớp" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationGroup="RegisterGroup">
                    </asp:CompareValidator>
                </div>
                
                <div class="form-group">
                    <label for="<%= txtRegHoTen.ClientID %>">Họ và tên</label>
                    <asp:TextBox ID="txtRegHoTen" runat="server" CssClass="form-control" placeholder="Nhập họ và tên của bạn"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvRegHoTen" runat="server" 
                        ControlToValidate="txtRegHoTen" 
                        ErrorMessage="Vui lòng nhập họ và tên" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationGroup="RegisterGroup">
                    </asp:RequiredFieldValidator>
                </div>
                
                <div class="form-group">
                    <label for="<%= txtRegPhone.ClientID %>">Số điện thoại</label>
                    <asp:TextBox ID="txtRegPhone" runat="server" CssClass="form-control" placeholder="Nhập số điện thoại"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvRegPhone" runat="server" 
                        ControlToValidate="txtRegPhone" 
                        ErrorMessage="Vui lòng nhập số điện thoại" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationGroup="RegisterGroup">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revRegPhone" runat="server" 
                        ControlToValidate="txtRegPhone" 
                        ErrorMessage="Số điện thoại không hợp lệ" 
                        CssClass="validation-message" 
                        Display="Dynamic"
                        ValidationExpression="^(0|\+84)[3|5|7|8|9]\d{8}$"
                        ValidationGroup="RegisterGroup">
                    </asp:RegularExpressionValidator>
                </div>
                
                <asp:Button ID="btnRegister" runat="server" Text="Đăng ký" CssClass="btn-auth" OnClick="btnRegister_Click" ValidationGroup="RegisterGroup" />
                
                <div class="auth-footer">
                    <p>Khi đăng ký, bạn đã đồng ý với <a href="Terms.aspx">Điều khoản sử dụng</a> của chúng tôi</p>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function() {
            const loginTab = document.getElementById('btnLoginTab');
            const registerTab = document.getElementById('btnRegisterTab');
            const loginForm = document.getElementById('loginForm');
            const registerForm = document.getElementById('registerForm');
            
            loginTab.addEventListener('click', function() {
                loginTab.classList.add('active');
                registerTab.classList.remove('active');
                loginForm.classList.add('active');
                registerForm.classList.remove('active');
            });
            
            registerTab.addEventListener('click', function() {
                registerTab.classList.add('active');
                loginTab.classList.remove('active');
                registerForm.classList.add('active');
                loginForm.classList.remove('active');
            });
            const urlParams = new URLSearchParams(window.location.search);
            const tab = urlParams.get('tab');

            if (tab === 'register') {
                registerTab.click(); // Hiển thị tab đăng ký
            } else {
                loginTab.click(); // Mặc định là tab đăng nhập
            }
        });
    </script>
</asp:Content>