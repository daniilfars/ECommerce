import { createContext, useContext, useState, useEffect, useCallback } from "react";
import { authAPI} from '../api/client';

const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    const parseToken = (token) => {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return {
                userId: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
                email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
                role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
            }
        } catch {
            return null;
        }
    }

    const updateUserFromToken = useCallback(() => {
        const token = localStorage.getItem('accessToken');
        if (token) {
            const parsed = parseToken(token);
            setUser(parsed);
        } else {
            setUser(null);
        }
    }, []);

    useEffect(() => {
        const handleTokenUpdate = () => updateUserFromToken();
        window.addEventListener('tokenUpdated', handleTokenUpdate);
        return () => window.removeEventListener('tokenUpdated', handleTokenUpdate);
    }, []);

    useEffect(() => {
        const token = localStorage.getItem('accessToken');
        if (token){
            const parsed = parseToken(token);
            setUser(parsed);
        }
        setLoading(false);
    }, []);

    const login = async (email, password) => {
        const data = await authAPI.login(email, password);
        localStorage.setItem('accessToken', data.accessToken);
        const parsed = parseToken(data.accessToken);
        setUser(parsed);
        return data;
    };

    const register = async (firstName, lastName, email, password) => {
        await authAPI.register(firstName, lastName, email, password);
        return login(email, password);
    };

    const logout = async () => {
        try {
            await authAPI.logout();
        } catch {

        }
        localStorage.removeItem('accessToken');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, loading, login, logout, register }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}