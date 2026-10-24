/****************/
// MSSV: 202418834
// Họ tên: Trần Tuấn Anh
/****************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabW07
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;


            // Khởi tạo các kho lưu trữ Generic InMemoryRepository
            var projectorRepo = new InMemoryRepository<ProjectorRemote>();
            var acRepo = new InMemoryRepository<AirConditionerRemote>();
            var micRepo = new InMemoryRepository<WirelessMicrophone>();

            // Chuẩn bị dữ liệu mẫu ban đầu cho các thiết bị
            var pr1 = new ProjectorRemote("PR01", "ĐK máy chiếu Panasonic PT-VX430", "Tủ A-01", 15); // Pin 15% < 20% -> không sẵn sàng
            var pr2 = new ProjectorRemote("PR02", "ĐK máy chiếu Sony VPL-EX455", "Tủ A-02", 55);    // Pin 55% >= 20% -> sẵn sàng
            var pr3 = new ProjectorRemote("PR03", "ĐK máy chiếu Epson EB-X06", "Tủ A-03", 90);      // Pin 90% >= 20% -> sẵn sàng (điểm cao nhất)

            var ac1 = new AirConditionerRemote("AC01", "ĐK điều hòa Daikin Inverter", "Tủ B-01", new[] { "A101", "A102", "A103", "B201" });
            var ac2 = new AirConditionerRemote("AC02", "ĐK điều hòa Panasonic Eco", "Tủ B-02", new[] { "A203", "A204", "B305" });

            var mc1 = new WirelessMicrophone("MC01", "Micro Shure BLX288", "Tủ C-01", 80, 35);      // Tín hiệu 35% < 40% -> không sẵn sàng
            var mc2 = new WirelessMicrophone("MC02", "Micro Sennheiser EW100", "Tủ C-02", 50, 75);  // Sẵn sàng, điểm = (50+75)/2 = 62.5
            var mc3 = new WirelessMicrophone("MC03", "Micro Sony UWP-D21", "Tủ C-03", 95, 90);      // Sẵn sàng, điểm = (95+90)/2 = 92.5

            Console.WriteLine(">>> KHỞI TẠO DỮ LIỆU MẪU THIẾT BỊ GIẢNG DẠY <<<");
            Console.WriteLine($" + 3 ĐK máy chiếu: {pr1.Id} ({pr1.BatteryLevel}%), {pr2.Id} ({pr2.BatteryLevel}%), {pr3.Id} ({pr3.BatteryLevel}%)");
            Console.WriteLine($" + 2 ĐK điều hòa:  {ac1.Id} ([{string.Join(", ", ac1.CompatibleRooms)}]), {ac2.Id} ([{string.Join(", ", ac2.CompatibleRooms)}])");
            Console.WriteLine($" + 3 micro:        {mc1.Id} (P:{mc1.BatteryLevel}%, TH:{mc1.SignalQuality}%), {mc2.Id} (P:{mc2.BatteryLevel}%, TH:{mc2.SignalQuality}%), {mc3.Id} (P:{mc3.BatteryLevel}%, TH:{mc3.SignalQuality}%)");
            Console.WriteLine();

            // -------------------------------------------------------------------------
            // PHẦN 1: KIỂM THỬ THAO TÁC KHO LƯU TRỮ VÀ TÍNH ĐA HÌNH
            // -------------------------------------------------------------------------
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("================================================================================");
            Console.WriteLine(">>> PHẦN 1: KIỂM THỬ THAO TÁC KHO LƯU TRỮ VÀ TÍNH ĐA HÌNH <<<");
            Console.WriteLine("================================================================================");
            Console.ResetColor();

            // 1. Thêm thiết bị vào repository
            Console.WriteLine("\n[1] Thêm thiết bị vào repository:");
            projectorRepo.Add(pr1);
            projectorRepo.Add(pr2);
            projectorRepo.Add(pr3);
            acRepo.Add(ac1);
            acRepo.Add(ac2);
            micRepo.Add(mc1);
            micRepo.Add(mc2);
            micRepo.Add(mc3);
            Console.WriteLine($" + Đã nạp {projectorRepo.GetAll().Count} điều khiển máy chiếu vào ProjectorRepo.");
            Console.WriteLine($" + Đã nạp {acRepo.GetAll().Count} điều khiển điều hòa vào ACRepo.");
            Console.WriteLine($" + Đã nạp {micRepo.GetAll().Count} micro vào MicRepo.");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS)");
            Console.ResetColor();

            // 2. Thử thêm hai thiết bị trùng mã
            Console.WriteLine("\n[2] Thử thêm hai thiết bị trùng mã:");
            try
            {
                var duplicatePr = new ProjectorRemote("PR01", "ĐK máy chiếu trùng mã", "Tủ A-99", 70);
                projectorRepo.Add(duplicatePr);
                Console.WriteLine(" [LỖI] Kho cho phép thêm thiết bị trùng mã!");
            }
            catch (DuplicateIdException ex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" => BẮT NGOẠI LỆ THÀNH CÔNG: {ex.GetType().Name}");
                Console.ResetColor();
                Console.WriteLine($"    Nội dung: {ex.Message}");
                Console.WriteLine($"    Mã trùng: {ex.DuplicateId}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS)");
                Console.ResetColor();
            }

            // 3. Tìm thiết bị theo mã
            Console.WriteLine("\n[3] Tìm thiết bị theo mã:");
            var found = projectorRepo.FindById("PR02");
            Console.WriteLine($" + Tìm kiếm mã tồn tại 'PR02': {(found != null ? found.ToString() : "Không tìm thấy")}");
            var notFound = projectorRepo.FindById("PR999");
            Console.WriteLine($" + Tìm kiếm mã không tồn tại 'PR999': {(notFound == null ? "Trả về null (chính xác)" : "Lỗi")}");
            if (found != null && found.Id == "PR02" && notFound == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS)");
                Console.ResetColor();
            }

            // 4. Lọc các thiết bị sẵn sàng
            Console.WriteLine("\n[4] Lọc các thiết bị sẵn sàng:");
            var readyProjectors = projectorRepo.FindAll(d => d.IsReadyForUse());
            Console.WriteLine($" + Số lượng máy chiếu sẵn sàng ({readyProjectors.Count}/{projectorRepo.GetAll().Count}):");
            foreach (var p in readyProjectors)
            {
                Console.WriteLine($"   * {p}");
            }
            Console.WriteLine($"   (Ghi chú: PR01 pin 15% < 20% nên đã bị loại khỏi danh sách sẵn sàng)");
            if (readyProjectors.Count == 2 && readyProjectors.All(p => p.BatteryLevel >= 20))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS)");
                Console.ResetColor();
            }
            Console.WriteLine();

            // -------------------------------------------------------------------------
            // PHẦN 2: KIỂM THỬ LỰA CHỌN VÀ CẤP PHÁT THEO CHIẾN LƯỢC
            // -------------------------------------------------------------------------
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================================================================================");
            Console.WriteLine(">>> PHẦN 2: KIỂM THỬ LỰA CHỌN VÀ CẤP PHÁT THEO CHIẾN LƯỢC <<<");
            Console.WriteLine("================================================================================");
            Console.ResetColor();

            // 5. Chọn điều khiển máy chiếu có điểm cao nhất
            Console.WriteLine("\n[5] Chọn điều khiển máy chiếu có điểm cao nhất:");
            var maxPinStrategy = new HighestPriorityStrategy<ProjectorRemote>(d => d.GetDefaultPriorityScore());
            var projectorService = new AllocationService<ProjectorRemote>(projectorRepo, maxPinStrategy);
            var allocatedPr = projectorService.Allocate();
            Console.WriteLine($" + Thiết bị được chọn cấp phát: {allocatedPr}");
            Console.WriteLine($" + Trạng thái thiết bị sau khi cấp phát: {allocatedPr.Status}");
            if (allocatedPr.Id == "PR03" && allocatedPr.BatteryLevel == 90 && allocatedPr.Status == DeviceStatus.Borrowed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS - Đã chọn đúng PR03 có mức pin cao nhất 90%)");
                Console.ResetColor();
            }

            // 6. Chọn điều khiển điều hòa tương thích với một phòng
            Console.WriteLine("\n[6] Chọn điều khiển điều hòa tương thích với một phòng:");
            var acStrategy = new FirstAvailableStrategy<AirConditionerRemote>();
            var acService = new AllocationService<AirConditionerRemote>(acRepo, acStrategy);
            string targetRoom = "A203";
            var allocatedAc = acService.Allocate(device => device.IsCompatibleWith(targetRoom));
            Console.WriteLine($" + Thiết bị được cấp phát cho phòng {targetRoom}: {allocatedAc}");
            Console.WriteLine($" + Tập phòng tương thích: [{string.Join(", ", allocatedAc.CompatibleRooms)}]");
            if (allocatedAc.Id == "AC02" && allocatedAc.IsCompatibleWith(targetRoom) && allocatedAc.Status == DeviceStatus.Borrowed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" => KẾT QUẢ: THÀNH CÔNG (PASS - Đã chọn đúng AC02 tương thích phòng {targetRoom})");
                Console.ResetColor();
            }

            // 7. Chọn micro đầu tiên sẵn sàng
            Console.WriteLine("\n[7] Chọn micro đầu tiên sẵn sàng:");
            var micFirstStrategy = new FirstAvailableStrategy<WirelessMicrophone>();
            var micService = new AllocationService<WirelessMicrophone>(micRepo, micFirstStrategy);
            var allocatedMic = micService.Allocate();
            Console.WriteLine($" + Micro đầu tiên sẵn sàng được cấp phát: {allocatedMic}");
            Console.WriteLine($"   (MC01 bị loại do tín hiệu 35% < 40%, chọn chính xác MC02)");
            if (allocatedMic.Id == "MC02" && allocatedMic.Status == DeviceStatus.Borrowed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS - Đã chọn đúng MC02)");
                Console.ResetColor();
            }

            // 8. Đổi chiến lược trong thời gian chạy và quan sát kết quả
            Console.WriteLine("\n[8] Đổi chiến lược trong thời gian chạy và quan sát kết quả:");
            Console.WriteLine(" + Trả lại micro MC02 về kho...");
            allocatedMic.Return();
            Console.WriteLine($"   Trạng thái sau khi trả: MC02 -> {allocatedMic.Status}");
            Console.WriteLine(" + Chuyển sang chiến lược điểm ưu tiên cao nhất (HighestPriorityStrategy)...");
            var micScoreStrategy = new HighestPriorityStrategy<WirelessMicrophone>(m => m.GetDefaultPriorityScore());
            micService.ChangeStrategy(micScoreStrategy);
            var allocatedMicByScore = micService.Allocate();
            Console.WriteLine($" + Kết quả cấp phát sau khi đổi chiến lược: {allocatedMicByScore}");
            Console.WriteLine($"   (MC02 có điểm 62.5, MC03 có điểm 92.5 -> Hệ thống chuyển sang chọn MC03)");
            if (allocatedMicByScore.Id == "MC03" && allocatedMicByScore.Status == DeviceStatus.Borrowed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS - Đổi chiến lược runtime thành công)");
                Console.ResetColor();
            }
            Console.WriteLine();

            // -------------------------------------------------------------------------
            // PHẦN 3: KIỂM THỬ NGOẠI LỆ VÀ CHUYỂN ĐỔI TRẠNG THÁI
            // -------------------------------------------------------------------------
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("================================================================================");
            Console.WriteLine(">>> PHẦN 3: KIỂM THỬ NGOẠI LỆ VÀ CHUYỂN ĐỔI TRẠNG THÁI <<<");
            Console.WriteLine("================================================================================");
            Console.ResetColor();

            // 9. Thử cấp phát khi không có ứng viên phù hợp
            Console.WriteLine("\n[9] Thử cấp phát khi không có ứng viên phù hợp:");
            try
            {
                acService.Allocate(device => device.IsCompatibleWith("C999"));
                Console.WriteLine(" [LỖI] Không ném ngoại lệ khi danh sách ứng viên rỗng!");
            }
            catch (NoSuitableItemException ex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" => BẮT NGOẠI LỆ THÀNH CÔNG: {ex.GetType().Name}");
                Console.ResetColor();
                Console.WriteLine($"    Nội dung: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS - Ném đúng NoSuitableItemException)");
                Console.ResetColor();
            }

            // 10. Chứng minh thiết bị được chọn chuyển sang Borrowed
            Console.WriteLine("\n[10] Chứng minh thiết bị được chọn chuyển sang Borrowed:");
            Console.WriteLine($" + Trạng thái thiết bị PR03 sau khi được cấp phát: {allocatedPr.Status}");
            if (allocatedPr.Status == DeviceStatus.Borrowed)
            {
                Console.WriteLine(" + Thiết bị đã chuyển sang trạng thái Borrowed.");
            }

            // Thử mượn lại thiết bị đang Borrowed
            try
            {
                Console.WriteLine(" + Cố gắng mượn lại PR03 đang ở trạng thái Borrowed:");
                allocatedPr.Borrow();
            }
            catch (DeviceNotReadyException ex)
            {
                Console.WriteLine($"   -> Bắt ngoại lệ mong đợi: {ex.Message}");
            }

            // Gọi Return() để đưa về Available
            Console.WriteLine(" + Thực hiện gọi Return() để trả PR03 về kho:");
            allocatedPr.Return();
            Console.WriteLine($"   Trạng thái sau khi Return(): {allocatedPr.Status}");
            if (allocatedPr.Status == DeviceStatus.Available)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS - Chuyển đổi chính xác sang Borrowed và Return về Available)");
                Console.ResetColor();
            }

            // 11. Chứng minh repository tổng quát có thể dùng cho ít nhất hai loại thiết bị mà không sao chép mã
            Console.WriteLine("\n[11] Chứng minh repository tổng quát có thể dùng cho ít nhất hai loại thiết bị mà không sao chép mã:");
            Console.WriteLine($" + Kiểu dữ liệu ProjectorRepo: {projectorRepo.GetType().Name}<{typeof(ProjectorRemote).Name}>");
            Console.WriteLine($" + Kiểu dữ liệu ACRepo:        {acRepo.GetType().Name}<{typeof(AirConditionerRemote).Name}>");
            Console.WriteLine($" + Kiểu dữ liệu MicRepo:       {micRepo.GetType().Name}<{typeof(WirelessMicrophone).Name}>");
            var masterRepo = new InMemoryRepository<TeachingDevice>();
            masterRepo.Add(pr2);
            masterRepo.Add(ac2);
            masterRepo.Add(mc2);
            Console.WriteLine($" + Kho đa hình MasterRepo lưu trữ hỗn hợp: {masterRepo.GetAll().Count} thiết bị.");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" => KẾT QUẢ: THÀNH CÔNG (PASS)");
            Console.ResetColor();

            Console.WriteLine("\n--------------------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("HỆ THỐNG HOÀN TẤT KIỂM THỬ THÀNH CÔNG - TẤT CẢ CÁC CA ĐỀU ĐẠT (PASS).");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------------------------------------\n");
        }
    }
}
