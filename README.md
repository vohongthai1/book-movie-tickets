# OnlineCinema - Hệ Thống Đặt Vé Xem Phim Online

## Giới thiệu

OnlineCinema là một ứng dụng web ASP.NET Web Forms cho phép người dùng đặt vé xem phim trực tuyến. Hệ thống cung cấp đầy đủ tính năng từ việc xem phim, chọn ghế, thanh toán đến quản lý cho admin.

## Tính năng chính

### Cho người dùng
- **Xem danh sách phim**: Duyệt qua các phim đang chiếu
- **Chi tiết phim**: Xem thông tin chi tiết về phim
- **Chọn ghế**: Lựa chọn ghế ngồi trực quan
- **Giỏ hàng**: Quản lý các vé đã chọn
- **Thanh toán**: Thực hiện thanh toán vé
- **Lịch sử vé**: Xem các vé đã đặt
- **Đăng ký/Đăng nhập**: Quản lý tài khoản người dùng

###  Cho quản trị viên
- **Quản lý phim**: Thêm, sửa, xóa phim
- **Quản lý lịch chiếu**: Tạo và quản lý lịch trình chiếu phim
- **Quản lý phòng chiếu**: Quản lý thông tin các phòng chiếu
- **Quản lý người dùng**: Quản lý tài khoản người dùng
- **Quản lý vé**: Xem và quản lý các vé đã đặt
- **Dashboard**: Xem thống kê và báo cáo

## Công nghệ sử dụng

- **Frontend**: ASP.NET Web Forms, HTML, CSS, JavaScript
- **Backend**: C# (.NET Framework)
- **Database**: (Xem trong Web.config để biết chi tiết)
- **IDE**: Visual Studio

## Cấu trúc dự án

```
OnlineCinema/
├── Admin/                    # Trang quản trị
├── Customers/               # Trang khách hàng
├── App_Data/                # Database và dữ liệu
├── css/                     # File CSS
├── images/                  # Hình ảnh
├── bin/                     # Binary files
├── obj/                     # Object files
├── packages/                # NuGet packages
├── Default.aspx             # Trang chủ
├── ChiTietPhim.aspx         # Chi tiết phim
├── ChonGhe.aspx             # Chọn ghế
├── GioHang.aspx             # Giỏ hàng
├── thanhtoan.aspx           # Thanh toán
├── DangKy_DangNhap.aspx     # Đăng ký/Đăng nhập
├── VeDaDat.aspx             # Vé đã đặt
├── QuanLy_*.aspx            # Các trang quản lý
├── Main.Master              # Master page cho khách hàng
├── Admin.Master             # Master page cho admin
└── Web.config               # Cấu hình ứng dụng
```

## Các trang chính

### Trang người dùng
- `Default.aspx` - Trang chủ hiển thị danh sách phim
- `ChiTietPhim.aspx` - Hiển thị thông tin chi tiết phim
- `ChonGhe.aspx` - Giao diện chọn ghế ngồi
- `GioHang.aspx` - Giỏ hàng của người dùng
- `thanhtoan.aspx` - Trang thanh toán
- `VeDaDat.aspx` - Lịch sử vé đã đặt

### Trang quản trị
- `Dashboard.aspx` - Bảng điều khiển chính
- `QuanLy_Phim.aspx` - Quản lý phim
- `QuanLy_LichChieu.aspx` - Quản lý lịch chiếu
- `QuanLy_PhongChieu.aspx` - Quản lý phòng chiếu
- `QuanLy_NguoiDung.aspx` - Quản lý người dùng
- `QuanLy_Ve.aspx` - Quản lý vé

## Cài đặt và chạy

### Yêu cầu
- Visual Studio 2017 trở lên
- .NET Framework 4.7.2 trở lên
- SQL Server (nếu sử dụng database)

### Hướng dẫn cài đặt
1. Clone hoặc tải dự án về máy
2. Mở file `OnlineCinema.sln` bằng Visual Studio
3. Restore NuGet packages (nếu cần)
4. Cấu hình chuỗi kết nối database trong `Web.config`
5. Build và chạy dự án (F5)

## Cấu hình

### Web.config
- Chuỗi kết nối database
- Cấu hình authentication
- Các thiết lập hệ thống khác

## Tác giả

Dự án được phát triển cho môn học **Kiểm Thử & Đảm Bảo Chất Lượng Phần Mềm**

## License

Dự án này được sử dụng cho mục đích học tập.
