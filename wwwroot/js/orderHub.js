// ===============================
// SIGNALR ORDER HUB
// ===============================

// connect hub
const orderConnection = new signalR.HubConnectionBuilder()
    .withUrl("/orderHub")
    .withAutomaticReconnect()
    .build();



// ===============================
// RECEIVE NEW ORDER
// ===============================

orderConnection.on("ReceiveNewOrder", function (order) {

    const list = document.getElementById("orders");
    if (!list) return;

    const statusClass =
        order.status === "Completed"
            ? "bg-success"
            : order.status === "Pending"
                ? "bg-warning text-dark"
                : "bg-secondary";

    // ✅ tạo badge bằng JS
    const errorBadge = order.isError
        ? `
            <div class="mt-2">
                <span class="badge bg-danger">
                    ⚠ Has item out of stock
                </span>
            </div>
          `
        : "";

    const cardErrorClass = order.isError
        ? "border-warning bg-warning-subtle"
        : "";

    const html = `
    <div class="col-md-6 col-lg-4 mb-4" id="order-${order.id}">
        <div class="card shadow-sm border-0 h-100 ${cardErrorClass}">

            <div class="card-body">

                <div class="d-flex justify-content-between">
                    <h5 class="fw-bold">
                        🍽 Table ${order.tableId}
                    </h5>

                    <span class="badge ${statusClass}">
                        ${order.status}
                    </span>
                </div>

                ${errorBadge}

                <hr/>

                <p>
                    <strong>Time:</strong><br/>
                    ${new Date(order.orderTime).toLocaleString()}
                </p>

                <p>
                    <strong>Total:</strong><br/>
                    <span class="text-danger fw-bold fs-5">
                        ${order.totalAmount.toLocaleString()} VNĐ
                    </span>
                </p>

            </div>
            <div class="card-footer bg-white border-0">
                    <div class="d-flex justify-content-between">

                        <a href="/Orders/Details/${order.id}"
                           class="btn btn-outline-primary btn-sm">
                            Details
                        </a>

                        <a href="/Orders/Edit/${order.id}"
                           class="btn btn-outline-warning btn-sm">
                            Edit
                        </a>

                        <a href="/Orders/Delete/${order.id}"
                           class="btn btn-outline-danger btn-sm">
                            Delete
                        </a>

                    </div>
                </div>

        </div>
    </div>
    `;

    list.insertAdjacentHTML("afterbegin", html);
});
// ===============================
// UPDATE MenuItemNotAvailable
// ===============================
orderConnection.on("MenuItemNotAvailable", function (orderIds) {

    console.log("Menu Item Not Available:", orderIds);

    if (window.location.pathname === "/Orders") {
        location.reload();
    }
    
});



orderConnection.on("TableOccupied",
    function (table) {
        console.log("Table Occupied:",table)
        

        // =========================
        // STATUS COLOR
        // =========================
        let statusClass = "bg-secondary";

        if (table.status === "Available")
            statusClass = "bg-success";

        if (table.status === "Occupied")
            statusClass = "bg-danger";

        if (table.status === "Reserved")
            statusClass = "bg-warning text-dark";
        console.log(window.location.pathname.toLowerCase());


        // =================================================
        // 1️⃣ IF USER IS IN /Tables (INDEX PAGE)
        // =================================================
        if (window.location.pathname.toLowerCase() === "/tables") {
            console.log("User is in /Tables page");
            console.log(`table_${table.tableNumber}`);
            const badge = document.getElementById(`table_${table.tableNumber}`);
            console.log("Badge element:", badge);

            if (badge) {
                badge.className =
                    table.status === "Available" ? "admin-badge admin-badge--completed" :
                    table.status === "Occupied" ? "admin-badge admin-badge--error" :
                    table.status === "Reserved" ? "admin-badge admin-badge--pending" :
                    "admin-badge admin-badge--cancelled";

                badge.innerText = table.status;
            }
        }


        // =================================================
        // 2️⃣ IF USER IS IN DETAILS PAGE
        // /Tables/Details/{id}
        // =================================================
        if (window.location.pathname
            .toLowerCase()
            .includes(`/tables/details/${table.id}`)) {

            const headerStatus =
                document.getElementById("table-status");

            const bodyStatus =
                document.getElementById("table-status-body");

            if (headerStatus) {
                headerStatus.className =
                    `badge ${statusClass} fs-6 px-3 py-2`;

                headerStatus.innerText = table.status;
            }

            if (bodyStatus) {
                bodyStatus.className =
                    `badge ${statusClass}`;

                bodyStatus.innerText = table.status;
            }
        }


        // =================================================
        // 3️⃣ IF USER IS IN EDIT PAGE
        // /Tables/Edit/{id}
        // =================================================
        if (window.location.pathname
            .toLowerCase()
            .includes(`/tables/edit/${table.id}`)) {

            const select =
                document.getElementById("table-status-select");

            if (select) {
                select.value = table.status;
            }
        }

    });



function escapeHtml(unsafe) {
    if (!unsafe) return "";
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/\'/g, "&#039;");
}

function findOrderRow(orderId) {
    let row = document.getElementById(`order-row-${orderId}`);
    if (!row) {
        row = document.querySelector(`[data-order-id="${orderId}"]`);
    }
    if (!row) {
        row = document.querySelector(`tr[data-order-id='${orderId}']`);
    }
    return row;
}

function formatOrderTime(value) {
    if (!value) return "";
    const date = new Date(value);
    return new Intl.DateTimeFormat('en-GB', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    }).format(date);
}

function getOrderStatusBadgeClass(status) {
    switch (status) {
        case 'Completed':
            return 'admin-badge admin-badge--completed';
        case 'Pending':
            return 'admin-badge admin-badge--pending';
        case 'Preparing':
            return 'admin-badge admin-badge--preparing';
        case 'Cancelled':
            return 'admin-badge admin-badge--cancelled';
        default:
            return 'admin-badge admin-badge--cancelled';
    }
}


// ===============================
// UPDATE ORDER
// ===============================
orderConnection.on("OrderUpdated", function (order) {
    console.log("Order Updated:", order);

    const orderId = order.id ?? order.Id;
    const orderStatus = order.status ?? order.Status;

    const orderCard = document.getElementById(`order-${orderId}`);
    const orderRow = findOrderRow(orderId);
    console.log("Order card:", orderCard);
    console.log("Order row:", orderRow);

    if (orderRow) {
        const statusBadge = orderRow.querySelector('.order-status span');
        if (statusBadge) {
            statusBadge.innerText = orderStatus;
            statusBadge.className = getOrderStatusBadgeClass(orderStatus);
        }

        const tableNumber = orderRow.querySelector('.order-table-number .fw-medium');
        if (tableNumber && order.tableNumber !== undefined) {
            tableNumber.innerText = order.tableNumber;
        }

        const noteCell = orderRow.querySelector('.order-note');
        if (noteCell) {
            if (order.note) {
                noteCell.innerHTML = `
                    <div class="small text-muted text-truncate" style="max-width: 14rem; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;" title="${escapeHtml(order.note)}">
                        ${escapeHtml(order.note)}
                    </div>
                `;
            } else {
                noteCell.innerHTML = '<span class="text-muted">—</span>';
            }
        }

        const timeCell = orderRow.querySelector('.order-time .admin-table__mono');
        if (timeCell && order.orderTime) {
            timeCell.innerText = formatOrderTime(order.orderTime);
        }

        const totalCell = orderRow.querySelector('.order-total .admin-table__amount');
        if (totalCell && order.totalAmount !== undefined) {
            totalCell.innerText = `${Number(order.totalAmount).toLocaleString()} đ`;
        }

        orderRow.classList.toggle('admin-row-alert', !!order.isError);

        console.log('Updated order row for', order.id);
    }

    if (orderCard) {
        const badge = orderCard.querySelector('.badge');
        if (badge) {
            badge.innerText = order.status;
            badge.className = getOrderStatusBadgeClass(order.status);
        }

        if (orderCard.firstElementChild) {
            orderCard.firstElementChild.classList.remove('border-danger', 'border', 'border-3');
        }

        if (order.status === 'Completed') {
            toastr.success(`Đơn hàng #${order.id} đã thanh toán thành công qua VNPAY!`, '💰 Đã nhận tiền');
        }
    }

    if (!orderRow && !orderCard && window.location.pathname.toLowerCase().startsWith('/orders')) {
        location.reload();
    }
});


// ===============================
// DELETE ORDER
// ===============================
orderConnection.on("OrderDeleted", function (orderId) {

    const card = document.getElementById(`order-${orderId}`);
    const row = document.getElementById(`order-row-${orderId}`);

    if (card) card.remove();
    if (row) row.remove();
});

// ===============================
// START CONNECTION
// ===============================
orderConnection.start()
    .then(() => console.log("✅ SignalR Connected"))
    .catch(err => console.error(err));

//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("/orderHub")
//    .withAutomaticReconnect()
//    .build();


//// REGISTER EVENT FIRST
//connection.on("ReceiveNewOrder", order => {

//    console.log("🔥 NEW ORDER RECEIVED", order);

//    const list = document.getElementById("orders");
//    if (!list) return;

//    const html = `
//        <div class="col-md-4" id="order-${order.id}">
//            <div class="card p-3">
//                <b>Table ${order.tableId}</b><br/>
//                Status: ${order.status}<br/>
//                Total: ${order.totalAmount.toLocaleString()} VNĐ
//            </div>
//        </div>
//    `;

//    list.insertAdjacentHTML("afterbegin", html);
//});


//// START CONNECTION
//async function start() {
//    try {
//        await connection.start();
//        console.log("✅ Admin connected SignalR");
//    } catch (err) {
//        console.error(err);
//        setTimeout(start, 3000);
//    }
//}

//start();

orderConnection.on("ReceiveCashAlert", function (data) {
    toastr.info(
        `Bàn số: ${data.tableId || 'Không rõ'}`,
        `🔔 ${data.message}`,
        {
            timeOut: 0,
            extendedTimeOut: 0
        }
    );

    if (data.orderId) {
        const orderCard = document.getElementById(`order-${data.orderId}`);
        if (orderCard) {
            orderCard.firstElementChild.classList.remove("border-0");
            orderCard.firstElementChild.classList.add("border", "border-danger", "border-3");
        }
    }
});
