import { ref, computed } from 'vue'
import axios from 'axios'
import type { AuthResponse, User } from '../api/models'
import { authApi } from '../api'

export function useAuth() {
    const user = ref<User | null>(null)
    const loading = ref(false)
    const error = ref<string | null>(null)

    // Restore user from localStorage if exists
    const storedUser = localStorage.getItem('user')
    if (storedUser) {
        user.value = JSON.parse(storedUser)
    }

    const isLoggedIn = computed(() => !!user.value)

    const saveUserToStorage = (
        userData: User,
        tokens: { accessToken: string; refreshToken: string; expiresAt: string }
        ) => {
            user.value = userData;
            localStorage.setItem('user', JSON.stringify(userData));
            localStorage.setItem('accessToken', tokens.accessToken);
            localStorage.setItem('refreshToken', tokens.refreshToken);
            localStorage.setItem('expiresAt', tokens.expiresAt);
    };

    const clearStorage = () => {
        user.value = null
        localStorage.removeItem('user')
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        localStorage.removeItem('expiresAt')
    }

    const register = async (
        email: string,
        password: string,
        passwordConfirm: string,
        name?: string
    ) => {
        loading.value = true;
        error.value = null;
        try {
            const res = await authApi.register(email, password, passwordConfirm, name);

            saveUserToStorage(res.data.user, {
                accessToken: res.data.accessToken,
                refreshToken: res.data.refreshToken,
                expiresAt: res.data.expiresAt,
            });

            return res.data;
        } catch (err: any) {
            const errorMessage = err?.response?.data?.error;
            error.value = mapErrorMessage(errorMessage) || 'Registration failed';
            throw err;
        } finally {
            loading.value = false;
        }
    };

    const login = async (email: string, password: string) => {
        loading.value = true;
        error.value = null;
        try {
            const res = await authApi.login(email, password);

            saveUserToStorage(res.data.user, {
                accessToken: res.data.accessToken,
                refreshToken: res.data.refreshToken,
                expiresAt: res.data.expiresAt
            });

            return res.data;
        } catch (err: any) {
            const errorMessage = err?.response?.data?.error;
            error.value = mapErrorMessage(errorMessage) || 'Invalid email or password';
            throw err;
        } finally {
            loading.value = false;
        }
    };

    const logout = async () => {
        loading.value = true
        error.value = null
        try {
            const accessToken = localStorage.getItem('accessToken')
            const refreshToken = localStorage.getItem('refreshToken')

            if (accessToken && refreshToken) {
                await axios.post(
                    '/api/auth/logout',
                    { refreshToken },
                    {
                        headers: {
                            Authorization: `Bearer ${accessToken}`,
                        },
                    }
                )
            }

            clearStorage()
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Logout failed'
            clearStorage()
            throw err
        } finally {
            loading.value = false
        }
    }

    const refreshAccessToken = async () => {
        try {
            const refreshToken = localStorage.getItem('refreshToken')
            if (!refreshToken) throw new Error('No refresh token available')

            const res = await axios.post<AuthResponse>('/api/auth/refresh-token', { refreshToken })
            localStorage.setItem('accessToken', res.data.accessToken)
            if (res.data.refreshToken) localStorage.setItem('refreshToken', res.data.refreshToken)
            localStorage.setItem('expiresAt', res.data.expiresAt)
            if (res.data.user) user.value = res.data.user
            return res.data.accessToken
        } catch (err: any) {
            error.value = 'Session expired. Please log in again.'
            clearStorage()
            throw err
        }
    }

    const mapErrorMessage = (errorMessage: string | undefined) => {
        if (!errorMessage) return undefined
        const map: Record<string, string> = {
            'Email already exists': 'This email is already registered.',
            'Invalid credentials': 'Invalid email or password.',
            'Password mismatch': 'Passwords do not match.',
        }
        return map[errorMessage] || errorMessage
    }

    return {
        user,
        isLoggedIn,
        loading,
        error,
        register,
        login,
        logout,
        refreshAccessToken,
    }
}
