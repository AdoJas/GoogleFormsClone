import { ref } from 'vue';
import { responseApi } from '../api';
import type { Response } from '../api/models';

export function useResponses() {
    const loading = ref(false);
    const error = ref<string | null>(null);
    const responses = ref<Response[]>([]);

    const fetchByForm = async (formId: string) => {
        loading.value = true;
        error.value = null;
        try {
            const res = await responseApi.getByForm(formId);
            responses.value = res.data;
            return res.data;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to fetch responses';
            throw err;
        } finally {
            loading.value = false;
        }
    };

    const create = async (response: Partial<Response>) => {
        try {
            const res = await responseApi.create(response);
            responses.value.push(res.data);
            return res.data;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to submit response';
            throw err;
        }
    };

    return {
        loading,
        error,
        responses,
        fetchByForm,
        create
    };
}
