using System.Collections.Generic;

namespace Workflow
{
    public static class Database
    {
        public static ICollection<NhiemVu> NhiemVus { get; set; } = new List<NhiemVu>();
        public static ICollection<PhanXuLyNhiemVu> PhanXuLyNhiemVus { get; set; } = new List<PhanXuLyNhiemVu>();
    }

    public class NhiemVu
    {
        public int Id { get; set; }
        public TrangThaiNhiemVu TrangThai { get; set; }
        public int CanBoId { get; set; }
        public int DonViId { get; set; }
        public string WorkflowId { get; set; }
    }

    public class PhanXuLyNhiemVu
    {
        public int Id { get; set; }
        public int CanBoId { get; set; }
        public int DonViId { get; set; }
        public int NhiemVuId { get; set; }
        public int? PhanXuLyNhiemVuChaId { get; set; }
        public VaiTroXuLy VaiTroXuLy { get; set; }
        public TrangThaiPhanXuLy TrangThai { get; set; }
        public string WorkflowId { get; set; }
    }

    public enum VaiTroXuLy
    {
        ChuTri,
        PhoiHop
    }

    public enum TrangThaiNhiemVu
    {
        ChoThucHien,
        DangThucHien,
        DaHoanThanh
    }

    public enum TrangThaiPhanXuLy
    {
        DaTraLai,
        DaThuHoi,
        DangThucHien
    }
}