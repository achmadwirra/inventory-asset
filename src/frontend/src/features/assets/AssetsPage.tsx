import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAssetsQuery, useReturnAssetMutation } from './assets.hooks';
import { AssetStatus } from '../../types';
import Layout from '../../shared/components/Layout';
import StatusBadge from '../../shared/ui/StatusBadge';
import AssignAssetModal from './AssignAssetModal';
import type { Asset } from '../../types';
import { useAuthStore } from '../../stores/auth.store';

export default function AssetsPage() {
    const { data: assets, isLoading, error } = useAssetsQuery();
    const returnMutation = useReturnAssetMutation();
    const [selectedAsset, setSelectedAsset] = useState<Asset | null>(null);
    const [statusFilter, setStatusFilter] = useState<AssetStatus | 'all'>('all');
    const navigate = useNavigate();
    const { user } = useAuthStore();

    const canCreateAsset = user?.role === 'Admin' || user?.role === 'ITStaff';

    const handleReturn = (asset: Asset) => {
        if (window.confirm(`Are you sure you want to return asset "${asset.name}"?`)) {
            returnMutation.mutate(asset.id);
        }
    };

    // Helper to normalize status for comparison
    const normalizeStatus = (status: AssetStatus | string): AssetStatus => {
        if (typeof status === 'string') {
            const stringToEnum: Record<string, AssetStatus> = {
                'InStock': AssetStatus.InStock,
                'Assigned': AssetStatus.Assigned,
                'Maintenance': AssetStatus.Maintenance,
                'Retired': AssetStatus.Retired,
            };
            return stringToEnum[status] ?? AssetStatus.InStock;
        }
        return status;
    };

    const filteredAssets = assets?.filter((asset) =>
        statusFilter === 'all' ? true : normalizeStatus(asset.status) === statusFilter
    );

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
                    <p className="text-red-300">Failed to load assets. Please try again.</p>
                </div>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="space-y-6">
                <div className="flex items-center justify-between">
                    <h1 className="text-2xl font-bold text-white">Assets</h1>
                    <div className="flex space-x-4">
                        <select
                            value={statusFilter}
                            onChange={(e) => setStatusFilter(e.target.value as AssetStatus | 'all')}
                            className="bg-white/5 border border-white/10 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
                        >
                            <option value="all">All Status</option>
                            <option value={AssetStatus.InStock}>In Stock</option>
                            <option value={AssetStatus.Assigned}>Assigned</option>
                            <option value={AssetStatus.Maintenance}>Maintenance</option>
                            <option value={AssetStatus.Retired}>Retired</option>
                        </select>
                        {canCreateAsset && (
                            <button
                                onClick={() => navigate('/assets/new')}
                                className="px-4 py-2 bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800 text-white font-semibold rounded-lg shadow-lg transition"
                            >
                                + Add Asset
                            </button>
                        )}
                    </div>
                </div>

                <div className="bg-white/5 backdrop-blur-lg rounded-xl border border-white/10 overflow-hidden">
                    <table className="min-w-full divide-y divide-white/10">
                        <thead className="bg-white/5">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Asset Code
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Name
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Location
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Status
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Actions
                                </th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-white/10">
                            {filteredAssets?.map((asset) => (
                                <tr key={asset.id} className="hover:bg-white/5 transition">
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-white font-mono">
                                        {asset.assetCode}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-white">
                                        {asset.name}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">
                                        {asset.location}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <StatusBadge status={asset.status} />
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm space-x-2">
                                        {normalizeStatus(asset.status) === AssetStatus.InStock && canCreateAsset && (
                                            <button
                                                onClick={() => setSelectedAsset(asset)}
                                                className="text-blue-400 hover:text-blue-300 transition"
                                            >
                                                Assign
                                            </button>
                                        )}
                                        {normalizeStatus(asset.status) === AssetStatus.Assigned && (
                                            <button
                                                onClick={() => handleReturn(asset)}
                                                disabled={returnMutation.isPending}
                                                className="text-yellow-400 hover:text-yellow-300 transition disabled:opacity-50"
                                            >
                                                Return
                                            </button>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {selectedAsset && (
                <AssignAssetModal
                    asset={selectedAsset}
                    onClose={() => setSelectedAsset(null)}
                />
            )}
        </Layout>
    );
}
