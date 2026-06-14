import { useEffect, useState } from "react";
import { catalogAPI } from "../../api/client";
import "./AdminProducts.css";

export default function AdminProducts() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState({ name: "", priceAmount: "", priceCurrency: "RUB" });
  const [editId, setEditId] = useState(null);
  const [message, setMessage] = useState("");

  const loadProducts = async () => {
    try {
      const data = await catalogAPI.getAll(1, 50);
      setProducts(data.products || []);
    } catch (err) {
      console.error("Ошибка загрузки:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProducts();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage("");
    try {
      if (editId) {
        await catalogAPI.update(editId, form.name, +form.priceAmount, form.priceCurrency);
        setMessage("Товар обновлён");
      } else {
        await catalogAPI.create(form.name, +form.priceAmount, form.priceCurrency);
        setMessage("Товар добавлен");
      }
      setForm({ name: "", priceAmount: "", priceCurrency: "RUB" });
      setEditId(null);
      loadProducts();
    } catch (err) {
      setMessage("Ошибка: " + err.message);
    }
  };

  const handleEdit = (p) => {
    setEditId(p.id);
    setForm({ name: p.name, priceAmount: p.priceAmount, priceCurrency: p.priceCurrency });
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Удалить товар?")) return;
    await catalogAPI.delete(id);
    loadProducts();
  };

  const handleImageUpload = async (id, file) => {
    await catalogAPI.uploadImage(id, file);
    loadProducts();
  };

  if (loading) return <div className="admin-loading">Загрузка...</div>;

  return (
    <div>
      <h2 className="admin-section-title">Товары ({products.length})</h2>

      <form className="admin-form" onSubmit={handleSubmit}>
        <input
          className="admin-input"
          placeholder="Название"
          value={form.name}
          onChange={(e) => setForm({ ...form, name: e.target.value })}
          required
        />
        <input
          className="admin-input"
          type="number"
          step="0.01"
          placeholder="Цена"
          value={form.priceAmount}
          onChange={(e) => setForm({ ...form, priceAmount: e.target.value })}
          required
        />
        <input
          className="admin-input"
          placeholder="Валюта"
          value={form.priceCurrency}
          onChange={(e) => setForm({ ...form, priceCurrency: e.target.value })}
          required
        />
        <button type="submit" className="admin-btn">
          {editId ? "Обновить" : "Добавить товар"}
        </button>
        {editId && (
          <button type="button" className="admin-btn admin-btn-cancel" onClick={() => { setEditId(null); setForm({ name: "", priceAmount: "", priceCurrency: "RUB" }); }}>
            Отмена
          </button>
        )}
      </form>

      {message && <p className="admin-message">{message}</p>}

      <div className="admin-products-table">
        {products.map((p) => (
          <div key={p.id} className="admin-product-row">
            <img src={p.imageUrl || "https://via.placeholder.com/40"} alt="" className="admin-product-img" />
            <span className="admin-product-name">{p.name}</span>
            <span className="admin-product-price">{p.priceAmount} {p.priceCurrency}</span>
            <div className="admin-product-actions">
              <label className="admin-file-label">
                📷
                <input type="file" hidden onChange={(e) => e.target.files[0] && handleImageUpload(p.id, e.target.files[0])} />
              </label>
              <button className="admin-btn-small" onClick={() => handleEdit(p)}>✏️</button>
              <button className="admin-btn-small admin-btn-danger" onClick={() => handleDelete(p.id)}>🗑️</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}