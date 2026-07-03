import { useEffect, useState } from "react";
import { catalogAPI } from "../../api/client";
import "./AdminProducts.css";

export default function AdminProducts() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState({ name: "", price: "" });
  const [editId, setEditId] = useState(null);
  const [message, setMessage] = useState("");
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 50;
  const totalPages = Math.ceil(totalCount / pageSize);

  const loadProducts = async () => {
    try {
      const data = await catalogAPI.getAll(page, pageSize);
      setProducts(data.products || []);
      setTotalCount(data.totalCount);
    } catch (err) {
      console.error("Ошибка загрузки:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProducts();
  }, [page]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage("");
    try {
      if (editId) {
        await catalogAPI.update(editId, form.name, +form.price);
        setMessage("Товар обновлён");
      } else {
        await catalogAPI.create(form.name, +form.price);
        setMessage("Товар добавлен");
      }
      setForm({ name: "", price: "" });
      setEditId(null);
      loadProducts();
    } catch (err) {
      setMessage("Ошибка: " + err.message);
    }
  };

  const handleEdit = (p) => {
    setEditId(p.id);
    setForm({ name: p.name, price: p.price.toString() });
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
      <h2 className="admin-section-title">Товары ({totalCount})</h2>

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
          value={form.price}
          onChange={(e) => setForm({ ...form, price: e.target.value })}
          required
        />
        <button type="submit" className="admin-btn">
          {editId ? "Обновить" : "Добавить товар"}
        </button>
        {editId && (
          <button type="button" className="admin-btn admin-btn-cancel" onClick={() => { setEditId(null); setForm({ name: "", price: "" }); }}>
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
            <span className="admin-product-price">{p.price} ₽</span>
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

      {totalPages > 1 && (
        <div className="pagination">
          <button onClick={() => setPage(p => p - 1)} disabled={page === 1}>← Назад</button>
          <span>{page} / {totalPages}</span>
          <button onClick={() => setPage(p => p + 1)} disabled={page >= totalPages}>Вперёд →</button>
        </div>
      )}
    </div>
  );
}