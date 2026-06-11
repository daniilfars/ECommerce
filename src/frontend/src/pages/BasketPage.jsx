import { useEffect, useState } from "react";
import { basketAPI } from "../api/client";
import BasketItemCard from "../components/basket/BasketItemCard";
import { NavLink } from "react-router-dom";
import './BasketPage.css';

export default function BasketPage() {
    const [products, setProducts] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [totalAmount, setTotalAmount] = useState(0);

    const loadProducts = async () => {
        try {
            const data = await basketAPI.get();
            setProducts(data.items);
            setTotalCount(data.items?.reduce((sum, item) => sum + item.quantity, 0) || 0);
            setTotalAmount(data.totalAmount);
        } catch (err) {
            console.error("Ошибка загрузки:", err);
        }
    };

    useEffect(() => {
        loadProducts();
    }, []);

    if (products.length === 0) {
        return (
            <div className="container basket-empty">
                <h2>Корзина пуста</h2>
                <p>Добавьте товары из каталога</p>
                <NavLink to="/catalog" className="basket-empty-link">Перейти в каталог</NavLink>
            </div>
        );
    }

    return (
        <div className="container basket-container">
            <main className="basket-main">
                <ul className="basket-list">
                    {products.map(product => (
                        <BasketItemCard basketItem={product} key={product.productId} onUpdate={loadProducts} />
                    ))}
                </ul>
            </main>
                
            <NavLink to="/checkout" className="basket-checkout-button">
                <span>К оформлению • {totalCount}</span>
                <span>{totalAmount} ₽</span>
            </NavLink>
        </div>
    );
}