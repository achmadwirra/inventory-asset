import api from '../../services/api';
import type { AssetCategory } from '../../types';

export interface CreateCategoryRequest {
    name: string;
}

export interface UpdateCategoryRequest {
    name: string;
}

export const categoriesApi = {
    getAll: async (): Promise<AssetCategory[]> => {
        const response = await api.get<AssetCategory[]>('/categories');
        return response.data;
    },

    create: async (data: CreateCategoryRequest): Promise<AssetCategory> => {
        const response = await api.post<AssetCategory>('/categories', data);
        return response.data;
    },

    update: async (id: string, data: UpdateCategoryRequest): Promise<AssetCategory> => {
        const response = await api.put<AssetCategory>(`/categories/${id}`, data);
        return response.data;
    },

    delete: async (id: string): Promise<void> => {
        await api.delete(`/categories/${id}`);
    },
};
