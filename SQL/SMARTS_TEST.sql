USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SMARTS_TEST')
BEGIN
    CREATE DATABASE [SMARTS_TEST]
END
GO

USE [SMARTS_TEST]
GO

-- 1. Create DANHMUC table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHMUC]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DANHMUC](
	[MADM] [char](5) NOT NULL,
	[TENDM] [nvarchar](30) NULL,
	[SOLUONG_SP] [int] NULL,
PRIMARY KEY CLUSTERED ([MADM] ASC)
)
END
GO

-- 2. Create SANPHAM table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SANPHAM]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SANPHAM](
	[MASP] [char](5) NOT NULL,
	[MADM] [char](5) NULL,
	[TENSP] [nvarchar](120) NULL,
	[GIA] [decimal](18, 0) NULL,
	[THUONGHIEU] [varchar](30) NULL,
	[MOTA] [nvarchar](1000) NULL,
	[THONGSO] [nvarchar](1000) NULL,
	[SOLUONG] [int] NULL,
	[ANHSP] [varchar](500) NULL,
PRIMARY KEY CLUSTERED ([MASP] ASC)
)
END
GO

-- 3. Create NGUOIDUNG table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NGUOIDUNG]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NGUOIDUNG](
	[MAND] [char](5) NOT NULL,
	[EMAIL] [varchar](30) NULL,
	[VAITRO] [varchar](30) NULL,
	[MATKHAU] [char](15) NULL,
	[HOTEN] [nvarchar](60) NULL,
	[SODIENTHOAI] [char](11) NULL,
	[ANHDAIDIEN] [varchar](300) NULL,
	[DIACHI] [varchar](1000) NULL,
PRIMARY KEY CLUSTERED ([MAND] ASC)
)
END
GO

-- 4. Create GIOHANG table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GIOHANG]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[GIOHANG](
	[MASP] [char](5) NOT NULL,
	[MAND] [char](5) NOT NULL,
	[ANHSP] [varchar](500) NULL,
	[TENSP] [nvarchar](120) NULL,
	[GIA] [decimal](18, 0) NULL,
	[SOLUONG] [int] NULL,
	[TAMTINH] [decimal](18, 0) NULL,
PRIMARY KEY CLUSTERED ([MASP] ASC, [MAND] ASC)
)
END
GO

-- 5. Create DATHANG table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DATHANG]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DATHANG](
	[MADH] [char](5) NOT NULL,
	[HO] [nvarchar](30) NULL,
	[TEN] [nvarchar](30) NULL,
	[EMAIL] [nvarchar](30) NULL,
	[SODIENTHOAI] [char](11) NULL,
	[DIACHI] [nvarchar](500) NULL,
	[PHUONGTHUCTHANHTOAN] [nvarchar](300) NULL,
	[MAND] [char](5) NULL,
PRIMARY KEY CLUSTERED ([MADH] ASC)
)
END
GO

-- 6. Create HOADON table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HOADON]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HOADON](
	[MAHD] [char](5) NOT NULL,
	[MADH] [char](5) NULL,
	[TINHTRANG] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED ([MAHD] ASC)
)
END
GO

-- 7. Create CHITIETHOADON table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CHITIETHOADON]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CHITIETHOADON](
	[MAHD] [char](5) NOT NULL,
	[MASP] [char](5) NOT NULL,
	[TENSP] [nvarchar](120) NULL,
	[SOLUONG] [int] NULL,
	[GIA] [decimal](18, 0) NULL,
	[TAMTINH] [decimal](18, 0) NULL,
	[THANHTIEN] [decimal](18, 0) NULL,
PRIMARY KEY CLUSTERED ([MAHD] ASC, [MASP] ASC)
)
END
GO

-- 8. Create LIENHE table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LIENHE]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LIENHE](
	[MALH] [char](5) NOT NULL,
	[EMAIL] [varchar](30) NULL,
	[HOTEN] [nvarchar](60) NULL,
	[SODIENTHOAI] [char](11) NULL,
	[DIACHI] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED ([MALH] ASC)
)
END
GO

-- Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__SANPHAM__MADM__3B75D760')
ALTER TABLE [dbo].[SANPHAM]  WITH CHECK ADD FOREIGN KEY([MADM])
REFERENCES [dbo].[DANHMUC] ([MADM])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__GIOHANG__MASP__3E52440B')
ALTER TABLE [dbo].[GIOHANG]  WITH CHECK ADD FOREIGN KEY([MASP])
REFERENCES [dbo].[SANPHAM] ([MASP])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__GIOHANG__MAND__3F466844')
ALTER TABLE [dbo].[GIOHANG]  WITH CHECK ADD FOREIGN KEY([MAND])
REFERENCES [dbo].[NGUOIDUNG] ([MAND])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__DATHANG__MAND__4222D4EF')
ALTER TABLE [dbo].[DATHANG]  WITH CHECK ADD FOREIGN KEY([MAND])
REFERENCES [dbo].[NGUOIDUNG] ([MAND])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__HOADON__MADH__44FF419A')
ALTER TABLE [dbo].[HOADON]  WITH CHECK ADD FOREIGN KEY([MADH])
REFERENCES [dbo].[DATHANG] ([MADH])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__CHITIETHOA__MAHD__47DBAE45')
ALTER TABLE [dbo].[CHITIETHOADON]  WITH CHECK ADD FOREIGN KEY([MAHD])
REFERENCES [dbo].[HOADON] ([MAHD])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__CHITIETHOA__MASP__48CFD27E')
ALTER TABLE [dbo].[CHITIETHOADON]  WITH CHECK ADD FOREIGN KEY([MASP])
REFERENCES [dbo].[SANPHAM] ([MASP])
GO

-- Stored Procedures
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DM_THEM_DM]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DM_THEM_DM]
GO
CREATE PROCEDURE [dbo].[DM_THEM_DM]
	@madm char(5),
	@tendm nvarchar(30),
	@soluong_sp int
AS
BEGIN
	INSERT INTO DANHMUC(MADM, TENDM, SOLUONG_SP) VALUES (@madm, @tendm, @soluong_sp)
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ND_THEM_ND]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ND_THEM_ND]
GO
CREATE PROCEDURE [dbo].[ND_THEM_ND]
	@mand char(5),
	@email varchar(30),
	@vaitro varchar(30),
	@matkhau char(15),
	@hoten nvarchar(60),
	@sodienthoai char(11)
AS
BEGIN
	INSERT INTO NGUOIDUNG(MAND, EMAIL, VAITRO, MATKHAU, HOTEN, SODIENTHOAI) 
	VALUES (@mand, @email, @vaitro, @matkhau, @hoten, @sodienthoai)
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_THEM_SP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_THEM_SP]
GO
CREATE PROCEDURE [dbo].[SP_THEM_SP]
	@masp char(5),
	@madm char(5),
	@tensp nvarchar(120),
	@gia decimal(18,0),
	@thuonghieu varchar(30),
	@mota nvarchar(1000),
	@thongso nvarchar(1000),
	@soluong int,
	@anhsp varchar(500)
AS
BEGIN
	INSERT INTO SANPHAM(MASP, MADM, TENSP, GIA, THUONGHIEU, MOTA, THONGSO, SOLUONG, ANHSP) 
	VALUES (@masp, @madm, @tensp, @gia, @thuonghieu, @mota, @thongso, @soluong, @anhsp)
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_XOA_SP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_XOA_SP]
GO
CREATE PROCEDURE [dbo].[SP_XOA_SP]
	@MASP char(5)
AS
BEGIN
	DELETE FROM SANPHAM WHERE MASP = @MASP
END
GO

-- Functions
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DS_DANHMUC]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DS_DANHMUC]
GO
CREATE FUNCTION [dbo].[DS_DANHMUC] ()
RETURNS TABLE 
AS
RETURN 
(
	SELECT MADM, TENDM FROM DANHMUC
)
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DS_SANPHAM]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DS_SANPHAM]
GO
CREATE FUNCTION [dbo].[DS_SANPHAM] ()
RETURNS TABLE 
AS
RETURN 
(
	SELECT * FROM SANPHAM
)
GO

-- Initial Data
INSERT INTO [dbo].[NGUOIDUNG] ([MAND], [EMAIL], [VAITRO], [MATKHAU], [HOTEN], [SODIENTHOAI], [ANHDAIDIEN], [DIACHI]) 
VALUES ('ND001', 'admin@gmail.com', 'Admin', 'admin123', N'Quản trị viên', '0123456789', 'admin.png', N'Hà Nội')

INSERT INTO [dbo].[DANHMUC] ([MADM], [TENDM], [SOLUONG_SP]) VALUES ('DM001', N'Điện thoại', 0)
INSERT INTO [dbo].[DANHMUC] ([MADM], [TENDM], [SOLUONG_SP]) VALUES ('DM002', N'Laptop', 0)
INSERT INTO [dbo].[DANHMUC] ([MADM], [TENDM], [SOLUONG_SP]) VALUES ('DM003', N'Phụ kiện', 0)

INSERT INTO [dbo].[SANPHAM] ([MASP], [MADM], [TENSP], [GIA], [THUONGHIEU], [MOTA], [THONGSO], [SOLUONG], [ANHSP]) 
VALUES ('SP001', 'DM001', N'iPhone 15 Pro Max', 34000000, 'Apple', N'Điện thoại cao cấp nhất của Apple', N'Chip A17 Pro, RAM 8GB', 50, 'iphone15.jpg')
GO
