import { ref } from 'vue';
import type { User } from '../api/models';
import { userApi } from '../api';
export function useUser() {
    const loading = ref(false);
    const error = ref<string | null>(null);
    const currentUser = ref<User | null>(null);

    const fetchMe = async () => {
        loading.value = true;
        error.value = null;
        try {
            const res = await userApi.me();
            currentUser.value = res.data;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to fetch user';
        } finally {
            loading.value = false;
        }
    };

    const updateMe = async (data: Partial<User>) => {
        loading.value = true;
        error.value = null;
        try {
            const res = await userApi.updateMe(data);
            currentUser.value = res.data;
            return res.data;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to update user';
            throw err;
        } finally {
            loading.value = false;
        }
    };

    const deleteMe = async () => {
        loading.value = true;
        error.value = null;
        try {
            await userApi.deleteMe();
            currentUser.value = null;
        } catch (err: any) {
            error.value = err?.response?.data?.error || 'Failed to delete user';
            throw err;
        } finally {
            loading.value = false;
        }
    };
    const setUser = (user: User | null) => {
        currentUser.value = user
    }

    return {
        loading,
        error,
        currentUser,
        fetchMe,
        updateMe,
        deleteMe,
        setUser,
    };
}
