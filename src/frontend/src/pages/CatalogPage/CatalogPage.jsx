import { useEffect, useState } from "react";
import { catalogAPI } from "../../api/client";
import ProductCard from "../../components/product/ProductCard";
import "./CatalogPage.css";

export default function CatalogPage() {
    const [products, setProducts] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(1);
    const [isLoading, setIsLoading] = useState(true);
    const [minPrice, setMinPrice] = useState('');
    const [maxPrice, setMaxPrice] = useState('');
    const [searchQuery, setSearchQuery] = useState('');
    const [isSearching, setIsSearching] = useState(false);
    const [isSearchMode, setIsSearchMode] = useState(false);
    const pageSize = 12;

    const totalPages = Math.ceil(totalCount / pageSize);

    const loadProducts = async (currentPage = page, currentMin = minPrice, currentMax = maxPrice) => {
        try {
            setIsLoading(true);
            setIsSearchMode(false);
            const data = await catalogAPI.getAll(
                currentPage, 
                pageSize, 
                currentMin || null, 
                currentMax || null
            );
            setProducts(data.products);
            setTotalCount(data.totalCount);
        } catch (err) {
            console.error("Ошибка загрузки:", err);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (!isSearchMode) {
            loadProducts();
        }
    }, [page]);

    const handleSearch = async () => {
        if (!searchQuery.trim()) return;
        setIsSearching(true);
        setIsSearchMode(true);
        setPage(1);
        try {
            const data = await catalogAPI.search(searchQuery, minPrice || null, maxPrice || null);
            setProducts(data.products || []);
            setTotalCount(data.totalCount || 0);
        } catch (err) {
            console.error("Ошибка поиска:", err);
        } finally {
            setIsSearching(false);
        }
    };

    const handleApplyFilter = () => {
        setPage(1);
        setIsSearchMode(false);
        loadProducts(1, minPrice, maxPrice);
    };

    const handleReset = () => {
        setMinPrice('');
        setMaxPrice('');
        setSearchQuery('');
        setIsSearchMode(false);
        setPage(1);
        loadProducts(1, '', '');
    };

    const handlePrev = () => {
        if (page > 1) setPage(page - 1);
    };

    const handleNext = () => {
        if (page < totalPages) setPage(page + 1);
    };

    if(isLoading) {
        return <div className="catalog-loading">Загрузка...</div>
    }

    return (
        <div className="container catalog-container">
            <p className="catalog-count">{`Найдено ${totalCount} товаров`}</p>

            <div className="catalog-filters">
                <div className="catalog-filters-inputs">
                    <input
                        type="text"
                        placeholder="Поиск товаров..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                        className="catalog-filter-input catalog-search-input"
                    />
                    <input
                        type="number"
                        placeholder="Цена от"
                        value={minPrice}
                        onChange={(e) => setMinPrice(e.target.value)}
                        className="catalog-filter-input"
                    />
                    <input
                        type="number"
                        placeholder="Цена до"
                        value={maxPrice}
                        onChange={(e) => setMaxPrice(e.target.value)}
                        className="catalog-filter-input"
                    />
                </div>
                
                <div className="catalog-filters-buttons">
                    <button onClick={handleSearch} className="catalog-filter-btn">
                        {isSearching ? 'Поиск...' : 'Поиск'}
                    </button>
                    <button onClick={handleApplyFilter} className="catalog-filter-btn">
                        Фильтр
                    </button>
                    <button onClick={handleReset} className="catalog-filter-btn catalog-filter-clear">
                        Сбросить
                    </button>
                </div>
            </div>

            <main className="catalog-main-grid">
                {products.map(product => (
                    <ProductCard product={product} key={product.id} />
                ))}
            </main>
            
            {totalPages > 1 && !isSearchMode && (
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