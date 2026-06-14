import { useEffect, useState } from "react";
import { useParams, NavLink } from "react-router-dom";
import { orderingAPI } from "../../api/client";
import "./OrderDetailPage.css";

export default function OrderDetailPage() {
    const { id } = useParams();
    const [order, setOrder] = useState(null);
    const [loading, setLoading] = useState(true);
    
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
  
    const loadOrder = async () => {
      try {
        const data = await orderingAPI.getById(id);
        setOrder(data);
      } catch (err) {
        console.error("Ошибка загрузки заказа:", err);
      } finally {
        setLoading(false);
      }
    };
  
    useEffect(() => {
      loadOrder();
    }, [id]);
  
    if (loading) return <div className="order-detail-loading">Загрузка...</div>;
    if (!order) return <div className="order-detail-loading">Заказ не найден</div>;

    return (<>
        <nav className="product-nav" aria-label="Хлебные крошки">
            <div className="container">
                <ul className="product-list">
                    <li className="product-item">
                        <NavLink to="/" className="product-link">Главная</NavLink>
                    </li>
                    <li className="product-item">
                        <NavLink to="/orders" className="product-link">Заказы</NavLink>
                    </li>
                    <li className="product-item">
                        <span className="product-current">Заказ №{order.id || id}</span>
                    </li>
                </ul>
            </div>
        </nav>

        <div className="container">
          <div className="order-detail-header">
            <h1 className="order-detail-title">Заказ №{order.id || id}</h1>
            <span
              className="order-status"
              style={{ background: statusColor[order.status] }}
            >
              {statusMap[order.status] || order.status}
            </span>
          </div>

          <div className="order-detail-info">
            <p><strong>Адрес доставки:</strong> {order.shippingAddress}</p>
            <p><strong>Сумма заказа:</strong> {order.totalAmount} ₽</p>
          </div>

          <div className="order-detail-items">
            <h2 className="order-detail-subtitle">Товары</h2>
            {order.items?.map((item) => (
              <div key={item.id} className="order-detail-item">
                <div className="order-detail-item-image">
                  <img
                    src={item.imageUrl || "https://via.placeholder.com/80"}
                    alt={item.productName}
                  />
                </div>
                <div className="order-detail-item-info">
                  <p className="order-detail-item-name">{item.productName}</p>
                  <p className="order-detail-item-qty">{item.quantity} шт.</p>
                </div>
                <p className="order-detail-item-price">
                  {item.totalPrice} {item.priceCurrency}
                </p>
              </div>
            ))}
          </div>

          <NavLink to="/orders" className="order-detail-back">
            ← Назад к заказам
          </NavLink>
        </div>
    </>);
}