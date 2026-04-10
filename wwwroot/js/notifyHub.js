
function showNotification(message) {

    const container = document.getElementById("notification-container");
    console.log(container)

    const notification = document.createElement("div");

    notification.className =
        "alert alert-warning shadow d-flex justify-content-between align-items-center";

    notification.style.minWidth = "300px";
    notification.style.marginBottom = "10px";

    notification.innerHTML = `
        <span>🔔 ${message}</span>
        <button class="btn-close"></button>
    `;

    // nút tắt
    notification.querySelector(".btn-close")
        .onclick = () => notification.remove();

    container.appendChild(notification);

    // auto hide sau 5s
    //setTimeout(() => {
    //    notification.remove();
    //}, 5000);
}
const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/notifyHub")
    .withAutomaticReconnect()
    .build();


// ===============================
// RECEIVE NOTIFICATION
// ===============================
notificationConnection.on("NotifyReceive", function (table) {
    console.log("vapd")

    showNotification(`Bàn số ${table.tableNumber} đang gọi nhân viên`);

});


notificationConnection.start()
    .then(() => console.log("SignalR Connected notify"))
    .catch(err => console.error(err));