import { Link, NavLink } from 'react-router-dom';
import logo from '../../assets/react.svg';
import cartIcon from '../../assets/shopping-cart.svg';
import { useAuth } from '../../context/AuthContext';
import "./Header.css";

export default function Header() {
    const { user, logout } = useAuth();

    return (
        <header className="header">
            <div className="container header-container">
                <div className="header-item">
                    <Link to="/" className="header-item-logo">
                        <img src={logo} alt="Shop" className="header-logo" />
                        <span className="header-logo-text">Shop</span>
                    </Link>
                </div>
                <nav className="header-item" aria-label="Главное меню">
                    <NavLink to="/" end className="header-link">Главная</NavLink>
                    <NavLink to="/catalog" className="header-link">Каталог</NavLink>
                    <NavLink to="/about" className="header-link">О нас</NavLink>
                </nav>
                <div className="header-item">
                    {user ? (
                        <button onClick={logout} className="header-item-button">Выйти</button>
                    ) : (
                        <Link to="/login" className="header-link">Войти</Link>
                    )}
                    {user && <Link to="/orders" className="header-link">Заказы</Link>}
                    <NavLink to="/basket" className="header-item-basket">
                        <img src={cartIcon} alt="Корзина" />
                        <span className="header-link-text">Корзина</span>
                    </NavLink>
                </div>
            </div>
        </header>
    );
}