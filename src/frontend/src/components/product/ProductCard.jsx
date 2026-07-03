import { Link } from "react-router-dom";
import { basketAPI } from "../../api/client";
import "./ProductCard.css";

export default function ProductCard({ product }) {
    const handleButtonClick = async (e) => {
        e.preventDefault();
        e.stopPropagation();
        await basketAPI.addProduct(product.id, 1);
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
                    
                    <div className="product-price-container">
                        <p className="product-price-p">{product.price} ₽</p>
                        <button onClick={handleButtonClick}><span>+</span></button>
                    </div>
                </div>
            </Link>
        </div>
    );
}
