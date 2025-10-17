import { ref } from 'vue';
import { formApi } from '../api';
import type { Form } from '../api/models';

export function useForms() {
    const loading = ref(false);
    const error = ref<string | null>(null);
    const forms = ref<Form[]>([]);

    const fetchAll = async () => {
        loading.value = true;
        error.value = null;
        try {
            const res = await formApi.getAll();
            forms.value = res.data;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to fetch forms';
        } finally {
            loading.value = false;
        }
    };

    const fetchByUser = async (userId: string) => {
        loading.value = true;
        error.value = null;
        try {
            const res = await formApi.getByUser(userId);
            forms.value = res.data;
            return res.data;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to fetch user forms';
            throw err;
        } finally {
            loading.value = false;
        }
    };

    const create = async (form: Partial<Form>) => {
        try {
            const res = await formApi.create(form);
            forms.value.push(res.data);
            return res.data;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to create form';
            throw err;
        }
    };

    const update = async (id: string, form: Partial<Form>) => {
        try {
            const res = await formApi.update(id, form);
            const index = forms.value.findIndex(f => f.id === id);
            if (index !== -1) forms.value[index] = res.data;
            return res.data;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to update form';
            throw err;
        }
    };

    const remove = async (id: string) => {
        try {
            await formApi.delete(id);
            forms.value = forms.value.filter(f => f.id !== id);
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to delete form';
            throw err;
        }
    };

    return {
        loading,
        error,
        forms,
        fetchAll,
        fetchByUser,
        create,
        update,
        remove
    };
}
