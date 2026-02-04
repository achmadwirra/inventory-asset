import api from '../../services/api';
import type { Asset, CreateAssetRequest, AssignAssetRequest } from '../../types';

export const assetsApi = {
    getAll: async (): Promise<Asset[]> => {
        const response = await api.get<Asset[]>('/assets');
        return response.data;
    },

    getById: async (id: string): Promise<Asset> => {
        const response = await api.get<Asset>(`/assets/${id}`);
        return response.data;
    },

    create: async (data: CreateAssetRequest): Promise<Asset> => {
        const response = await api.post<Asset>('/assets', data);
        return response.data;
    },

    assign: async (id: string, data: AssignAssetRequest): Promise<void> => {
        await api.post(`/assets/${id}/assign`, data);
    },

    return: async (id: string): Promise<void> => {
        await api.post(`/assets/${id}/return`);
    },
};
