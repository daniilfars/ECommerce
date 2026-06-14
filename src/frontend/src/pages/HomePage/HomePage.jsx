import { NavLink } from "react-router-dom";
import "./HomePage.css";

export default function HomePage() {
    return <section className="home-section">
        <div className="home-item">
            <div className="container home-item-container">
                <span className="home-span">Новая коллекция 2026</span>
                <h1 className="home-title">Открой мир</h1>
                <p className="home-p">Тысячи товаров лучших брендов с доставкой по всей России. Качество, которому доверяют.</p>
                <NavLink to="/catalog" className="home-navlink">Смотреть каталог</NavLink>
                <ul className="home-list">
                    <li className="home-list-item">
                        <strong>50 000+</strong>
                        <span>Товаров</span>
                    </li>
                    <li className="home-list-item">
                        <strong>1 200+</strong>
                        <span>Брендов</span>
                    </li>
                    <li className="home-list-item">
                        <strong>4.9</strong>
                        <span>Рейтинг</span>
                    </li>
                </ul>
            </div>
        </div>
        <div className="home-item">
            {/* Если будет что-то, то обернуть в div.container */}
        </div>
    </section>;
}