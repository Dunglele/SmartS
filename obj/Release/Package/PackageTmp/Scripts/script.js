/* === 1. HELPER FUNCTIONS (CÁC HÀM HỖ TRỢ) === */

// Hàm định dạng tiền tệ VNĐ (cho code gọn hơn)
const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
};

// Hàm lấy giỏ hàng an toàn
function getCart() {
    try {
        return JSON.parse(localStorage.getItem('cart')) || [];
    } catch (e) {
        return [];
    }
}

function saveCart(cart) {
    localStorage.setItem('cart', JSON.stringify(cart));
}

/* === 2. CORE CART ACTIONS (THÊM, SỬA, XÓA) === */

function addToCart(id, name, price, image) {
    let cart = getCart();
    const productIndex = cart.findIndex(item => item.id === id);

    const quantityInput = document.getElementById('product-quantity');
    const quantityToAdd = quantityInput ? parseInt(quantityInput.value) : 1;

    if (productIndex > -1) {
        cart[productIndex].quantity += quantityToAdd;
    } else {
        cart.push({ id, name, price, image, quantity: quantityToAdd });
    }

    saveCart(cart);
    updateCartCount();

    // Toast thông báo (nếu có thư viện bootstrap toast)
    if (typeof showToast === 'function') {
        showToast(`${name} đã thêm vào giỏ!`);
    } else {
        alert(`${name} đã được thêm vào giỏ hàng!`); // Fallback nếu không có toast
    }
}

function removeFromCart(id) {
    let cart = getCart();
    cart = cart.filter(item => item.id !== id);
    saveCart(cart);
    refreshAllViews(); // Cập nhật lại toàn bộ giao diện
}

function updateCartItemQuantity(id, quantity) {
    let cart = getCart();
    const productIndex = cart.findIndex(item => item.id === id);

    if (productIndex > -1) {
        const newQuantity = parseInt(quantity);
        if (newQuantity > 0) {
            cart[productIndex].quantity = newQuantity;
        } else {
            // Nếu nhập số 0 hoặc âm -> Xóa luôn
            cart = cart.filter(item => item.id !== id);
        }
    }

    saveCart(cart);
    refreshAllViews();
}

// Hàm gom nhóm việc cập nhật giao diện
//function refreshAllViews() {
//    updateCartCount();
//    displayCart();
//    displayCheckoutTotal();
//}

function updateCartCount() {
    const cart = getCart();
    const totalQuantity = cart.reduce((sum, item) => sum + item.quantity, 0);

    // Badge trên header
    const cartCountElement = document.getElementById('cart-count');
    if (cartCountElement) {
        cartCountElement.textContent = totalQuantity;
        cartCountElement.style.display = totalQuantity > 0 ? 'block' : 'none';
    }

    // Badge trang thanh toán
    const checkoutBadge = document.getElementById('checkout-cart-count');
    if (checkoutBadge) checkoutBadge.innerText = totalQuantity;
}

function calculateCartTotals() {
    const cart = getCart();
    let subtotal = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const tax = subtotal * 0.1; // 10% VAT
    const total = subtotal + tax;
    return { subtotal, tax, total };
}

/* === 3. DISPLAY FUNCTIONS (HIỂN THỊ) === */

//function displayCart() {
//    const container = document.getElementById('cart-items-container');
//    const totalContainer = document.getElementById('cart-total-container');
//    const emptyMsg = document.getElementById('cart-empty-msg');

//    if (!container) return; // Nếu không ở trang giỏ hàng thì thoát

//    const cart = getCart();

//    if (cart.length === 0) {
//        container.innerHTML = '';
//        if (totalContainer) totalContainer.style.display = 'none';
//        if (emptyMsg) emptyMsg.style.display = 'block';
//        return;
//    }

//    if (totalContainer) totalContainer.style.display = 'flex';
//    if (emptyMsg) emptyMsg.style.display = 'none';

//    const totals = calculateCartTotals();

//    //container.innerHTML = cart.map(item => `
//    //    <tr>
//    //      <td>
//    //        <div class="d-flex align-items-center">
//    //          <img src="${item.image}" alt="${item.name}" class="me-3 rounded" style="width: 50px; height: 50px; object-fit: cover;">
//    //          <span class="fw-bold text-dark">${item.name}</span>
//    //        </div>
//    //      </td>
//    //      <td class="text-nowrap">${formatCurrency(item.price)}</td>
//    //      <td style="min-width: 80px;">
//    //        <input type="number" class="form-control quantity-input text-center" 
//    //          value="${item.quantity}" min="1"
//    //          onchange="updateCartItemQuantity('${item.id}', this.value)">
//    //      </td>
//    //      <td class="text-nowrap fw-bold">${formatCurrency(item.price * item.quantity)}</td>
//    //      <td>
//    //        <button class="btn btn-sm btn-outline-danger" onclick="removeFromCart('${item.id}')">
//    //          <i class="bi bi-trash"></i>
//    //        </button>
//    //      </td>
//    //    </tr>
//    //`).join('');

//    if (document.getElementById('subtotal')) document.getElementById('subtotal').textContent = formatCurrency(totals.subtotal);
//    if (document.getElementById('tax')) document.getElementById('tax').textContent = formatCurrency(totals.tax);
//    if (document.getElementById('total')) document.getElementById('total').textContent = formatCurrency(totals.total);
////}

function displayCheckoutTotal() {
    const container = document.getElementById('checkout-total-summary');
    if (!container) return; // Nếu không ở trang thanh toán thì thoát

    const cart = getCart();
    if (cart.length === 0) {
        container.innerHTML = '<div class="alert alert-warning text-center">Giỏ hàng trống.</div>';
        return;
    }

    const totals = calculateCartTotals();

    const itemsHtml = cart.map(item => `
    //    <li class="list-group-item d-flex justify-content-between lh-sm">
    //        <div>
    //            <h6 class="my-0">${item.name}</h6>
    //            <small class="text-muted">SL: ${item.quantity} x ${formatCurrency(item.price)}</small>
    //        </div>
    //        <span class="text-muted">${formatCurrency(item.price * item.quantity)}</span>
    //    </li>
    //`).join('');

    container.innerHTML = `
        //<h4 class="d-flex justify-content-between align-items-center mb-3">
        //    <span class="text-primary">Đơn hàng</span>
        //    <span class="badge bg-primary rounded-pill">${cart.length}</span>
        //</h4>
        //<ul class="list-group mb-3">
        //    ${itemsHtml}
        //    <li class="list-group-item d-flex justify-content-between">
        //        <span>Tạm tính</span>
        //        <strong>${formatCurrency(totals.subtotal)}</strong>
        //    </li>
        //    <li class="list-group-item d-flex justify-content-between">
        //        <span>VAT (10%)</span>
        //        <strong>${formatCurrency(totals.tax)}</strong>
        //    </li>
        //    <li class="list-group-item d-flex justify-content-between fw-bold bg-light">
        //        <span>Tổng cộng</span>
        //        <strong class="fs-5 text-dark">${formatCurrency(totals.total)}</strong>
        //    </li>
        //</ul>
    `;
}

/* === 4. CHECKOUT LOGIC (THANH TOÁN) === */

function processCheckout(event) {
    const cart = getCart();

    if (cart.length === 0) {
        event.preventDefault();
        alert('Giỏ hàng trống. Không thể thanh toán.');
        return;
    }

    // Chuẩn bị dữ liệu JSON
    const cartDataForServer = cart.map(item => ({
        MASP: item.id,
        TENSP: item.name,
        GIA: item.price,
        SOLUONG: item.quantity
    }));

    const jsonInput = document.getElementById('CartJson');
    if (jsonInput) {
        jsonInput.value = JSON.stringify(cartDataForServer);
        // Form sẽ tự động submit sau dòng này (vì không gọi preventDefault)
    } else {
        event.preventDefault();
        alert('Lỗi hệ thống: Không tìm thấy input chứa giỏ hàng.');
    }
}

// --- MỚI: HÀM KIỂM TRA ĐỂ XÓA GIỎ HÀNG SAU KHI THANH TOÁN ---
function checkOrderSuccess() {
    // Kiểm tra URL xem có tham số ?orderSuccess=true không
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('orderSuccess') === 'true') {
        // Xóa giỏ hàng
        localStorage.removeItem('cart');
        updateCartCount();

        // Thông báo đẹp
        alert("🎉 Đặt hàng thành công! Cảm ơn bạn đã mua sắm.");

        // (Tùy chọn) Xóa query param để F5 không hiện lại thông báo
        const newUrl = window.location.pathname;
        window.history.replaceState({}, document.title, newUrl);
    }
}

/* === 5. FILTER & UI FIXES === */

function applyProductFilters() {
    const searchInput = document.getElementById('filterSearch');
    const tagInput = document.getElementById('filterTag');
    const priceInput = document.getElementById('filterPrice');

    if (!searchInput || !tagInput || !priceInput) return;

    const searchTerm = searchInput.value.toLowerCase().trim();
    const selectedTag = tagInput.value;
    const selectedPrice = priceInput.value;
    const productList = document.querySelectorAll('.product-col');

    let minPrice = 0, maxPrice = Infinity;

    if (selectedPrice !== 'all') {
        if (selectedPrice === '1000plus') {
            minPrice = 1000;
        } else {
            const parts = selectedPrice.split('-');
            minPrice = parseInt(parts[0]) || 0;
            maxPrice = parseInt(parts[1]) || Infinity;
        }
    }

    productList.forEach(product => {
        const name = (product.dataset.name || '').toLowerCase();
        const tag = product.dataset.tag;
        const price = parseInt(product.dataset.price || 0);

        const matchesSearch = name.includes(searchTerm);
        const matchesTag = (selectedTag === 'all' || selectedTag === tag);
        const matchesPrice = (price >= minPrice && price <= maxPrice);

        product.style.display = (matchesSearch && matchesTag && matchesPrice) ? 'block' : 'none';
    });
}

/* === 6. EVENT LISTENERS (INIT) === */

document.addEventListener('DOMContentLoaded', () => {
    // 1. Khởi tạo dữ liệu
    refreshAllViews();

    // 2. Kiểm tra nếu vừa đặt hàng xong thì xóa giỏ
    checkOrderSuccess();

    // 3. Xử lý Input tìm kiếm (Mobile Fix)
    const searchInput = document.getElementById('filterSearch');
    if (searchInput) {
        // Fix lỗi readonly trên mobile
        const removeReadonly = () => {
            if (searchInput.hasAttribute('readonly')) searchInput.removeAttribute('readonly');
        };
        ['touchstart', 'click', 'focus'].forEach(evt =>
            searchInput.addEventListener(evt, removeReadonly)
        );

        // Lắng nghe sự kiện nhập liệu
        searchInput.addEventListener('input', applyProductFilters);
    }

    // 4. Lắng nghe bộ lọc
    const tagInput = document.getElementById('filterTag');
    const priceInput = document.getElementById('filterPrice');
    if (tagInput) tagInput.addEventListener('change', applyProductFilters);
    if (priceInput) priceInput.addEventListener('change', applyProductFilters);
});