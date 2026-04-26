# 🛒 SMARTS - Thương Mại Điện Tử Chuyên Nghiệp

[![ASP.NET MVC](https://img.shields.io/badge/Framework-ASP.NET%20MVC%205-blue.svg)](https://dotnet.microsoft.com/en-us/apps/aspnet/mvc)
[![Entity Framework](https://img.shields.io/badge/ORM-Entity%20Framework%206-red.svg)](https://learn.microsoft.com/en-us/ef/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-orange.svg)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/UI-Bootstrap%205-purple.svg)](https://getbootstrap.com/)

**SMARTS** là một hệ thống thương mại điện tử hiện đại được xây dựng trên nền tảng ASP.NET MVC, cho phép người dùng mua sắm các thiết bị công nghệ một cách thuận tiện và quản trị viên quản lý cửa hàng một cách hiệu quả.
<hr />
<img width="3682" height="1676" alt="image" src="https://github.com/user-attachments/assets/b12cf46f-fcf8-47d5-9f6f-738d59e9de31" />

---

## ✨ Tính năng nổi bật

### 👨‍💻 Phân hệ Người dùng (Customer)
- **Trang chủ**: Giao diện trực quan, hiển thị sản phẩm nổi bật và danh mục.
- **Tìm kiếm & Lọc**: Tìm kiếm sản phẩm thông minh theo tên, thương hiệu và danh mục.
- **Giỏ hàng**: Quản lý giỏ hàng, cập nhật số lượng và tính toán tổng tiền.
- **Thanh toán**: Tích hợp quy trình đặt hàng và thanh toán trực tuyến.
- **Tài khoản**: Đăng ký, đăng nhập và quản lý thông tin cá nhân.
- **Lịch sử đơn hàng**: Theo dõi trạng thái đơn hàng đã đặt.

### 🛡️ Phân hệ Quản trị (Admin)
- **Dashboard**: Thống kê tổng quan doanh thu, đơn hàng và khách hàng bằng biểu đồ (Chart.js).
- **Quản lý sản phẩm**: CRUD (Thêm, Sửa, Xóa) sản phẩm, quản lý kho và hình ảnh.
- **Quản lý danh mục**: Tổ chức sản phẩm theo các phân loại công nghệ.
- **Quản lý đơn hàng**: Cập nhật trạng thái đơn hàng (Chờ duyệt, Đang giao, Đã giao).
- **Quản lý người dùng**: Kiểm soát danh sách khách hàng và nhân viên.

---

## 🛠️ Công nghệ sử dụng

- **Backend**: C# (ASP.NET MVC 5)
- **Database**: SQL Server với Entity Framework (Database First)
- **Frontend**: HTML5, CSS3, JavaScript, jQuery
- **UI Framework**: Bootstrap 5
- **Charts**: Chart.js cho báo cáo thống kê
- **Khác**: Newtonsoft.Json, WebGrease, Antlr

---

## 📂 Cấu trúc thư mục

```text
Smarts_DoAn/
├── App_Start/          # Cấu hình Route, Bundle, Filter
├── Controllers/        # Xử lý Logic nghiệp vụ (Admin, Home, Payment, User...)
├── Models/             # Entity Framework Models & DTOs
├── Views/              # Giao diện người dùng (.cshtml)
├── Content/            # Tài nguyên CSS, hình ảnh, icon
├── Scripts/            # Thư viện JavaScript & Scripts tùy chỉnh
├── SQL/                # Scripts khởi tạo cơ sở dữ liệu
└── Web.config          # Cấu hình hệ thống & Connection String
```

---

## 🚀 Hướng dẫn cài đặt

### 1. Yêu cầu hệ thống
- Visual Studio 2019 trở lên.
- SQL Server 2012 trở lên.
- .NET Framework 4.8.

### 2. Thiết lập cơ sở dữ liệu
- Vào thư mục `SQL/`.
- Mở file `SMARTS_TEST.sql` trong SQL Server Management Studio (SSMS).
- Chạy (F5) để khởi tạo Database và dữ liệu mẫu.

### 3. Cấu hình ứng dụng
- Mở file `Web.config` trong Visual Studio.
- Cập nhật `connectionString` phù hợp với Server SQL của bạn:
  ```xml
  <connectionStrings>
    <add name="SMARTS_TESTEntities" connectionString="metadata=res://*/Models.Model1.csdl|...data source=YOUR_SERVER_NAME;initial catalog=SMARTS_TEST;integrated security=True;..." />
  </connectionStrings>
  ```

### 4. Chạy dự án
- Mở file `.sln`.
- Nhấn `F5` hoặc `Ctrl + F5` để khởi chạy ứng dụng trên trình duyệt.

---

## 🔐 Tài khoản truy cập

| Vai trò | Email | Mật khẩu |
| :--- | :--- | :--- |
| **Admin** | `admin@gmail.com` | `admin123` |
| **Customer** | (Tự đăng ký trên trang web) | - |

---

## 📸 Demo giao diện
<img width="2782" height="1702" alt="image" src="https://github.com/user-attachments/assets/1a2d25ff-43d0-4066-a1e5-d667ce51e301" />
<hr />
- Trang chủ: Hiện đại, responsive.
<br />
- Quản trị: Trình bày dữ liệu khoa học, dễ sử dụng.

---

## 📝 Giấy phép
Dự án được phát triển cho mục đích học tập và làm đồ án. Vui lòng ghi rõ nguồn khi tham khảo.

---
**SMARTS Team** - Kết nối đam mê công nghệ.
<br />
**Quản lý dự án** - ldqdung@outlook.com (Lê Đỗ Quang Dũng)
