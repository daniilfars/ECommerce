import { Link, NavLink } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import logo from "../../assets/react.svg";
import cartIcon from "../../assets/shopping-cart.svg";
import "./Header.css";

export default function Header() {
    const {user, logout} = useAuth();

    return (
        <header className="header">
            <div className="container header-container">
                <div className="header-item">
                    <Link to="/" className="header-item-logo">
                        <img src={logo} alt="Shop" className="header-logo" />
                        <p className="header-logo-p">Shop</p>
                    </Link>
                </div>
                <div className="header-item">
                    <NavLink to="/" end>
                        <p className="header-p">Главная</p>
                    </NavLink>
                    <NavLink to="/catalog">
                        <p className="header-p">Каталог</p>
                    </NavLink>
                    <NavLink to="/about">
                        <p className="header-p">О нас</p>
                    </NavLink>
                </div>
                <div className="header-item">
                    {user ? (<button onClick={logout} className="header-item-button">Выйти</button>) : (<Link to="/login" className="header-p">Войти</Link>)}
                    {user && <Link to="/orders">Заказы</Link>}
                    <NavLink to="/basket" className="header-item-basket">
                        <img src={cartIcon} alt="Корзина" />
                        <p className="header-p">Корзина</p>
                    </NavLink>
                </div>
            </div>
        </header>
    )
}