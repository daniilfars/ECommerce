import { Routes, Route } from 'react-router-dom';
import HomePage from '../pages/HomePage/HomePage.jsx';
import CatalogPage from '../pages/CatalogPage/CatalogPage.jsx';
import ProductPage from '../pages/ProductPage/ProductPage.jsx';
import LoginPage from '../pages/Auth/LoginPage.jsx';
import RegisterPage from '../pages/Auth/RegisterPage.jsx';
import BasketPage from '../pages/BasketPage/BasketPage.jsx';
import CheckoutPage from '../pages/CheckoutPage/CheckoutPage.jsx';
import OrdersPage from '../pages/OrdersPage/OrdersPage.jsx';
import OrderDetailPage from '../pages/OrderDetailPage/OrderDetailPage.jsx';
import AdminPage from '../pages/Admin/AdminPage.jsx';
import AdminProducts from '../pages/Admin/AdminProducts.jsx';
import AdminOrders from '../pages/Admin/AdminOrders.jsx';
import ProtectedRoute from './ProtectedRoute.jsx';

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
            <Route path="/admin" element={
                <ProtectedRoute requiredRole="Admin">
                    <AdminPage />
                </ProtectedRoute>
            }>
                <Route index element={<AdminProducts />} />
                <Route path="products" element={<AdminProducts />} />
                <Route path="orders" element={<AdminOrders />} />
            </Route>
        </Routes>
    )
}

export default AppRouter;