import api from '../../services/api';
import type { AuditLog, PaginatedResult } from '../../types';

export interface AuditLogFilters {
    page?: number;
    pageSize?: number;
    action?: string;
}

export const auditLogsApi = {
    getAll: async (filters: AuditLogFilters = {}): Promise<PaginatedResult<AuditLog>> => {
        const params = new URLSearchParams();
        if (filters.page) params.append('page', filters.page.toString());
        if (filters.pageSize) params.append('pageSize', filters.pageSize.toString());
        if (filters.action) params.append('action', filters.action);

        const response = await api.get<PaginatedResult<AuditLog>>(`/audit-logs?${params.toString()}`);
        return response.data;
    },

    getDistinctActions: async (): Promise<string[]> => {
        const response = await api.get<string[]>('/audit-logs/actions');
        return response.data;
    },
};
