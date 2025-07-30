using System;

namespace Website_Giay.Models
{
    public partial class DonHang
    {
        private DateTime? _ThoiGianGiaoHang;
        private string _PhuongThucThanhToan;

        public DateTime? ThoiGianGiaoHang
        {
            get { return _ThoiGianGiaoHang; }
            set
            {
                if (_ThoiGianGiaoHang != value)
                {
                    OnThoiGianGiaoHangChanging(value);
                    SendPropertyChanging();
                    _ThoiGianGiaoHang = value;
                    SendPropertyChanged("ThoiGianGiaoHang");
                    OnThoiGianGiaoHangChanged();
                }
            }
        }

        public string PhuongThucThanhToan
        {
            get { return _PhuongThucThanhToan; }
            set
            {
                if (_PhuongThucThanhToan != value)
                {
                    OnPhuongThucThanhToanChanging(value);
                    SendPropertyChanging();
                    _PhuongThucThanhToan = value;
                    SendPropertyChanged("PhuongThucThanhToan");
                    OnPhuongThucThanhToanChanged();
                }
            }
        }

        partial void OnThoiGianGiaoHangChanging(DateTime? value);
        partial void OnThoiGianGiaoHangChanged();
        partial void OnPhuongThucThanhToanChanging(string value);
        partial void OnPhuongThucThanhToanChanged();
    }
} 