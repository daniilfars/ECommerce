import { useState } from "react";
import { reviewsAPI } from "../../api/client";
import "./ReviewForm.css";

export default function ReviewForm({ productId, onSuccess }) {
    const [text, setText] = useState("");
    const [stars, setStars] = useState(5);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!text.trim()) {
            setError("Введите текст отзыва");
            return;
        }
        setError("");
        setSubmitting(true);
        try {
            await reviewsAPI.create(productId, text, stars);
            setText("");
            setStars(5);
            if (onSuccess) onSuccess();
        } catch (err) {
            setError(err.message || "Ошибка отправки отзыва");
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <form className="review-form" onSubmit={handleSubmit}>
            <h4>Оставить отзыв</h4>
            <div className="review-form-stars">
                {[1, 2, 3, 4, 5].map((s) => (
                    <button
                        type="button"
                        key={s}
                        className={`star-btn ${s <= stars ? "active" : ""}`}
                        onClick={() => setStars(s)}
                        aria-label={`Оценка ${s} из 5`}
                    >
                        ★
                    </button>
                ))}
            </div>
            <textarea
                className="review-form-text"
                placeholder="Ваш отзыв..."
                value={text}
                onChange={(e) => setText(e.target.value)}
                required
                rows={4}
            />
            {error && <p className="review-form-error">{error}</p>}
            <button type="submit" className="review-form-btn" disabled={submitting}>
                {submitting ? "Отправка..." : "Отправить отзыв"}
            </button>
        </form>
    );
}