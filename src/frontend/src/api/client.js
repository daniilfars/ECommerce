const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

async function request(endpoint, options = {}) {
    const url = `${API_BASE}${endpoint}`;
    const token = localStorage.getItem('accessToken');

    const config = {
        ...options,
        credentials: 'include',
        headers: {
            ...options.headers,
        },
    };

    if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
    }

    if (!(options.body instanceof FormData)) {
        config.headers['Content-Type'] = 'application/json';
    }

    let response = await fetch(url, config);

    // Автообновление токена при 401
    if (response.status === 401) {
        const refreshResponse = await fetch(`${API_BASE}/Identity/refresh`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({}),
        });

        if (refreshResponse.ok) {
            const data = await refreshResponse.json();
            const newToken = data.accessToken || data.value?.accessToken;
            
            if (newToken) {
                localStorage.setItem('accessToken', newToken);
                window.dispatchEvent(new Event('tokenUpdated')); // Для AuthContext, чтоб обновил компоненты
                config.headers['Authorization'] = `Bearer ${newToken}`;
                response = await fetch(url, config);
            } else {
                localStorage.removeItem('accessToken');
                window.location.href = '/login';
                throw new Error('Не удалось обновить токен');
            }
        } else {
            localStorage.removeItem('accessToken');
            window.location.href = '/login';
            throw new Error('Сессия истекла');
        }
    }

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || `Ошибка ${response.status}`);
    }

    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
        return response.json();
    }
    return null;
}

export const authAPI = {
    login: (email, password) =>
        request('/Identity/login', {
            method: 'POST',
            body: JSON.stringify({email, password})
        }),

    register: (firstName, lastName, email, password) =>
        request('/Identity/register', {
            method: 'POST',
            body: JSON.stringify({firstName, lastName, email, password})
        }),

    logout: () =>
        request('/Identity/logout', {
            method: 'POST',
        }),

    refresh: () =>
        request('/Identity/refresh', {
            method: 'POST',
        }),
};

export const catalogAPI = {
    getAll: (page = 1, pageSize = 10) =>
        request(`/Catalog?page=${page}&pageSize=${pageSize}`, {
           method: 'GET', 
        }),
    
    getById: (id) =>
        request(`/Catalog/${id}`, {
            method: 'GET',
        }),

    create: (name, price, stockQuantity) =>
        request('/Catalog', {
            method: 'POST',
            body: JSON.stringify({name, price, stockQuantity})
        }),
    
    update: (id, name = null, price = null, stockQuantity = null) =>
        request(`/Catalog/${id}`, {
            method: 'PUT',
            body: JSON.stringify({id, name, price, stockQuantity})
        }),

    delete: (id) =>
        request(`/Catalog/${id}`, {
            method: 'DELETE',
        }),

    uploadImage: (id, file) => {
        const formData = new FormData();
        formData.append('file', file);

        return request(`/Catalog/${id}/upload-image`, {
            method: 'POST',
            body: formData,
        });
    },  
}

export const basketAPI = {
    get: () =>
        request('/Basket', {
            method: 'GET',
        }),

    addProduct: (productId, quantity) =>
        request('/Basket', {
            method: 'POST',
            body: JSON.stringify({productId, quantity})
        }),
    
    checkout: (shippingAddress) =>
        request('/Basket/checkout', {
            method: 'POST',
            body: JSON.stringify({ shippingAddress }),
        }),

    deleteProduct: (productId) =>
        request(`/Basket/${productId}`, {
            method: 'DELETE',
        }),

    updateQuantity: (productId, quantity) =>
        request(`/Basket/${productId}`, {
            method: 'PATCH',
            body: JSON.stringify({quantity}),
        }),
}

export const orderingAPI = {
    getAll: (page = 1, pageSize = 10) =>
        request(`/Ordering?page=${page}&pageSize=${pageSize}`, {
           method: 'GET', 
        }),

    getAllOrders: (page = 1, pageSize = 50) =>
        request(`/Ordering/all?page=${page}&pageSize=${pageSize}`, {
            method: 'GET',
        }),
    
    getById: (orderId) =>
        request(`/Ordering/${orderId}`, {
            method: 'GET',
        }),

    cancel: (orderId) =>
        request(`/Ordering/${orderId}/cancel`, {
        method: 'POST',
        }),

    pay: (orderId) =>
        request(`/Ordering/${orderId}/pay`, {
            method: 'POST',
        }),

    ship: (orderId) =>
        request(`/Ordering/${orderId}/ship`, {
            method: 'POST',
        }),

    deliver: (orderId) =>
        request(`/Ordering/${orderId}/deliver`, {
            method: 'POST',
        }),
}