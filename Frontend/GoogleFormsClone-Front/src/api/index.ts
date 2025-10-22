import api from './axios';
import type { AuthResponse } from './models';
import type { User } from './models';
import type { Form } from './models';
import type { Response } from './models';
import axios from 'axios';


export const authApi = {
    register: (email: string, password: string, passwordConfirm: string, name?: string) =>
        api.post<AuthResponse>('/auth/register', { email, password, passwordConfirm, name }),
    login: (email: string, password: string) =>
        api.post<AuthResponse>('/auth/login', { email, password }),
    logout: (refreshToken: string) =>
        api.post('/auth/logout', { refreshToken }),
    refreshToken: (refreshToken: string) =>
        api.post<AuthResponse>('/auth/refresh', { refreshToken }),
};
export const userApi = {
    me: () => api.get<User>('/users/me'),
    updateMe: (data: Partial<User>) => api.patch<User>('/users/me', data),
    deleteMe: () => api.delete('/users/me'),
}
export const formApi = {
    getAll: () => api.get<Form[]>('/forms'),
    getByUser: (userId: string) => api.get<Form[]>(`/forms/user/${userId}`),
    getById: (id: string) => api.get<Form>(`/forms/${id}`),
    create: (form: Partial<Form>) => api.post<Form>('/forms', form),
    update: (id: string, form: Partial<Form>) => api.put<Form>(`/forms/${id}`, form),
    delete: (id: string) => api.delete(`/forms/${id}`),
}
export const responseApi = {
    submit: (payload: Partial<Response>) => api.post<Response>('/response', payload),
    getByFormId: (formId: string) => api.get<Response[]>(`/response/${formId}/responses`),
    getById: (id: string) => api.get<Response>(`/response/${id}`),
};

export default api;