import { useEffect, useState } from "react";
import { orderingAPI } from "../../api/client";
import { NavLink } from "react-router-dom";
import "./OrdersPage.css";

export default function OrdersPage() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);

  const statusMap = {
    Pending: "Проверка склада",
    Confirmed: "Подтверждён",
    Paid: "Оплачен",
    Shipped: "В пути",
    Delivered: "Доставлен",
    Cancelled: "Отменён",
  };

  const statusColor = {
      Pending: "#f59e0b",
      Confirmed: "#6366f1",
      Paid: "#3b82f6",
      Shipped: "#8b5cf6",
      Delivered: "#10b981",
      Cancelled: "#ef4444",
  };

  const loadOrders = async () => {
    try {
      const data = await orderingAPI.getAll();
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

  if (loading) return <div className="orders-loading">Загрузка...</div>;

  if (orders.length === 0) {
    return (
      <div className="container orders-empty">
        <h2>У вас пока нет заказов</h2>
        <NavLink to="/catalog" className="orders-empty-link">Перейти в каталог</NavLink>
      </div>
    );
  }

  return (
    <div className="container orders-container">
      <h1 className="orders-title">Мои заказы</h1>
      <div className="orders-list">
        {orders.map((order) => (
          <div key={order.id} className="order-card">
            <div className="order-header">
              <div>
                <span className="order-id">Заказ №{order.id}</span>
                <span
                  className="order-status"
                  style={{ background: statusColor[order.status] }}
                >
                  {statusMap[order.status] || order.status}
                </span>
              </div>
              <span className="order-total">
                {order.totalAmount} ₽
              </span>
            </div>
            <div className="order-body">
              <span>Товаров: {order.itemsCount}</span>
              <NavLink to={`/orders/${order.id}`} className="order-details-link">
                Подробнее →
              </NavLink>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}