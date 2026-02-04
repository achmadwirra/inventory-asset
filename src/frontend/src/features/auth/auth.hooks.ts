import { useMutation } from '@tanstack/react-query';
import { authApi } from './auth.api';
import { useAuthStore } from '../../stores/auth.store';
import { useNavigate } from 'react-router-dom';
import type { LoginRequest } from '../../types';

export function useLogin() {
    const login = useAuthStore((state) => state.login);
    const navigate = useNavigate();

    return useMutation({
        mutationFn: (data: LoginRequest) => authApi.login(data),
        onSuccess: (response) => {
            login(response.token, response.user);
            navigate('/assets');
        },
    });
}
