import { useState } from 'react';
import { useAuditLogsQuery, useAuditLogActionsQuery } from './auditLogs.hooks';
import Layout from '../../shared/components/Layout';

export default function AuditLogsPage() {
    const [page, setPage] = useState(1);
    const [actionFilter, setActionFilter] = useState<string>('');
    const pageSize = 10;

    const { data, isLoading, error } = useAuditLogsQuery({
        page,
        pageSize,
        action: actionFilter || undefined,
    });

    const { data: actions } = useAuditLogActionsQuery();

    const handlePrevPage = () => {
        if (page > 1) setPage(page - 1);
    };

    const handleNextPage = () => {
        if (data && page < data.totalPages) setPage(page + 1);
    };

    const handleFilterChange = (action: string) => {
        setActionFilter(action);
        setPage(1); // Reset to first page when filter changes
    };

    if (isLoading) {
        return (
            <Layout>
                <div className="flex items-center justify-center h-64">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
                </div>
            </Layout>
        );
    }

    if (error) {
        return (
            <Layout>
                <div className="bg-red-500/20 border border-red-500/50 rounded-lg p-4">
                    <p className="text-red-300">Failed to load audit logs. Please try again.</p>
                </div>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="space-y-6">
                <div className="flex items-center justify-between">
                    <h1 className="text-2xl font-bold text-white">Audit Logs</h1>
                    <select
                        value={actionFilter}
                        onChange={(e) => handleFilterChange(e.target.value)}
                        className="bg-white/5 border border-white/10 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                        <option value="">All Actions</option>
                        {actions?.map((action) => (
                            <option key={action} value={action}>
                                {action}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="bg-white/5 backdrop-blur-lg rounded-xl border border-white/10 overflow-hidden">
                    <table className="min-w-full divide-y divide-white/10">
                        <thead className="bg-white/5">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Timestamp
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Action
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Entity
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Details
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Performed By
                                </th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-white/10">
                            {data?.items.map((log) => (
                                <tr key={log.id} className="hover:bg-white/5 transition">
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">
                                        {new Date(log.timestamp).toLocaleString('id-ID')}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <ActionBadge action={log.action} />
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-white">
                                        {log.entityName}
                                    </td>
                                    <td className="px-6 py-4 text-sm text-gray-300 max-w-xs truncate" title={log.newValue}>
                                        {log.newValue || '-'}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300 font-mono">
                                        {log.performedByUserId.substring(0, 8)}...
                                    </td>
                                </tr>
                            ))}
                            {data?.items.length === 0 && (
                                <tr>
                                    <td colSpan={5} className="px-6 py-8 text-center text-gray-400">
                                        No audit logs found.
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>

                {/* Pagination */}
                {data && data.totalPages > 1 && (
                    <div className="flex items-center justify-between bg-white/5 backdrop-blur-lg rounded-xl border border-white/10 px-6 py-3">
                        <div className="text-sm text-gray-400">
                            Showing {(page - 1) * pageSize + 1} to {Math.min(page * pageSize, data.totalItems)} of {data.totalItems} results
                        </div>
                        <div className="flex items-center space-x-2">
                            <button
                                onClick={handlePrevPage}
                                disabled={page === 1}
                                className="px-3 py-1 rounded-lg bg-white/10 text-white hover:bg-white/20 disabled:opacity-50 disabled:cursor-not-allowed transition"
                            >
                                Previous
                            </button>
                            <span className="text-white px-3">
                                Page {page} of {data.totalPages}
                            </span>
                            <button
                                onClick={handleNextPage}
                                disabled={page === data.totalPages}
                                className="px-3 py-1 rounded-lg bg-white/10 text-white hover:bg-white/20 disabled:opacity-50 disabled:cursor-not-allowed transition"
                            >
                                Next
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </Layout>
    );
}

function ActionBadge({ action }: { action: string }) {
    const getColorClasses = (action: string) => {
        switch (action.toLowerCase()) {
            case 'create':
                return 'bg-green-500/20 text-green-400';
            case 'assign':
                return 'bg-blue-500/20 text-blue-400';
            case 'return':
                return 'bg-yellow-500/20 text-yellow-400';
            case 'update':
                return 'bg-purple-500/20 text-purple-400';
            case 'delete':
                return 'bg-red-500/20 text-red-400';
            default:
                return 'bg-gray-500/20 text-gray-400';
        }
    };

    return (
        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getColorClasses(action)}`}>
            {action}
        </span>
    );
}
