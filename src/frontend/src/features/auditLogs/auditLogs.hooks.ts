import { useQuery } from '@tanstack/react-query';
import { auditLogsApi, type AuditLogFilters } from './auditLogs.api';

export function useAuditLogsQuery(filters: AuditLogFilters = {}) {
    return useQuery({
        queryKey: ['audit-logs', filters],
        queryFn: () => auditLogsApi.getAll(filters),
    });
}

export function useAuditLogActionsQuery() {
    return useQuery({
        queryKey: ['audit-log-actions'],
        queryFn: auditLogsApi.getDistinctActions,
    });
}
