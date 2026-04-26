$('#orderTable').DataTable({
    // Cấu hình DOM quan trọng: Dùng d-flex justify-content-between để đẩy sang 2 cực
    "dom": '<"d-flex justify-content-between align-items-center mb-2"lf>' + 
           'rt' + 
           '<"d-flex justify-content-between align-items-center mt-3"ip>',
    "language": {
        "lengthMenu": "Hiển thị _MENU_", // Bỏ chữ 'dòng' cho gọn
        "search": "", // Ẩn chữ 'Tìm kiếm:', chỉ hiện ô input
        "searchPlaceholder": "Tìm kiếm đơn hàng...",
        "zeroRecords": "Không tìm thấy dữ liệu",
        "info": "Hiển thị _START_ - _END_ trong _TOTAL_ đơn",
        "paginate": { "first": "«", "last": "»", "next": "›", "previous": "‹" }
    },
    "pageLength": 10,
    "ordering": false,
    "autoWidth": false,
    "responsive": true
});