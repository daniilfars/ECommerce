import { NavLink, Outlet, useLocation } from "react-router-dom";
import "./AdminPage.css";

export default function AdminPage() {
  const { pathname } = useLocation();

  return (
    <div className="container admin-container">
      <h1 className="admin-title">Админ-панель</h1>
      <div className="admin-layout">
        <nav className="admin-sidebar">
          <NavLink
            to="/admin/products"
            className={`admin-link ${pathname.includes("/admin/products") ? "admin-link-active" : ""}`}
          >
            📦 Товары
          </NavLink>
          <NavLink
            to="/admin/orders"
            className={`admin-link ${pathname.includes("/admin/orders") ? "admin-link-active" : ""}`}
          >
            📋 Заказы
          </NavLink>
        </nav>
        <div className="admin-content">
          <Outlet />
        </div>
      </div>
    </div>
  );
}