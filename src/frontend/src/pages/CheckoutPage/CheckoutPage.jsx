import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import { basketAPI } from "../../api/client";
import "./CheckoutPage.css";

export default function CheckoutPage() {
  const [basket, setBasket] = useState(null);
  const [address, setAddress] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);

  const loadBasket = async () => {
    try {
      const data = await basketAPI.get();
      setBasket(data);
    } catch (err) {
      console.error("Ошибка загрузки корзины:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadBasket();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!address.trim()) {
      setError("Введите адрес доставки");
      return;
    }
    setError("");
    setSubmitting(true);
    try {
      await basketAPI.checkout(address);
      setSuccess(true);
    } catch (err) {
      setError(err.message || "Ошибка оформления заказа");
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <div className="checkout-loading">Загрузка...</div>;
  
  if (success) {
    return (
      <div className="container checkout-success">
        <h2>Заказ оформлен!</h2>
        <p>Заказ появится в списке через несколько секунд.</p>
        <NavLink to="/orders" className="checkout-success-link">Перейти к заказам</NavLink>
      </div>
    );
  }

  if (!basket || !basket.items?.length) {
    return (
      <div className="container checkout-empty">
        <h2>Корзина пуста</h2>
        <p>Добавьте товары перед оформлением</p>
      </div>
    );
  }

  return (
    <div className="container checkout-container">
      <h1 className="checkout-title">Оформление заказа</h1>

      <div className="checkout-layout">
        <div className="checkout-items">
          {basket.items.map((item) => (
            <div key={item.productId} className="checkout-item">
              <div className="checkout-item-image">
                <img
                  src={item.imageUrl || "https://via.placeholder.com/80"}
                  alt={item.productName}
                />
              </div>
              <div className="checkout-item-info">
                <p className="checkout-item-name">{item.productName}</p>
                <p className="checkout-item-qty">{item.quantity} шт.</p>
              </div>
              <p className="checkout-item-price">
                {item.totalPrice} ₽
              </p>
            </div>
          ))}
        </div>

        <div className="checkout-sidebar">
          <div className="checkout-summary">
            <div className="checkout-summary-row">
              <span>Товары ({basket.items?.reduce((s, i) => s + i.quantity, 0) || 0} шт.)</span>
              <span>{basket.totalAmount} ₽</span>
            </div>
            <div className="checkout-summary-row checkout-summary-total">
              <span>Итого</span>
              <span>{basket.totalAmount} ₽</span>
            </div>
          </div>

          <form onSubmit={handleSubmit} className="checkout-form">
            <label className="checkout-label">
              Адрес доставки
              <input
                type="text"
                className="checkout-input"
                placeholder="Город, улица, дом, квартира"
                value={address}
                onChange={(e) => setAddress(e.target.value)}
              />
            </label>
            {error && <p className="checkout-error">{error}</p>}
            <button
              type="submit"
              className="checkout-submit"
              disabled={submitting}
            >
              {submitting ? "Оформляем..." : "Подтвердить заказ"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}