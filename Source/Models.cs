/****************/
// MSSV: 202418834
// Họ tên: Trần Tuấn Anh
/****************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace LabW07
{
    #region 1. GIAO DIỆN ĐỊNH DANH VÀ LỚP THIẾT BỊ CƠ SỞ TRỪU TƯỢNG

    /// <summary>
    /// Giao diện định danh tổng quát cho các đối tượng có mã định danh duy nhất (Id).
    /// Cho phép các cấu trúc dữ liệu tổng quát (Generic Repository) tìm kiếm và thao tác độc lập với lớp cụ thể.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// Mã định danh duy nhất của đối tượng.
        /// </summary>
        string Id { get; }
    }

    /// <summary>
    /// Lớp trừu tượng cơ sở đại diện cho mọi thiết bị dùng trong phòng dạy.
    /// Đóng gói các thuộc tính chung và cung cấp các phương thức trừu tượng để lớp con cài đặt đa hình.
    /// </summary>
    public abstract class TeachingDevice : IIdentifiable
    {
        /// <summary>
        /// Mã thiết bị (không được để trống).
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Tên gọi của thiết bị.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Vị trí lưu trữ thiết bị trong kho (ví dụ: Tủ A-01, Kệ B-02).
        /// </summary>
        public string StorageLocation { get; }

        /// <summary>
        /// Trạng thái hoạt động hiện tại của thiết bị.
        /// </summary>
        public DeviceStatus Status { get; protected set; }

        /// <summary>
        /// Khởi tạo một thiết bị giảng dạy với trạng thái ban đầu là Available.
        /// </summary>
        protected TeachingDevice(string id, string name, string storageLocation)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Mã thiết bị không được để trống.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên thiết bị không được để trống.", nameof(name));

            if (string.IsNullOrWhiteSpace(storageLocation))
                throw new ArgumentException("Vị trí lưu trữ không được để trống.", nameof(storageLocation));

            Id = id.Trim();
            Name = name.Trim();
            StorageLocation = storageLocation.Trim();
            Status = DeviceStatus.Available;
        }

        /// <summary>
        /// Kiểm tra khả năng sẵn sàng cấp phát của thiết bị.
        /// Phương thức trừu tượng được cài đặt đa hình tại các lớp dẫn xuất.
        /// </summary>
        /// <returns>True nếu thiết bị sẵn sàng; ngược lại False.</returns>
        public abstract bool IsReadyForUse();

        /// <summary>
        /// Tính toán điểm ưu tiên mặc định khi cấp phát thiết bị.
        /// Phương thức trừu tượng được cài đặt đa hình tại các lớp dẫn xuất.
        /// </summary>
        /// <returns>Điểm ưu tiên tính toán (double).</returns>
        public abstract double GetDefaultPriorityScore();

        /// <summary>
        /// Cấp phát mượn thiết bị. Chỉ thành công khi IsReadyForUse() trả về true.
        /// Chuyển trạng thái sang Borrowed.
        /// </summary>
        /// <exception cref="DeviceNotReadyException">Ném ra nếu thiết bị không sẵn sàng.</exception>
        public virtual void Borrow()
        {
            if (!IsReadyForUse())
            {
                throw new DeviceNotReadyException(Id, Status, 
                    $"Không thể mượn thiết bị [{Id}] ({Name}) vì thiết bị chưa sẵn sàng sử dụng (Trạng thái: {Status}).");
            }

            Status = DeviceStatus.Borrowed;
        }

        /// <summary>
        /// Trả thiết bị về kho. Chỉ thành công khi thiết bị đang ở trạng thái Borrowed.
        /// Chuyển trạng thái về Available.
        /// </summary>
        /// <exception cref="InvalidOperationException">Ném ra nếu thiết bị không ở trạng thái Borrowed.</exception>
        public virtual void Return()
        {
            if (Status != DeviceStatus.Borrowed)
            {
                throw new InvalidOperationException(
                    $"Không thể trả thiết bị [{Id}] vì thiết bị không ở trạng thái Borrowed (Trạng thái hiện tại: {Status}).");
            }

            Status = DeviceStatus.Available;
        }

        /// <summary>
        /// Đưa thiết bị vào diện bảo trì kỹ thuật.
        /// </summary>
        public virtual void MarkAsUnderMaintenance()
        {
            Status = DeviceStatus.UnderMaintenance;
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} | Vị trí: {StorageLocation} | Trạng thái: {Status} | Sẵn sàng: {(IsReadyForUse() ? "Có" : "Không")} | Điểm UT: {GetDefaultPriorityScore():F1}";
        }
    }

    #endregion

    #region 2. CÁC LỚP THIẾT BỊ GIẢNG DẠY CỤ THỂ (DẪN XUẤT TỪ TEACHINGDEVICE)

    /// <summary>
    /// Lớp biểu diễn thiết bị: Điều khiển máy chiếu (Projector Remote).
    /// </summary>
    public class ProjectorRemote : TeachingDevice
    {
        private int _batteryLevel;

        /// <summary>
        /// Mức dung lượng pin từ 0 đến 100%.
        /// </summary>
        public int BatteryLevel
        {
            get => _batteryLevel;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), "Mức pin của điều khiển máy chiếu phải từ 0 đến 100%.");
                _batteryLevel = value;
            }
        }

        public ProjectorRemote(string id, string name, string storageLocation, int batteryLevel)
            : base(id, name, storageLocation)
        {
            BatteryLevel = batteryLevel;
        }

        /// <summary>
        /// Sẵn sàng khi trạng thái là Available và mức pin từ 20% trở lên.
        /// </summary>
        public override bool IsReadyForUse()
        {
            return Status == DeviceStatus.Available && BatteryLevel >= 20;
        }

        /// <summary>
        /// Điểm ưu tiên mặc định bằng mức pin hiện tại.
        /// </summary>
        public override double GetDefaultPriorityScore()
        {
            return BatteryLevel;
        }

        public override string ToString()
        {
            return base.ToString() + $" | Pin: {BatteryLevel}%";
        }
    }

    /// <summary>
    /// Lớp biểu diễn thiết bị: Điều khiển điều hòa (Air Conditioner Remote).
    /// </summary>
    public class AirConditionerRemote : TeachingDevice
    {
        private readonly HashSet<string> _compatibleRooms;

        /// <summary>
        /// Danh sách tập hợp các phòng học tương thích với điều khiển này.
        /// </summary>
        public IReadOnlySet<string> CompatibleRooms => _compatibleRooms;

        public AirConditionerRemote(string id, string name, string storageLocation, IEnumerable<string>? compatibleRooms = null)
            : base(id, name, storageLocation)
        {
            _compatibleRooms = compatibleRooms != null 
                ? new HashSet<string>(compatibleRooms, StringComparer.OrdinalIgnoreCase) 
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra tính tương thích của điều khiển với một phòng học cụ thể.
        /// </summary>
        public bool IsCompatibleWith(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                return false;

            return _compatibleRooms.Contains(roomId.Trim());
        }

        /// <summary>
        /// Sẵn sàng khi trạng thái là Available.
        /// </summary>
        public override bool IsReadyForUse()
        {
            return Status == DeviceStatus.Available;
        }

        /// <summary>
        /// Điểm ưu tiên mặc định bằng 100 nếu có phòng tương thích, ngược lại bằng 0.
        /// </summary>
        public override double GetDefaultPriorityScore()
        {
            return _compatibleRooms.Count > 0 ? 100.0 : 0.0;
        }

        public override string ToString()
        {
            string rooms = _compatibleRooms.Count > 0 ? string.Join(", ", _compatibleRooms) : "Không có";
            return base.ToString() + $" | Phòng tương thích: [{rooms}]";
        }
    }

    /// <summary>
    /// Lớp biểu diễn thiết bị: Micro không dây (Wireless Microphone).
    /// </summary>
    public class WirelessMicrophone : TeachingDevice
    {
        private int _batteryLevel;
        private int _signalQuality;

        /// <summary>
        /// Mức dung lượng pin từ 0 đến 100%.
        /// </summary>
        public int BatteryLevel
        {
            get => _batteryLevel;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), "Mức pin của micro phải từ 0 đến 100%.");
                _batteryLevel = value;
            }
        }

        /// <summary>
        /// Chất lượng tín hiệu không dây từ 0 đến 100%.
        /// </summary>
        public int SignalQuality
        {
            get => _signalQuality;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), "Chất lượng tín hiệu của micro phải từ 0 đến 100%.");
                _signalQuality = value;
            }
        }

        public WirelessMicrophone(string id, string name, string storageLocation, int batteryLevel, int signalQuality)
            : base(id, name, storageLocation)
        {
            BatteryLevel = batteryLevel;
            SignalQuality = signalQuality;
        }

        /// <summary>
        /// Sẵn sàng khi trạng thái Available, mức pin từ 30% trở lên và chất lượng tín hiệu từ 40% trở lên.
        /// </summary>
        public override bool IsReadyForUse()
        {
            return Status == DeviceStatus.Available && BatteryLevel >= 30 && SignalQuality >= 40;
        }

        /// <summary>
        /// Điểm ưu tiên mặc định bằng trung bình cộng giữa mức pin và chất lượng tín hiệu.
        /// </summary>
        public override double GetDefaultPriorityScore()
        {
            return (BatteryLevel + SignalQuality) / 2.0;
        }

        public override string ToString()
        {
            return base.ToString() + $" | Pin: {BatteryLevel}% | Tín hiệu: {SignalQuality}%";
        }
    }

    #endregion

    #region 3. KHO LƯU TRỮ TỔNG QUÁT (GENERIC REPOSITORY)

    /// <summary>
    /// Interface kho lưu trữ tổng quát cho các đối tượng triển khai IIdentifiable.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể, ràng buộc where T : IIdentifiable.</typeparam>
    public interface IRepository<T> where T : IIdentifiable
    {
        void Add(T item);
        bool Remove(string id);
        T? FindById(string id);
        IReadOnlyList<T> GetAll();
        IReadOnlyList<T> FindAll(Predicate<T> condition);
    }

    /// <summary>
    /// Cài đặt Generic Repository lưu trữ dữ liệu trong bộ nhớ (In-Memory).
    /// Sử dụng Dictionary&lt;string, T&gt; để tối ưu hóa hiệu năng tra cứu O(1) và bảo toàn tính duy nhất của mã Id.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể triển khai IIdentifiable.</typeparam>
    public class InMemoryRepository<T> : IRepository<T> where T : IIdentifiable
    {
        private readonly Dictionary<string, T> _items = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Thêm phần tử vào kho lưu trữ.
        /// </summary>
        /// <exception cref="ArgumentNullException">Ném ra nếu item là null.</exception>
        /// <exception cref="DuplicateIdException">Ném ra nếu mã Id của phần tử đã tồn tại.</exception>
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Không thể thêm phần tử null vào kho lưu trữ.");

            if (string.IsNullOrWhiteSpace(item.Id))
                throw new ArgumentException("Mã Id của phần tử không được để trống.", nameof(item));

            if (_items.ContainsKey(item.Id))
                throw new DuplicateIdException(item.Id);

            _items.Add(item.Id, item);
        }

        /// <summary>
        /// Xóa phần tử khỏi kho theo mã Id.
        /// </summary>
        /// <returns>True nếu xóa thành công; False nếu không tìm thấy mã.</returns>
        public bool Remove(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            return _items.Remove(id);
        }

        /// <summary>
        /// Tìm kiếm phần tử theo mã định danh Id.
        /// </summary>
        /// <returns>Đối tượng nếu tìm thấy, hoặc null nếu không tồn tại.</returns>
        public T? FindById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            return _items.TryGetValue(id, out var found) ? found : default;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phần tử trong kho dưới dạng danh sách chỉ đọc (Read-only).
        /// Không làm lộ collection nội bộ ra bên ngoài.
        /// </summary>
        public IReadOnlyList<T> GetAll()
        {
            return _items.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Lọc danh sách phần tử theo một điều kiện Predicate.
        /// </summary>
        public IReadOnlyList<T> FindAll(Predicate<T> condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition), "Điều kiện lọc không được null.");

            return _items.Values.Where(item => condition(item)).ToList().AsReadOnly();
        }
    }

    #endregion

    #region 4. CHIẾN LƯỢC LỰA CHỌN THIẾT BỊ (STRATEGY PATTERN & GENERIC)

    /// <summary>
    /// Giao diện chiến lược lựa chọn thiết bị tốt nhất từ danh sách ứng viên (Strategy Pattern).
    /// </summary>
    /// <typeparam name="T">Kiểu đối tượng cần lựa chọn.</typeparam>
    public interface ISelectionStrategy<T>
    {
        /// <summary>
        /// Chọn một đối tượng tốt nhất từ danh sách ứng viên.
        /// </summary>
        /// <param name="candidates">Danh sách ứng viên chỉ đọc.</param>
        /// <returns>Đối tượng được chọn theo chiến lược.</returns>
        /// <exception cref="NoSuitableItemException">Ném ra nếu danh sách rỗng.</exception>
        T Select(IReadOnlyList<T> candidates);
    }

    /// <summary>
    /// Chiến lược chọn đối tượng có điểm ưu tiên cao nhất.
    /// Cho phép tùy biến linh hoạt hàm tính điểm thông qua Func&lt;T, double&gt;.
    /// </summary>
    public class HighestPriorityStrategy<T> : ISelectionStrategy<T>
    {
        private readonly Func<T, double> _scoreSelector;

        public HighestPriorityStrategy(Func<T, double> scoreSelector)
        {
            _scoreSelector = scoreSelector ?? throw new ArgumentNullException(nameof(scoreSelector), "Hàm tính điểm không được null.");
        }

        public T Select(IReadOnlyList<T> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                throw new NoSuitableItemException("Danh sách ứng viên rỗng, không thể chọn đối tượng có điểm cao nhất.");

            T bestCandidate = candidates[0];
            double maxScore = _scoreSelector(bestCandidate);

            for (int i = 1; i < candidates.Count; i++)
            {
                double score = _scoreSelector(candidates[i]);
                if (score > maxScore)
                {
                    maxScore = score;
                    bestCandidate = candidates[i];
                }
            }

            return bestCandidate;
        }
    }

    /// <summary>
    /// Chiến lược chọn phần tử sẵn sàng đầu tiên trong danh sách ứng viên (First Available).
    /// </summary>
    public class FirstAvailableStrategy<T> : ISelectionStrategy<T>
    {
        public T Select(IReadOnlyList<T> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                throw new NoSuitableItemException("Danh sách ứng viên rỗng, không có thiết bị nào sẵn sàng để chọn.");

            return candidates[0];
        }
    }

    #endregion

    #region 5. DỊCH VỤ CẤP PHÁT TỔNG QUÁT (ALLOCATION SERVICE)

    /// <summary>
    /// Dịch vụ cấp phát thiết bị tổng quát (Generic Allocation Service).
    /// Phối hợp giữa IRepository&lt;T&gt; và ISelectionStrategy&lt;T&gt; thông qua trừu tượng.
    /// Tuân thủ nguyên lý Open/Closed Principle: Hoàn toàn không sử dụng câu lệnh rẽ nhánh kiểu (if is / type check).
    /// </summary>
    /// <typeparam name="T">Kiểu thiết bị, ràng buộc where T : TeachingDevice.</typeparam>
    public class AllocationService<T> where T : TeachingDevice
    {
        private readonly IRepository<T> _repository;
        private ISelectionStrategy<T> _strategy;

        public AllocationService(IRepository<T> repository, ISelectionStrategy<T> strategy)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Kho lưu trữ không được null.");
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy), "Chiến lược lựa chọn không được null.");
        }

        /// <summary>
        /// Cho phép hoán đổi chiến lược lựa chọn linh hoạt tại thời gian chạy (Runtime Strategy Swapping).
        /// </summary>
        public void ChangeStrategy(ISelectionStrategy<T> newStrategy)
        {
            _strategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy), "Chiến lược mới không được null.");
        }

        /// <summary>
        /// Quy trình nghiệp vụ cấp phát thiết bị:
        /// 1. Lấy danh sách từ repository.
        /// 2. Lọc chỉ giữ các thiết bị IsReadyForUse() (đa hình).
        /// 3. Áp dụng thêm điều kiện bổ sung nếu có.
        /// 4. Dùng chiến lược hiện tại để chọn thiết bị tối ưu.
        /// 5. Gọi Borrow() trên thiết bị được chọn.
        /// 6. Trả về thiết bị đã cấp phát.
        /// </summary>
        public T Allocate(Predicate<T>? additionalCondition = null)
        {
            // 1. Lấy các đối tượng từ repository
            var allItems = _repository.GetAll();

            // 2 & 3. Sàng lọc ứng viên sẵn sàng và thỏa mãn điều kiện
            var candidates = allItems
                .Where(device => device.IsReadyForUse() && (additionalCondition == null || additionalCondition(device)))
                .ToList();

            if (candidates.Count == 0)
                throw new NoSuitableItemException("Không tìm thấy thiết bị nào sẵn sàng và thỏa mãn điều kiện yêu cầu.");

            // 4. Chọn thiết bị theo chiến lược hiện tại
            T selected = _strategy.Select(candidates.AsReadOnly());

            // 5. Cấp phát mượn thiết bị
            selected.Borrow();

            // 6. Trả về thiết bị
            return selected;
        }
    }

    #endregion
}
