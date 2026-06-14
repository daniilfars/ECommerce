import { useEffect, useState } from "react";
import { orderingAPI } from "../../api/client";
import "./AdminOrders.css";

export default function AdminOrders() {
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [expandedId, setExpandedId] = useState(null);

    const statusMap = {
      Pending: "Ожидает оплаты",
      Paid: "Оплачен",
      Shipped: "В пути",
      Delivered: "Доставлен",
      Cancelled: "Отменён",
    };

    const statusColor = {
      Pending: "#f59e0b",
      Paid: "#3b82f6",
      Shipped: "#8b5cf6",
      Delivered: "#10b981",
      Cancelled: "#ef4444",
    };

    const loadOrders = async () => {
      try {
        const data = await orderingAPI.getAllOrders(1, 50);
        setOrders(data.orders || []);
      } catch (err) {
        console.error("Ошибка загрузки заказов:", err);
      } finally {
        setLoading(false);
      }
    };

    useEffect(() => {
      loadOrders();
    }, []);

    const handleAction = async (orderId, action) => {
      try {
        if (action === "pay") await orderingAPI.pay(orderId);
        if (action === "ship") await orderingAPI.ship(orderId);
        if (action === "deliver") await orderingAPI.deliver(orderId);
        if (action === "cancel") await orderingAPI.cancel(orderId);
        loadOrders();
      } catch (err) {
        console.error(`Ошибка ${action}:`, err);
      }
    };

    const toggleExpand = async (orderId) => {
      if (expandedId === orderId) {
        setExpandedId(null);
        return;
      }
      setExpandedId(orderId);
    };

    if (loading) return <div className="admin-loading">Загрузка...</div>;

    return (
        <div>
          <h2 className="admin-section-title">Заказы ({orders.length})</h2>

          <div className="admin-orders-table">
            {orders.map((order) => (
              <div key={order.id} className="admin-order-card">
                <div
                  className="admin-order-header"
                  onClick={() => toggleExpand(order.id)}
                >
                  <div className="admin-order-header-left">
                    <span className="admin-order-id">№{order.id}</span>
                    <span
                      className="order-status"
                      style={{ background: statusColor[order.status] }}
                    >
                      {statusMap[order.status] || order.status}
                    </span>
                  </div>
                  <div className="admin-order-header-right">
                    <span className="admin-order-total">
                      {order.totalAmount} ₽
                    </span>
                    <span className="admin-order-expand">
                      {expandedId === order.id ? "▲" : "▼"}
                    </span>
                  </div>
                </div>

                {expandedId === order.id && (
                  <div className="admin-order-details">
                    <div className="admin-order-meta">
                      <p><strong>Адрес:</strong> {order.shippingAddress}</p>
                      <p><strong>Товаров:</strong> {order.itemsCount}</p>
                    </div>

                    <div className="admin-order-actions">
                      {order.status === "Pending" && (
                        <>
                          <button
                            className="admin-btn admin-btn-sm"
                            onClick={() => handleAction(order.id, "pay")}
                          >
                            💰 Оплатить
                          </button>
                          <button
                            className="admin-btn admin-btn-sm admin-btn-cancel"
                            onClick={() => handleAction(order.id, "cancel")}
                          >
                            ❌ Отменить
                          </button>
                        </>
                      )}
                      {order.status === "Paid" && (
                        <button
                          className="admin-btn admin-btn-sm"
                          onClick={() => handleAction(order.id, "ship")}
                        >
                          🚚 Отправить
                        </button>
                      )}
                      {order.status === "Shipped" && (
                        <button
                          className="admin-btn admin-btn-sm"
                          onClick={() => handleAction(order.id, "deliver")}
                        >
                          ✅ Доставить
                        </button>
                      )}
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
  );
}