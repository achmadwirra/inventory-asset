import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { assetsApi } from './assets.api';
import type { CreateAssetRequest, AssignAssetRequest } from '../../types';

export const ASSETS_QUERY_KEY = ['assets'];

export function useAssetsQuery() {
    return useQuery({
        queryKey: ASSETS_QUERY_KEY,
        queryFn: assetsApi.getAll,
    });
}

export function useAssetQuery(id: string) {
    return useQuery({
        queryKey: [...ASSETS_QUERY_KEY, id],
        queryFn: () => assetsApi.getById(id),
        enabled: !!id,
    });
}

export function useCreateAssetMutation() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateAssetRequest) => assetsApi.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ASSETS_QUERY_KEY });
        },
    });
}

export function useAssignAssetMutation() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: AssignAssetRequest }) =>
            assetsApi.assign(id, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ASSETS_QUERY_KEY });
        },
    });
}

export function useReturnAssetMutation() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: string) => assetsApi.return(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ASSETS_QUERY_KEY });
        },
    });
}
