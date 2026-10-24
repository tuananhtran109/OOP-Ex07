/****************/
// MSSV: 202418834
// Họ tên: Trần Tuấn Anh
/****************/

using System;

namespace LabW07
{
    /// <summary>
    /// Ngoại lệ cơ sở cho toàn bộ các ngoại lệ nghiệp vụ quản lý và cấp phát thiết bị.
    /// </summary>
    public class DeviceException : Exception
    {
        public DeviceException(string message)
            : base(message)
        {
        }

        public DeviceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Phát sinh khi thêm vào kho lưu trữ một đối tượng có mã định danh (Id) đã tồn tại.
    /// </summary>
    public class DuplicateIdException : DeviceException
    {
        /// <summary>
        /// Mã định danh bị trùng lặp.
        /// </summary>
        public string DuplicateId { get; }

        public DuplicateIdException(string duplicateId, string message)
            : base(message)
        {
            DuplicateId = duplicateId;
        }

        public DuplicateIdException(string duplicateId)
            : this(duplicateId, $"Phần tử có mã định danh Id '[{duplicateId}]' đã tồn tại trong kho lưu trữ.")
        {
        }
    }

    /// <summary>
    /// Phát sinh khi không tìm thấy thiết bị nào trong kho thỏa mãn tiêu chí lựa chọn hoặc điều kiện cấp phát.
    /// </summary>
    public class NoSuitableItemException : DeviceException
    {
        public NoSuitableItemException(string message)
            : base(message)
        {
        }

        public NoSuitableItemException()
            : base("Không tìm thấy thiết bị nào sẵn sàng và thỏa mãn điều kiện yêu cầu để cấp phát.")
        {
        }
    }

    /// <summary>
    /// Phát sinh khi cố gắng mượn một thiết bị chưa sẵn sàng sử dụng (IsReadyForUse trả về false).
    /// </summary>
    public class DeviceNotReadyException : DeviceException
    {
        /// <summary>
        /// Mã thiết bị chưa sẵn sàng.
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// Trạng thái hiện tại của thiết bị.
        /// </summary>
        public DeviceStatus CurrentStatus { get; }

        public DeviceNotReadyException(string deviceId, DeviceStatus currentStatus, string message)
            : base(message)
        {
            DeviceId = deviceId;
            CurrentStatus = currentStatus;
        }

        public DeviceNotReadyException(string deviceId, DeviceStatus currentStatus)
            : this(deviceId, currentStatus, $"Thiết bị [{deviceId}] hiện chưa sẵn sàng để mượn (Trạng thái hiện tại: {currentStatus}).")
        {
        }
    }
}
