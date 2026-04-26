function printOrder(orderId) {
    // 1. Xác định modal cụ thể dựa trên orderId được truyền vào
    var modalId = 'popup-' + orderId;
    var modalElement = document.getElementById(modalId);

    if (!modalElement) {
        alert("Không tìm thấy dữ liệu đơn hàng!");
        return;
    }

    // Lấy nội dung bên trong class 'modal-body-custom' của modal đó
    var contentElement = modalElement.querySelector('.modal-body-custom');
    var customerInfo = contentElement.innerHTML;

    // 2. Tạo nội dung Header cho bản in
    var printHeader = `
            <div class="text-center mb-4">
                <h2 class="fw-bold">HÓA ĐƠN BÁN HÀNG #` + orderId + `</h2>
                <p class="mb-0">Cửa hàng SmartS - Thiết bị công nghệ</p>
                <small class="text-muted">Ngày in: ${new Date().toLocaleString('vi-VN')}</small>
            </div>
            <hr class="my-4">
        `;

    // 3. Tạo cửa sổ in mới
    var printWindow = window.open('', '', 'height=800,width=800');

    printWindow.document.write('<html><head><title>In Hóa Đơn - SmartS</title>');

    // Nhúng Bootstrap
    printWindow.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');

    // CSS tùy chỉnh cho bản in để giữ giao diện đẹp
    printWindow.document.write(`
            <style>
                body { font-family: 'Segoe UI', sans-serif; padding: 40px; }
                .item-img { width: 50px; height: 50px; object-fit: cover; margin-right: 10px; border: 1px solid #ddd; }
                .item-row { display: flex; align-items: center; border-bottom: 1px dashed #eee; padding: 10px 0; }
                .info-group { background-color: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px; border: 1px solid #eee; }
                .info-label { font-size: 0.8rem; color: #6c757d; text-transform: uppercase; font-weight: 700; }
                .fw-bold { font-weight: 700 !important; }
                /* Fix lỗi layout Bootstrap khi in (cột col-md-6) */
                .row { display: flex; flex-wrap: wrap; }
                .col-md-6 { width: 50%; float: left; padding: 0 10px; }
            </style>
        `);

    printWindow.document.write('</head><body>');

    // 4. Ghi nội dung
    printWindow.document.write(printHeader);
    printWindow.document.write(customerInfo);

    // Phần chữ ký
    printWindow.document.write(`
            <div class="row mt-5">
                <div class="col-6 text-center" style="width: 50%; float: left;">
                    
                </div>
                <div class="col-6 text-center" style="width: 50%; float: left;">
                    <p class="fw-bold">Khách hàng</p>
                    <br><br><br>
                    <p>(Ký, họ tên)</p>
                </div>
            </div>
        `);

    printWindow.document.write('</body></html>');
    printWindow.document.close();

    // 5. Đợi tải xong style rồi mới in
    // setTimeout dùng để đảm bảo CSS load xong mới bật hộp thoại in
    setTimeout(function () {
        printWindow.focus();
        printWindow.print();
        printWindow.close();
    }, 500);
}