import { Link, NavLink } from 'react-router-dom';
import './Footer.css';
import truck from '../../assets/truck2.svg';
import check from '../../assets/check.svg';
import refresh from '../../assets/refresh-cw.svg';
import headphones from '../../assets/headphones.svg';
import logo from '../../assets/react.svg';

export default function Footer() {
    return (
        <footer className="footer">
            <div className="container footer-info">
                <ul className="footer-list">
                    <li className="footer-list-item">
                        <div className="footer-image-container">
                            <img src={truck} alt="Доставка" className="footer-image" />
                            </div>
                        <h3 className="footer-h3">Бесплатная доставка</h3>
                        <p className="footer-p">от 3 000 р.</p>
                    </li>
                    <li className="footer-list-item">
                        <div className="footer-image-container">
                            <img src={check} alt="Гарантия" className="footer-image" />
                        </div>
                        <h3 className="footer-h3">Гарантия качества</h3>
                        <p className="footer-p">Только оригиналы</p>
                    </li>
                    <li className="footer-list-item">
                        <div className="footer-image-container">
                            <img src={refresh} alt="Возврат" className="footer-image" />
                        </div>
                        <h3 className="footer-h3">Возврат 30 дней</h3>
                        <p className="footer-p">Без вопросов</p>
                    </li>
                    <li className="footer-list-item">
                        <div className="footer-image-container">
                            <img src={headphones} alt="Поддержка" className="footer-image" />
                        </div>
                        <h3 className="footer-h3">Поддержка 24/7</h3>
                        <p className="footer-p">Всегда на связи</p>
                    </li>
                </ul>
            </div>

            <div className="footer-bottom">
                <div className="container footer-bottom-container">
                    <div className="footer-brand">
                        <Link to="/" className="footer-brand-link">
                            <img src={logo} alt="Shop" className="header-logo" />
                            <span className="footer-logo">Shop</span>
                        </Link>
                        <p className="footer-brand-p">Ваш надёжный онлайн-партнёр для покупок. Лучшие товары, лучшие цены.</p>
                    </div>

                    <nav className="footer-nav">
                        <div className="footer-nav-column">
                            <h4 className="footer-title">Магазин</h4>
                            <ul className="footer-nav-list">
                                <li className="footer-nav-item">
                                    <NavLink to='/catalog' className="footer-link">Каталог</NavLink>
                                    <NavLink className="footer-link">Акции</NavLink>
                                    <NavLink className="footer-link">Новинки</NavLink>
                                    <NavLink className="footer-link">Популярное</NavLink>
                                </li>
                            </ul>
                        </div>
                        <div className="footer-nav-column">
                            <h4 className="footer-title">Поддержка</h4>
                            <ul className="footer-nav-list">
                                <li className="footer-nav-item">
                                    <NavLink className="footer-link">Помощь</NavLink>
                                    <NavLink className="footer-link">Доставка</NavLink>
                                    <NavLink className="footer-link">Возврат</NavLink>
                                    <NavLink className="footer-link">Контакты</NavLink>
                                </li>
                            </ul>
                        </div>
                        <div className="footer-nav-column">
                            <h4 className="footer-title">Соцсети</h4>
                            <ul className="footer-nav-list">
                                <li className="footer-nav-item">
                                    <NavLink className="footer-link">ВКонтакте</NavLink>
                                    <NavLink className="footer-link">Telegram</NavLink>
                                    <NavLink className="footer-link">Instagram</NavLink>
                                </li>
                            </ul>
                        </div>
                    </nav>
                </div>
            </div>
        </footer>
    );
}