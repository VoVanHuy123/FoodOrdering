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
                    `badge ${statusClass} mb-3 px-3 py-2`;

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



// ===============================
// UPDATE ORDER
// ===============================
orderConnection.on("OrderUpdated", function (order) {
    console.log("Order Updated:", order);

    const orderCard = document.getElementById(`order-${order.id}`);
    console.log("Order card:", orderCard);

    if (orderCard) {
        const badge = orderCard.querySelector(".badge");
        if (badge) {
            badge.innerText = order.status;
            badge.className = "badge";

            if (order.status === "Completed") {
                badge.classList.add("bg-success");
            } else if (order.status === "Pending") {
                badge.classList.add("bg-warning", "text-dark");
            } else {
                badge.classList.add("bg-secondary");
            }
        }

        orderCard.firstElementChild.classList.remove("border-danger", "border", "border-3");

        console.log("Is order error?", order.status);
        if (order.status === "Completed") {
            console.log("Order completed, show success toast");
            toastr.success(`Đơn hàng #${order.id} đã thanh toán thành công qua VNPAY!`, "💰 Đã nhận tiền");
        }
    }
});


// ===============================
// DELETE ORDER
// ===============================
orderConnection.on("OrderDeleted", function (orderId) {

    const el = document.getElementById(`order-${orderId}`);

    if (el) el.remove();
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
