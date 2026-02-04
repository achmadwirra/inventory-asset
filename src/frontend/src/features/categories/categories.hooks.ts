import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { categoriesApi, type CreateCategoryRequest, type UpdateCategoryRequest } from './categories.api';

export function useCategoriesQuery() {
    return useQuery({
        queryKey: ['categories'],
        queryFn: categoriesApi.getAll,
    });
}

export function useCreateCategoryMutation() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateCategoryRequest) => categoriesApi.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['categories'] });
        },
    });
}

export function useUpdateCategoryMutation() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateCategoryRequest }) =>
            categoriesApi.update(id, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['categories'] });
        },
    });
}

export function useDeleteCategoryMutation() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: string) => categoriesApi.delete(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['categories'] });
        },
    });
}
