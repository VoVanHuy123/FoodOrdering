// ===============================
// SIGNALR ORDER HUB
// ===============================

// connect hub
const orderConnection = new signalR.HubConnectionBuilder()
    .withUrl("/orderHub")
    .withAutomaticReconnect()
    .build();

// ===============================
// START CONNECTION
// ===============================
orderConnection.start()
    .then(() => console.log("✅ SignalR Connected"))
    .catch(err => console.error(err));


// ===============================
// RECEIVE NEW ORDER
// ===============================
orderConnection.on("ReceiveNewOrder", function (order) {

    console.log("New Order:", order);

    const list = document.getElementById("orders");
    if (!list) return;

    list.innerHTML += `
        <div id="order-${order.id}">
            Order #${order.id} - Table ${order.tableId}
            - ${order.status}
        </div>
    `;
});


// ===============================
// UPDATE ORDER
// ===============================
orderConnection.on("OrderUpdated", function (order) {

    const el = document.getElementById(`order-${order.id}`);

    if (el) {
        el.innerHTML =
            `Order #${order.id} - ${order.status}`;
    }
});


// ===============================
// DELETE ORDER
// ===============================
orderConnection.on("OrderDeleted", function (orderId) {

    const el = document.getElementById(`order-${orderId}`);

    if (el) el.remove();
});