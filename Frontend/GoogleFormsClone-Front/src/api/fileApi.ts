import api from './axios';
import type { FileResource } from './models';

export interface UploadFileResponse {
    fileId: string;
}

export const fileApi = {
    uploadFile: (file: File, associatedWith: string, associatedEntityType: 'Form' | 'Question' | 'User') => {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('associatedWith', associatedWith);
        formData.append('associatedEntityType', associatedEntityType);

        return api.post<UploadFileResponse>('/files/upload', formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        }).then(res => res.data);
    },

    getFilesByAssociatedEntity: (id: string, type: 'Form' | 'Question' | 'User') => {
        return api.get<FileResource[]>(`/files/associated/${id}?type=${type}`)
            .then(res => res.data);
    },

    downloadFile: (fileId: string) => {
        return api.get<Blob>(`/files/${fileId}`, { responseType: 'blob' });
    },

    deleteFile: (fileId: string) => {
        return api.delete(`/files/${fileId}`);
    },
    uploadAvatar: async (file: File, userId: string) => {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('associatedWith', userId);
        formData.append('associatedEntityType', 'User');

        const res = await api.post<UploadFileResponse>('/files/upload', formData, {
            headers: { 'Content-Type': 'multipart/form-data' },
        });

        return res.data.fileId; 
    },
};
