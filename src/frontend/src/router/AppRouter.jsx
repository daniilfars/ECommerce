import { Routes, Route } from 'react-router-dom';
import HomePage from '../pages/HomePage.jsx';
import CatalogPage from '../pages/CatalogPage.jsx';
import ProductPage from '../pages/ProductPage.jsx';
import LoginPage from '../pages/LoginPage.jsx';
import RegisterPage from '../pages/RegisterPage.jsx';
import BasketPage from '../pages/BasketPage.jsx';
import CheckoutPage from '../pages/CheckoutPage.jsx';
import OrdersPage from '../pages/OrdersPage.jsx';
import ProtectedRoute from './ProtectedRoute.jsx';
import OrderDetailPage from '../pages/OrderDetailPage.jsx';

function AppRouter() {
    return (
        <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/catalog" element={<CatalogPage />} />
            <Route path="/catalog/:id" element={<ProductPage />}/>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />}/>
            <Route path="/basket" element={
                <ProtectedRoute>
                    <BasketPage />
                </ProtectedRoute>
            } />
            <Route path="/checkout" element={
                <ProtectedRoute>
                    <CheckoutPage  />
                </ProtectedRoute>
            } />
            <Route path="/orders" element={
                <ProtectedRoute>
                    <OrdersPage  />
                </ProtectedRoute>
            } />
            <Route path="/orders/:id" element={
                <ProtectedRoute>
                    <OrderDetailPage />
                </ProtectedRoute>
            } />
        </Routes>
    )
}

export default AppRouter;