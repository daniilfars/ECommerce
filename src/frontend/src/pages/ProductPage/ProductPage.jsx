import { useParams, NavLink } from "react-router-dom";
import { useEffect, useState } from "react";
import { basketAPI, catalogAPI } from "../../api/client";
import cartIcon from "../../assets/shopping-cart.svg";
import truck from "../../assets/truck.svg";
import check from "../../assets/shield-check.svg";
import ReviewForm from "../../components/reviews/ReviewForm";
import ReviewsList from "../../components/reviews/ReviewsList";
import "./ProductPage.css";

export default function ProductPage() {
    const [product, setProduct] = useState(null);
    const { id } = useParams();
    const [quantity, setQuantity] = useState(1);
    const [isAdded, setIsAdded] = useState(false);
    const [reviewRefresh, setReviewRefresh] = useState(0);

    const loadProduct = async () => {
        try {
            const data = await catalogAPI.getById(id);
            setProduct(data);
        } catch (err) {
            console.error("Ошибка загрузки:", err);
        }
    };

    useEffect(() => {
        loadProduct();
    }, [id]);

    useEffect(() => {
        if (!isAdded) return;
        const timer = setTimeout(() => setIsAdded(false), 2000);
        return () => clearTimeout(timer);
    }, [isAdded]);

    const handleReviewSuccess = () => setReviewRefresh(prev => prev + 1);

    const handlerAdd = async () => {
        try {
            await basketAPI.addProduct(id, quantity);
            setIsAdded(true);
        } catch (err) {
            console.error('Ошибка добавления в корзину:', err);
        }
    };

    const handleIncrement = () => {
        setQuantity(prev => Math.min(prev + 1, product?.stockQuantity || 1));
    };

    const handleDecrement = () => {
        setQuantity(prev => (prev > 1 ? prev - 1 : 1));
    };

    if (!product) {
        return <div className="product-loading">Загрузка ...</div>;
    }

    return (
        <>
            <nav className="product-nav" aria-label="Хлебные крошки">
                <div className="container product-nav-container">
                    <ul className="product-list">
                        <li className="product-item">
                            <NavLink to="/" className="product-link">Главная</NavLink>
                        </li>
                        <li className="product-item">
                            <NavLink to="/catalog" className="product-link">Каталог</NavLink>
                        </li>
                        <li className="product-item">
                            <span aria-current="page" className="product-current">{product.name}</span>
                        </li>
                    </ul>
                </div>
            </nav>

            <main className="product-main">
                <div className="container product-main-container">
                    <figure className="product-image">
                        {product.imageUrl ? (
                            <img src={product.imageUrl} alt={`Товар ${product.name}`} />
                        ) : (
                            <div className="product-no-image">Нет фото</div>
                        )}
                    </figure>

                    <section className="product-info" aria-label="Информация о товаре">
                        <h1 className="product-title">{product.name}</h1>
                        
                        {/* Рейтинг и количество отзывов */}
                        <div className="product-rating">
                            <span className="product-stars" aria-label={`Рейтинг ${product.averageStars || 0} из 5`}>
                                {'★'.repeat(Math.round(product.averageStars || 0)) + '☆'.repeat(5 - Math.round(product.averageStars || 0))}
                            </span>
                            <span className="product-rating-value">
                                {product.averageStars?.toFixed(1) || '0.0'}
                            </span>
                            <span className="product-review-count">
                                ({product.reviewCount || 0} отзывов)
                            </span>
                        </div>

                        <h3 className="product-price">{product.price} ₽</h3>
                        <p className={`product-stock ${product.stockQuantity > 0 ? 'in-stock' : 'out-of-stock'}`}>
                            {product.stockQuantity > 0 ? `В наличии: ${product.stockQuantity} шт.` : 'Нет в наличии'}
                        </p>

                        {product.stockQuantity > 0 ? (
                            <div className="product-button-container">
                                <div className="product-quantity-controls">
                                    <button onClick={handleDecrement} disabled={quantity <= 1} aria-label="Уменьшить количество">-</button>
                                    <span className="product-quantity-value">{quantity}</span>
                                    <button onClick={handleIncrement} disabled={quantity >= product.stockQuantity} aria-label="Увеличить количество">+</button>
                                </div>

                                <button className={`product-button-add${isAdded ? " added" : ""}`} onClick={handlerAdd} disabled={isAdded}>
                                    {isAdded ? (
                                        <span>✓ Добавлено</span>
                                    ) : (
                                        <>
                                            <img src={cartIcon} alt="" className="product-cartIcon" aria-hidden="true" />
                                            <span>Добавить в корзину</span>
                                        </>
                                    )}
                                </button>
                            </div>
                        ) : (
                            <button className="product-button-add product-button-disabled" disabled>
                                <span>Нет в наличии</span>
                            </button>
                        )}

                        <div className="product-info-container">
                            <div className="product-info-item">
                                <img src={truck} alt="" className="product-info-image" aria-hidden="true" />
                                <div className="product-item-container">
                                    <strong className="product-info-title">Доставка завтра</strong>
                                    <p className="product-info-desc">Бесплатная доставка</p>
                                </div>
                            </div>
                        
                            <div className="product-info-item">
                                <img src={check} alt="" className="product-info-image" aria-hidden="true" />
                                <div className="product-item-container">
                                    <strong className="product-info-title">Гарантия 2 года</strong>
                                    <p className="product-info-desc">Официальная гарантия</p>
                                </div>
                            </div>
                        </div>
                    </section>
                </div>

                <section className="product-reviews container">
                    <ReviewForm productId={product.id} onSuccess={handleReviewSuccess} />
                    <ReviewsList productId={product.id} refreshTrigger={reviewRefresh} />
                </section>
            </main>
        </>
    );
}