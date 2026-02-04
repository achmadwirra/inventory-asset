import api from '../../services/api';

export interface User {
    id: string;
    email: string;
    role: string;
}

export const usersApi = {
    getByRole: async (role: string): Promise<User[]> => {
        const response = await api.get<User[]>(`/users?role=${role}`);
        return response.data;
    },
};
