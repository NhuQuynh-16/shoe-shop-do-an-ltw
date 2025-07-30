-- Thêm cột ThoiGianGiaoHang và PhuongThucThanhToan vào bảng DonHang
ALTER TABLE DonHang
ADD ThoiGianGiaoHang DateTime NULL,
    PhuongThucThanhToan nvarchar(50) NULL; 