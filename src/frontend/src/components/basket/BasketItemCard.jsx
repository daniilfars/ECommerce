import { basketAPI } from "../../api/client";
import "./BasketItemCard.css";
import icon from "../../assets/мусорка.png";

export default function BasketItemCard({basketItem, onUpdate}) {
    const handleIncrement = async () => {
        if (basketItem.quantity >= basketItem.stockQuantity) return;
        await basketAPI.updateQuantity(basketItem.productId, basketItem.quantity + 1);
        if (onUpdate)
            await onUpdate();
    };

    const handleDecrement = async () => {
        if(basketItem.quantity > 1) {
            await basketAPI.updateQuantity(basketItem.productId, basketItem.quantity - 1);
            if (onUpdate)
                await onUpdate();
        }
    };

    const handleDelete = async () => {
        try {
            await basketAPI.deleteProduct(basketItem.productId);
            if (onUpdate)
                await onUpdate();
        } catch (err) {
            console.error(err);
        }
    };

    return (
        <li className="basket-item">
            <figure className="basket-item-image">
                <img src={basketItem.imageUrl} alt={`Изображение товара ${basketItem.productName}`} />
            </figure>

            <div className="basket-item-info">
                <h4 className="basket-item-title">{basketItem.productName}</h4>
                <p className="basket-item-price">{basketItem.totalPrice} ₽</p>
                <p className="basket-item-stock">В наличии: {basketItem.stockQuantity} шт.</p>
            </div>

            <div className="basket-item-actions">
                <button onClick={handleDelete} className="basket-item-delete" aria-label="Удалить товар">
                    <img src={icon} alt="Мусорка" className="basket-delete-image" />
                </button>
                <div className="product-quantity-controls">
                    <button onClick={handleDecrement} disabled={basketItem.quantity <= 1} aria-label="Уменьшить количество">-</button>
                    <span className="product-quantity-value">{basketItem.quantity}</span>
                    <button onClick={handleIncrement} disabled={basketItem.quantity >= basketItem.stockQuantity} aria-label="Увеличить количество">+</button>
                </div>
            </div>
        </li>
    );
};