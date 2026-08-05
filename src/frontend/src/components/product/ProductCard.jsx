import { Link } from "react-router-dom";
import { basketAPI } from "../../api/client";
import "./ProductCard.css";

export default function ProductCard({ product }) {
    const handleButtonClick = async (e) => {
        e.preventDefault();
        e.stopPropagation();
        await basketAPI.addProduct(product.id, 1);
    };

    const renderStars = (average) => {
        const full = Math.round(average);
        return '★'.repeat(full) + '☆'.repeat(5 - full);
    };

    return (
        <div className="product-card-wrapper">
            <Link to={`/catalog/${product.id}`}>
                <div className="product-container">
                    {product.imageUrl ? (
                        <img src={product.imageUrl} alt={product.name} />
                    ) : (
                        <div className="product-no-image">Нет фото</div>
                    )}
                    <p className="product-name">{product.name}</p>
                    
                    <div className="product-rating">
                        <span className="product-stars" aria-label={`Рейтинг ${product.averageStars || 0} из 5`}>
                            {renderStars(product.averageStars || 0)}
                        </span>
                        <span className="product-review-count">
                            {product.reviewCount || 0} отзывов
                        </span>
                    </div>

                    <div className="product-price-container">
                        <p className="product-price-p">{product.price} ₽</p>
                        {product.stockQuantity > 0 ? (
                            <button onClick={handleButtonClick}><span>+</span></button>
                        ) : (
                            <span className="out-of-stock">Нет в наличии</span>
                        )}
                    </div>
                </div>
            </Link>
        </div>
    );
}