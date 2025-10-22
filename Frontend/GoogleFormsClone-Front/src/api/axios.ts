import axios from 'axios';

const api = axios.create({
    baseURL: '/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

api.interceptors.request.use((config) => {
    console.log('Axios request:', config.url, config.baseURL);
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
}, (error) => {
    console.error('Axios request error:', error);
    return Promise.reject(error);
});

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        console.error('Axios response error:', error.response?.status, error.response?.data);
        const originalRequest = error.config;
        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;
            try {
                const refreshToken = localStorage.getItem('refreshToken');
                if (!refreshToken) throw new Error('No refresh token available');

                console.log('Attempting token refresh');
                const response = await axios.post('/api/auth/refresh', { refreshToken });
                const { accessToken, refreshToken: newRefreshToken, expiresAt } = response.data;

                localStorage.setItem('accessToken', accessToken);
                if (newRefreshToken) localStorage.setItem('refreshToken', newRefreshToken);
                localStorage.setItem('expiresAt', expiresAt);

                originalRequest.headers.Authorization = `Bearer ${accessToken}`;
                return api(originalRequest);
            } catch (refreshError) {
                console.error('Refresh token failed:', refreshError);
                localStorage.removeItem('accessToken')
                localStorage.removeItem('refreshToken')
                localStorage.removeItem('expiresAt')
                localStorage.removeItem('user')
                window.location.href = '/login';
                return Promise.reject(refreshError);
            }
        }
        return Promise.reject(error);
    }
);

export default api;