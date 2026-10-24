/****************/
// MSSV: 202418834
// Họ tên: Trần Tuấn Anh
/****************/

namespace LabW07
{
    /// <summary>
    /// Liệt kê các trạng thái hoạt động của thiết bị giảng dạy trong hệ thống.
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// Thiết bị sẵn sàng phục vụ mượn / cấp phát.
        /// </summary>
        Available,

        /// <summary>
        /// Thiết bị hiện đang được mượn sử dụng.
        /// </summary>
        Borrowed,

        /// <summary>
        /// Thiết bị đang trong quá trình bảo trì, sửa chữa hoặc kiểm định kỹ thuật.
        /// </summary>
        UnderMaintenance
    }
}
