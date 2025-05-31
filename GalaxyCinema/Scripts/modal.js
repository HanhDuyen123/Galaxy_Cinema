// ==== BIẾN TOÀN CỤC ======================================================
let dayTypeData = {
    // Sau khi load từ server, sẽ chứa:
    // { NgayThuong: [...], HappyDay: [...], NgayCuoiTuan: [...], NgayLe: [...] }
};

let holidays = [];          // Mảng tạm giữ các holiday hiện tại (chuỗi "MM-DD")
let enabledHolidays = new Set();  // Set tạm giữ các holiday đang được chọn

// ==== HÀM HỖ TRỢ CHUNG ===================================================

// 1. Get DayName từ enum số (nếu cần, hoặc có thể không dùng tới)
function getDayName(dayEnum) {
    const dayNames = {
        8: 'Sunday',
        2: 'Monday',
        3: 'Tuesday',
        4: 'Wednesday',
        5: 'Thursday',
        6: 'Friday',
        7: 'Saturday'
    };
    return dayNames[dayEnum] || dayEnum;
}

// 2. Hiển thị alert (type: 'success' | 'danger' | 'warning' | 'info')
function showAlert(message, type) {
    const alertDiv = $('#alertMessage');
    alertDiv
        .removeClass('alert-success alert-danger alert-warning alert-info')
        .addClass('alert-' + type)
        .html(message)
        .show();
    setTimeout(() => alertDiv.fadeOut(), 5000);
}

// 3. Thu thập dữ liệu từ các checkbox NgayThuong / HappyDay / NgayCuoiTuan
function collectFormData() {
    const formData = {
        NgayThuong: [],
        HappyDay: [],
        NgayCuoiTuan: []
    };

    $('input[name="NgayThuong"]:checked').each(function () {
        formData.NgayThuong.push($(this).val());
    });
    $('input[name="HappyDay"]:checked').each(function () {
        formData.HappyDay.push($(this).val());
    });
    $('input[name="NgayCuoiTuan"]:checked').each(function () {
        formData.NgayCuoiTuan.push($(this).val());
    });

    return formData;
}

// 4. Khởi tạo dropdown Tháng – Ngày
function initMonthDayDropdowns() {
    const monthSelect = document.getElementById('monthSelect');
    const daySelect = document.getElementById('daySelect');

    // 4.1. Điền tháng "01" → "12"
    monthSelect.innerHTML = '';
    for (let m = 1; m <= 12; m++) {
        const opt = document.createElement('option');
        const twoDigit = m < 10 ? '0' + m : String(m);
        opt.value = twoDigit;
        opt.textContent = twoDigit;
        monthSelect.appendChild(opt);
    }

    // 4.2. Cập nhật lần đầu dropdown ngày (theo tháng 1)
    updateDays();
}

// 5. Cập nhật dropdown Ngày dựa vào tháng đã chọn
function updateDays() {
    const monthSelect = document.getElementById('monthSelect');
    const daySelect = document.getElementById('daySelect');

    const month = parseInt(monthSelect.value, 10); // 1–12
    const year = new Date().getFullYear();
    const daysInMonth = new Date(year, month, 0).getDate();

    daySelect.innerHTML = '';
    for (let d = 1; d <= daysInMonth; d++) {
        const opt = document.createElement('option');
        const twoDigitDay = d < 10 ? '0' + d : String(d);
        opt.value = twoDigitDay;
        opt.textContent = twoDigitDay;
        daySelect.appendChild(opt);
    }
}

// 6. Render bảng Ngày Lễ hiện có vào <tbody id="holidaysContainer">
function renderHolidays() {
    const tbody = document.getElementById('holidaysContainer');
    if (!tbody) return;

    if (holidays.length === 0) {
        tbody.innerHTML = `
      <tr>
        <td colspan="2" class="text-center text-muted py-3">
          Chưa có ngày lễ nào.
        </td>
      </tr>`;
        return;
    }

    let html = '';
    holidays.forEach((mmdd, idx) => {
        const [month, day] = mmdd.split('-');
        const displayDate = `${day}/${month}`;
        html += `
      <tr>
        <td>
          <span class="badge bg-primary">${displayDate}</span>
        </td>
        <td>
          <button
            class="btn btn-sm btn-outline-danger"
            onclick="removeHoliday(${idx})"
            title="Xóa"
          >
            <i class="fas fa-trash-alt"></i>
          </button>
        </td>
      </tr>`;
    });
    tbody.innerHTML = html;
}

// 7. Xóa Ngày Lễ theo index rồi render lại (về phía client)
function removeHoliday(idx) {
    const removed = holidays.splice(idx, 1)[0];
    enabledHolidays.delete(removed);
    renderHolidays();
}

// ==== HÀM GỌP VIỆC MỞ MODAL + KHỞI TẠO DỮ LIỆU ============================

// (A) Khi người dùng nhấn nút mở modal, gọi AJAX để load PartialView modal
function openDayTypeModal() {
    $.ajax({
        url: '/DAYCATs/EditDayCategory', // Controller trả về PartialView chứa modal
        type: 'GET',
        success: function (html) {
            // A.1. Xóa modal cũ nếu còn tồn tại
            $('#editDayTypeModal').remove();

            // A.2. Append modal mới vào body
            $('body').append(html);

            // A.3. Show modal
            $('#editDayTypeModal').modal('show');

            // A.4. Sau khi modal đã được append, ta bind các event & load data
            bindModalEvents();
            loadDayTypeSettings();
        },
        error: function () {
            showAlert('Lỗi khi tải modal!', 'danger');
        }
    });
}

// (B) Bind các event chỉ xuất hiện sau khi modal đã được append
function bindModalEvents() {
    // B.1. Khi chọn tháng, cập nhật ngày
    $('#monthSelect').off('change', updateDays).on('change', updateDays);

    // B.2. Khi click "Thêm" (addBtn) → gọi AJAX AddHoliday
    $('#addBtn').off('click').on('click', function (e) {
        e.preventDefault();
        postAddHoliday();
    });

    // B.3. Khi click "Lưu" (saveDayTypeBtn) → gọi AJAX UpdateDayTypeSettings
    $('#saveDayTypeBtn').off('click').on('click', function (e) {
        e.preventDefault();
        saveDayCategories();
    });
}

// ==== 8. Load dữ liệu cài đặt từ server (GET /GetDayTypeSettings) ================
function loadDayTypeSettings() {
    $.ajax({
        url: '/DAYCATs/GetDayTypeSettings', // Phải là GET để lấy JSON ban đầu
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // 8.1. Lưu tạm vào dayTypeData
            dayTypeData = data;

            // 8.2. Prefill các checkbox
            populateCheckboxes(data);

            // 8.3. Prefill danh sách Ngày Lễ (mảng "MM-DD")
            const ngayLeArray = data.NgayLe || [];
            holidays = [...ngayLeArray];
            enabledHolidays = new Set(ngayLeArray);
            renderHolidays();

            // 8.4. Khởi tạo dropdown Tháng–Ngày và gán sự kiện change
            initMonthDayDropdowns();
            $('#monthSelect').off('change', updateDays).on('change', updateDays);
        },
        error: function (xhr, status, error) {
            console.error('AJAX Error:', error);
            showAlert('Lỗi khi tải dữ liệu!', 'danger');
        }
    });
}

// 9. Điền checkbox (prefill) dựa vào dayTypeData
function populateCheckboxes(data) {
    // 9.1. Bỏ tick hết tất cả checkbox trước
    $('#dayTypeForm input[type="checkbox"]').prop('checked', false);

    // 9.2. Ngày Thường
    if (Array.isArray(data.NgayThuong)) {
        data.NgayThuong.forEach(function (dayEnum) {
            const dayName = getDayName(dayEnum);
            $(`input[name="NgayThuong"][value="${dayName}"]`).prop('checked', true);
        });
    }
    // 9.3. Happy Day
    if (Array.isArray(data.HappyDay)) {
        data.HappyDay.forEach(function (dayEnum) {
            const dayName = getDayName(dayEnum);
            $(`input[name="HappyDay"][value="${dayName}"]`).prop('checked', true);
        });
    }
    // 9.4. Ngày Cuối Tuần
    if (Array.isArray(data.NgayCuoiTuan)) {
        data.NgayCuoiTuan.forEach(function (dayEnum) {
            const dayName = getDayName(dayEnum);
            $(`input[name="NgayCuoiTuan"][value="${dayName}"]`).prop('checked', true);
        });
    }
}

// ==== 10. Hàm POST thêm 1 Ngày Lễ mới (AddHoliday lên server) ===============
function postAddHoliday() {
    // Lấy ngày đang chọn
    const month = $('#monthSelect').val(); // "MM"
    const day = $('#daySelect').val();     // "DD"
    const key = `${month}-${day}`;         // "MM-DD"

    if (!key) {
        showAlert('Vui lòng chọn ngày và tháng hợp lệ!', 'warning');
        return;
    }

    // Nếu đã tồn tại ở client (chưa call server) thì cảnh báo, tránh call network
    if (holidays.includes(key)) {
        showAlert(`Ngày lễ ${day}/${month} đã tồn tại!`, 'warning');
        return;
    }

    // Gọi AJAX AddHoliday lên server để thêm vào DB
    $.ajax({
        url: '/DAYCATs/AddHoliday',
        type: 'POST',
        data: { holidayValue: key },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                // Thêm thành công ở server → cập nhật mảng client
                holidays.push(key);
                enabledHolidays.add(key);
                renderHolidays();
                showAlert(`Đã thêm ngày lễ ${day}/${month}.`, 'success');
            } else {
                // Nếu server trả về lỗi (vd: đã tồn tại, hoặc holidayValue rỗng)
                showAlert(response.message || 'Lỗi khi thêm ngày lễ!', 'danger');
            }
        },
        error: function (xhr, status, error) {
            showAlert('Lỗi kết nối khi thêm ngày lễ: ' + error, 'danger');
        }
    });
}

// ==== 11. Hàm lưu cài đặt Loại Ngày khi bấm “Lưu” (UpdateDayTypeSettings) =====
function saveDayCategories() {
    // 11.1. Thu thập checkbox NgàyThuong / HappyDay / NgayCuoiTuan
    const formData = collectFormData();

    // 11.2. Lấy mảng holiday hiện đang enable
    const enabledHolidaysArray = Array.from(enabledHolidays);

    // 11.3. Lấy DayCatId (nếu cần; nếu không dùng có thể bỏ)
    const dayCatIdVal = parseInt($('#dayCatId').val(), 10) || 0;

    // 11.4. Tạo payload JSON
    const payload = {
        CurrentDayCatId: dayCatIdVal,
        NgayThuong: formData.NgayThuong,       // ["Monday", "Tuesday", …]
        HappyDay: formData.HappyDay,         // ["Wednesday", …]
        NgayCuoiTuan: formData.NgayCuoiTuan,     // ["Saturday", …]
        Holidays: enabledHolidaysArray       // ["MM-DD", …]
    };

    // 11.5. Gửi AJAX POST lên server để update toàn bộ
    $.ajax({
        url: '/DAYCATs/UpdateDayTypeSettings',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                showAlert('Lưu cài đặt thành công!', 'success');
                // Đóng modal sau 0.5s, rồi reload trang nếu cần
                setTimeout(() => {
                    $('#editDayTypeModal').modal('hide');
                    setTimeout(() => location.reload(), 500);
                }, 500);
            } else {
                showAlert('Lỗi: ' + (response.message || 'Unknown error'), 'danger');
            }
        },
        error: function (xhr, status, error) {
            showAlert('Lỗi kết nối: ' + error, 'danger');
        }
    });
}

// ==== KHI TRANG ĐÃ LOAD XONG, bind event cho nút mở modal =================
$(document).ready(function () {
    $('#openDayTypeBtn').on('click', function () {
        openDayTypeModal();
    });
});
