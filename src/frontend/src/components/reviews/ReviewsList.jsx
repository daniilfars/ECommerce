import { useEffect, useState } from "react";
import { reviewsAPI } from "../../api/client";
import "./ReviewsList.css";

export default function ReviewsList({ productId, refreshTrigger }) {
    const [reviews, setReviews] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(1);
    const [loading, setLoading] = useState(true);
    const pageSize = 5;

    const totalPages = Math.ceil(totalCount / pageSize);

    const loadReviews = async () => {
        setLoading(true);
        try {
            const data = await reviewsAPI.getByProduct(productId, page, pageSize);
            const list = data.reviews || data.items || [];
            const count = data.totalCount || data.total || list.length;
            setReviews(list);
            setTotalCount(count);
        } catch (err) {
            console.error("Ошибка загрузки отзывов:", err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadReviews();
    }, [productId, page, refreshTrigger]);

    const renderStars = (count) => {
        return '★'.repeat(count) + '☆'.repeat(5 - count);
    };

    if (loading) return <div className="reviews-loading">Загрузка отзывов...</div>;

    return (
        <section className="reviews-container">
            <h3 className="reviews-title">Отзывы ({totalCount})</h3>
            
            {reviews.length === 0 ? (
                <p className="reviews-empty">Отзывов пока нет. Будьте первым!</p>
            ) : (
                <ul className="reviews-list">
                    {reviews.map((review) => (
                        <li key={review.id} className="review-card">
                            <div className="review-header">
                                <span className="review-stars" aria-label={`Оценка ${review.stars} из 5`}>
                                    {renderStars(review.stars)}
                                </span>
                                {review.createdAt && (
                                    <time className="review-date" dateTime={review.createdAt}>
                                        {new Date(review.createdAt).toLocaleDateString()}
                                    </time>
                                )}
                            </div>
                            <p className="review-text">{review.text}</p>
                            {review.userId && (
                                <p className="review-author">Пользователь {review.userId.substring(0, 8)}...</p>
                            )}
                        </li>
                    ))}
                </ul>
            )}

            {totalPages > 1 && (
                <nav className="pagination" aria-label="Пагинация отзывов">
                    <button onClick={() => setPage(p => p - 1)} disabled={page === 1}>
                        ← Назад
                    </button>
                    <span>{page} / {totalPages}</span>
                    <button onClick={() => setPage(p => p + 1)} disabled={page >= totalPages}>
                        Вперёд →
                    </button>
                </nav>
            )}
        </section>
    );
}