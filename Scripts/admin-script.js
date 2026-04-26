/* admin-script.js - Fixed Save & Edit Logic */

document.addEventListener('DOMContentLoaded', function() {
    setupSidebar();
    setupDeleteAction();
    setupModalAutoFill();
    setupSaveAction();
    setupImageUpload(); 
});

let activeEditButton = null; // Biến quan trọng: Nhớ nút nào đang được Sửa

// 1. Sidebar Active
function setupSidebar() {
    const currentLocation = window.location.href;
    const menuItems = document.querySelectorAll('.sidebar-nav .nav-link');
    document.querySelectorAll('.sidebar-nav li').forEach(li => li.classList.remove('active'));
    menuItems.forEach(link => {
        if(link.href === currentLocation || (currentLocation.includes(link.getAttribute('href')) && link.getAttribute('href') !== '#')) {
            link.parentElement.classList.add('active');
        }
    });
}

// 2. Xử lý Xóa
function setupDeleteAction() {
    document.body.addEventListener('click', function(e) {
        if (e.target.closest('.btn-delete')) {
            e.preventDefault();
            const row = e.target.closest('tr');
            if (confirm('Bạn có chắc chắn muốn xóa dữ liệu này?')) {
                row.style.transition = "all 0.4s ease";
                row.style.opacity = "0";
                setTimeout(() => { row.remove(); showToast('Đã xóa thành công!', 'success'); }, 400);
            }
        }
    });
}

// 3. Xử lý Upload ảnh (Preview)
function setupImageUpload() {
    const imgInput = document.getElementById('prod-img-input');
    const previewContainer = document.getElementById('preview-container');
    const imgPreview = document.getElementById('prod-img-preview');

    if(imgInput) {
        imgInput.addEventListener('change', function(e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    imgPreview.src = e.target.result;
                    previewContainer.style.display = 'block';
                }
                reader.readAsDataURL(file);
            }
        });
    }
}
window.removeImage = function() {
    document.getElementById('prod-img-input').value = "";
    document.getElementById('prod-img-preview').src = "";
    document.getElementById('preview-container').style.display = 'none';
    const curr = document.getElementById('prod-current-img');
    if(curr) curr.value = "";
}

// 4. Auto-Fill Modal (Đổ dữ liệu từ Nút Sửa -> Form Modal)
function setupModalAutoFill() {
    const modals = document.querySelectorAll('.modal');
    modals.forEach(modalEl => {
        modalEl.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget; 
            if (!button) return;

            const action = button.getAttribute('data-action');
            const form = modalEl.querySelector('form');
            const modalTitle = modalEl.querySelector('.modal-title');

            if(form) form.reset();

            // Reset ảnh nếu có
            const previewContainer = modalEl.querySelector('#preview-container');
            const imgPreview = modalEl.querySelector('#prod-img-preview');
            if(previewContainer) previewContainer.style.display = 'none';
            if(imgPreview) imgPreview.src = "";

            if (action === 'add') {
                if(modalTitle) modalTitle.textContent = "Thêm mới";
                activeEditButton = null; // Đặt null để biết là đang thêm mới
                
                // Mở khóa input ID danh mục khi thêm mới
                const catIdInput = modalEl.querySelector('#cat-id');
                if(catIdInput) catIdInput.readOnly = false;
            } 
            else if (action === 'edit') {
                if(modalTitle) modalTitle.textContent = "Cập nhật thông tin";
                activeEditButton = button; // Lưu lại nút đang bấm

                // Khóa input ID danh mục khi sửa (thường ID không được sửa)
                const catIdInput = modalEl.querySelector('#cat-id');
                if(catIdInput) catIdInput.readOnly = true;

                // Tự động điền dữ liệu (Logic map data-xyz -> id=xyz)
                const attributes = button.attributes;
                for (let i = 0; i < attributes.length; i++) {
                    const attrName = attributes[i].name; 
                    if (attrName.startsWith('data-') && !attrName.includes('bs-')) {
                        const fieldId = attrName.substring(5); // data-cat-name -> cat-name
                        const input = modalEl.querySelector(`#${fieldId}`);
                        if (input) input.value = attributes[i].value;
                    }
                }
                
                // Xử lý riêng cho ảnh sản phẩm
                const oldImgUrl = button.getAttribute('data-prod-img');
                if (oldImgUrl && imgPreview) {
                    imgPreview.src = oldImgUrl;
                    previewContainer.style.display = 'block';
                }
            }
        });
    });
}

// 5. Nút LƯU (Quan trọng nhất)
function setupSaveAction() {
    const saveButtons = document.querySelectorAll('.btn-save-data');
    saveButtons.forEach(btn => {
        btn.addEventListener('click', function() {
            const modalEl = this.closest('.modal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            const originalText = this.innerHTML;

            this.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Đang lưu...';
            this.disabled = true;

            setTimeout(() => {
                this.innerHTML = originalText;
                this.disabled = false;
                
                const modalId = modalEl.getAttribute('id');

                // --- PHÂN LOẠI XỬ LÝ THEO TỪNG TRANG ---
                
                // A. SẢN PHẨM (admin-products.html)
                if (modalId === 'productModal') {
                    handleSaveProduct(modalEl);
                }
                // B. DANH MỤC (admin-categories.html)
                else if (modalId === 'catModal') {
                    handleSaveCategory(modalEl);
                }
                // C. CÁC TRANG KHÁC (Order, Account)
                else {
                    handleSaveGeneric(modalEl);
                }

                showToast('Lưu dữ liệu thành công!', 'success');
                modalInstance.hide();
            }, 500);
        });
    });
}

// === LOGIC LƯU SẢN PHẨM ===
function handleSaveProduct(modal) {
    const name = modal.querySelector('#prod-name').value;
    const cat = modal.querySelector('#prod-cat').value;
    const price = modal.querySelector('#prod-price').value;
    const stock = modal.querySelector('#prod-stock').value;
    
    // Xử lý ảnh
    const imgPreview = modal.querySelector('#prod-img-preview');
    let imgSrc = imgPreview.src;
    if (modal.querySelector('#preview-container').style.display === 'none' || !imgSrc) {
        imgSrc = 'https://via.placeholder.com/40'; // Ảnh mặc định
    }

    if (activeEditButton) {
        // --- CASE SỬA ---
        const row = activeEditButton.closest('tr');
        
        // 1. Cập nhật giao diện bảng
        row.querySelector('.product-img').src = imgSrc;
        row.querySelector('h6').innerText = name;
        // row.cells[1] là danh mục, cell 2 là giá, cell 3 là tồn kho (theo HTML bảng)
        row.cells[1].innerText = cat; 
        row.cells[2].innerText = Number(price).toLocaleString() + '$';
        row.cells[3].innerText = stock;

        // 2. Cập nhật lại data attribute cho nút Sửa (để lần sau mở lên đúng dữ liệu mới)
        activeEditButton.setAttribute('data-prod-name', name);
        activeEditButton.setAttribute('data-prod-cat', cat);
        activeEditButton.setAttribute('data-prod-price', price);
        activeEditButton.setAttribute('data-prod-stock', stock);
        activeEditButton.setAttribute('data-prod-img', imgSrc);
    } else {
        // --- CASE THÊM MỚI ---
        const tbody = document.querySelector('table tbody');
        const newRow = document.createElement('tr');
        newRow.innerHTML = `
            <td class="ps-3"><div class="d-flex align-items-center"><img src="${imgSrc}" class="product-img"><div><h6 class="mb-0">${name}</h6><small class="text-muted">NEW</small></div></div></td>
            <td>${cat}</td><td class="fw-bold">${Number(price).toLocaleString()}$</td><td>${stock}</td>
            <td class="text-end pe-3">
                <button class="btn btn-sm btn-light text-primary me-2" data-bs-toggle="modal" data-bs-target="#productModal" data-action="edit" 
                    data-prod-name="${name}" data-prod-cat="${cat}" data-prod-price="${price}" data-prod-stock="${stock}" data-prod-img="${imgSrc}">
                    <i class="bi bi-pencil-square"></i>
                </button>
                <button class="btn btn-sm btn-light text-danger btn-delete"><i class="bi bi-trash"></i></button>
            </td>`;
        tbody.insertBefore(newRow, tbody.firstChild);
    }
}

// === LOGIC LƯU DANH MỤC (Có ID) ===
function handleSaveCategory(modal) {
    const id = modal.querySelector('#cat-id').value;
    const name = modal.querySelector('#cat-name').value;
    const desc = modal.querySelector('#cat-desc').value;

    if (activeEditButton) {
        // --- CASE SỬA ---
        const row = activeEditButton.closest('tr');
        row.cells[0].innerText = id; // Cột ID
        row.cells[1].innerText = name; // Cột Tên
        row.cells[2].innerText = desc; // Cột Mô tả

        activeEditButton.setAttribute('data-cat-id', id);
        activeEditButton.setAttribute('data-cat-name', name);
        activeEditButton.setAttribute('data-cat-desc', desc);
    } else {
        // --- CASE THÊM MỚI ---
        const tbody = document.querySelector('table tbody');
        const newRow = document.createElement('tr');
        newRow.innerHTML = `
            <td class="ps-3 fw-bold">${id}</td>
            <td>${name}</td>
            <td>${desc}</td>
            <td class="text-end pe-3">
                <button class="btn btn-sm btn-light text-primary me-2" data-bs-toggle="modal" data-bs-target="#catModal" data-action="edit" 
                    data-cat-id="${id}" data-cat-name="${name}" data-cat-desc="${desc}">
                    <i class="bi bi-pencil-square"></i>
                </button>
                <button class="btn btn-sm btn-light text-danger btn-delete"><i class="bi bi-trash"></i></button>
            </td>`;
        tbody.insertBefore(newRow, tbody.firstChild);
    }
}

// === LOGIC LƯU CHUNG (Order, Account...) ===
function handleSaveGeneric(modal) {
    if (!activeEditButton) return; // Chỉ hỗ trợ Edit cho các bảng khác
    
    const row = activeEditButton.closest('tr');
    const modalId = modal.getAttribute('id');

    if (modalId === 'orderModal') {
        const status = modal.querySelector('#ord-status').value;
        const statusCell = row.cells[3];
        let badgeClass = 'bg-secondary'; let badgeText = 'Unknown';
        
        if (status === 'pending') { badgeClass = 'badge-pending'; badgeText = 'Chờ xử lý'; }
        else if (status === 'shipping') { badgeClass = 'badge-shipping'; badgeText = 'Đang giao'; }
        else if (status === 'delivered') { badgeClass = 'badge-delivered'; badgeText = 'Đã giao'; }
        
        statusCell.innerHTML = `<span class="badge ${badgeClass}">${badgeText}</span>`;
        activeEditButton.setAttribute('data-ord-status', status);
    }
    
    else if (modalId === 'accModal') {
        const name = modal.querySelector('#acc-name').value;
        const role = modal.querySelector('#acc-role').value;
        
        row.cells[1].innerText = name;
        let roleHtml = '<span class="badge badge-customer">Customer</span>';
        if(role === 'admin') roleHtml = '<span class="badge badge-admin">Admin</span>';
        else if(role === 'staff') roleHtml = '<span class="badge badge-staff">Staff</span>';
        
        row.cells[2].innerHTML = roleHtml;
        activeEditButton.setAttribute('data-acc-name', name);
        activeEditButton.setAttribute('data-acc-role', role);
    }
}

// 6. Toast Notification
function showToast(message, type = 'success') {
    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container';
        document.body.appendChild(toastContainer);
    }
    const bgClass = type === 'success' ? 'bg-success' : 'bg-danger';
    const toastHtml = `
        <div class="toast align-items-center text-white ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body"><i class="bi bi-check-circle-fill me-2"></i> ${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>`;
    
    const toastElement = document.createElement('div');
    toastElement.innerHTML = toastHtml;
    toastContainer.appendChild(toastElement.firstElementChild);
    const bsToast = new bootstrap.Toast(toastContainer.lastElementChild, { delay: 3000 });
    bsToast.show();
    toastContainer.lastElementChild.addEventListener('hidden.bs.toast', () => toastContainer.lastElementChild.remove());
}