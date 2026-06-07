import { useEffect, useState } from "react";
import { catalogAPI } from "../api/client";
import ProductCard from "../components/product/ProductCard";
import "./CatalogPage.css";

export default function CatalogPage() {
    const [products, setProducts] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(1);
    const pageSize = 12;

    const totalPages = Math.ceil(totalCount / pageSize);

    const loadProducts = async () => {
        try {
            const data = await catalogAPI.getAll(page, pageSize);
            setProducts(data.products);
            setTotalCount(data.totalCount);
        } catch (err) {
            console.error("Ошибка загрузки:", err);
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

    return (
        <div className="container catalog-container">
            <p className="catalog-count">Найдено {totalCount} товаров</p>
            <div className="products-grid">
                {products.map(product => (
                    <ProductCard product={product} key={product.id} />
                ))}
            </div>
            {totalPages > 1 && (
                <div className="pagination">
                    <button onClick={handlePrev} disabled={page === 1}>
                        &lt;
                    </button>
                    <span>{page} / {totalPages}</span>
                    <button onClick={handleNext} disabled={page === totalPages}>
                        &gt;
                    </button>
                </div>
            )}
        </div>
    );
}