import { useEffect, useState } from "react";
import { catalogAPI } from "../api/client";
import ProductCard from "../components/product/ProductCard";
import "./CatalogPage.css";

export default function CatalogPage() {
    const [products, setProducts] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(1);
    const [isLoading, setIsLoading] = useState(true);
    const pageSize = 12;

    const totalPages = Math.ceil(totalCount / pageSize);

    const loadProducts = async () => {
        try {
            setIsLoading(true);
            const data = await catalogAPI.getAll(page, pageSize);
            setProducts(data.products);
            setTotalCount(data.totalCount);
        } catch (err) {
            console.error("Ошибка загрузки:", err);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadProducts();
    }, [page]);

    const handlePrev = () => {
        if (page > 1) setPage(page - 1);
    };

    const handleNext = () => {
        if (page < totalPages) setPage(page + 1);
    };

    if(isLoading) {
        return <div>Загрузка...</div>
    }

    return (
        <div className="container catalog-container">
            <p className="catalog-count">{`Найдено ${totalCount} товаров`}</p>

            <main className="catalog-main-grid">
                {products.map(product => (
                    <ProductCard product={product} key={product.id} />
                ))}
            </main>
            
            {totalPages > 1 && (
                <nav className="catalog-pagination" aria-label="Навигация по страницам">
                    <button onClick={handlePrev} disabled={page === 1} aria-label="Предыдущая страница">&lt;</button>
                    <span className="catalog-pagination-info" aria-live="polite">
                        Страница {page} из {totalPages}
                    </span>
                    <button onClick={handleNext} disabled={page === totalPages} aria-label="Следующая страница">&gt;</button>
                </nav>
            )}
        </div>
    );
}
