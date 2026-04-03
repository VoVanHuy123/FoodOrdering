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

        console.log("New Order:", order);

        const list = document.getElementById("orders");
        console.log("Orders List Element:", list);
        if (!list) return;

        const statusClass =
            order.status === "Completed"
                ? "bg-success"
                : order.status === "Pending"
                    ? "bg-warning text-dark"
                    : "bg-secondary";

        const html = `
        <div class="col-md-6 col-lg-4 mb-4" id="order-${order.id}">
            <div class="card shadow-sm border-0 h-100">

                <div class="card-body">

                    <div class="d-flex justify-content-between">
                        <h5 class="fw-bold">
                            🍽 Table ${order.tableId}
                        </h5>

                        <span class="badge ${statusClass}">
                            ${order.status}
                        </span>
                    </div>

                    <hr/>

                    <p>
                        <strong>Time:</strong><br/>
                        ${new Date(order.orderTime)
                .toLocaleString()}
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

        // ⭐ thêm order mới lên đầu list
        list.insertAdjacentHTML("afterbegin", html);


    });


// ===============================
// UPDATE ORDER
// ===============================
orderConnection.on("OrderUpdated", function (order) {
    console.log("Order Updated:", order);

    const orderCard = document.getElementById(`order-${order.id}`);

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

        if (order.status === "Completed") {
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
