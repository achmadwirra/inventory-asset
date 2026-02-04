import { useQuery } from '@tanstack/react-query';
import { usersApi } from './users.api';

export function useUsersByRoleQuery(role: string) {
    return useQuery({
        queryKey: ['users', role],
        queryFn: () => usersApi.getByRole(role),
        enabled: !!role,
    });
}
