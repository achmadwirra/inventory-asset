import { useParams } from 'react-router-dom';
import { useAssetQuery } from './assets.hooks';
import Layout from '../../shared/components/Layout';
import StatusBadge from '../../shared/ui/StatusBadge';

export default function AssetDetailPage() {
    const { id } = useParams<{ id: string }>();
    const { data: asset, isLoading, error } = useAssetQuery(id || '');

    if (isLoading) {
        return (
            <Layout>
                <div className="flex items-center justify-center h-64">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
                </div>
            </Layout>
        );
    }

    if (error || !asset) {
        return (
            <Layout>
                <div className="bg-red-500/20 border border-red-500/50 rounded-lg p-4">
                    <p className="text-red-300">Asset not found.</p>
                </div>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="bg-white/5 backdrop-blur-lg rounded-xl border border-white/10 p-6">
                <div className="flex items-center justify-between mb-6">
                    <h1 className="text-2xl font-bold text-white">{asset.name}</h1>
                    <StatusBadge status={asset.status} />
                </div>

                <dl className="grid grid-cols-2 gap-4">
                    <div>
                        <dt className="text-sm text-gray-400">Asset Code</dt>
                        <dd className="text-white font-mono">{asset.assetCode}</dd>
                    </div>
                    <div>
                        <dt className="text-sm text-gray-400">Location</dt>
                        <dd className="text-white">{asset.location}</dd>
                    </div>
                    <div>
                        <dt className="text-sm text-gray-400">Purchase Date</dt>
                        <dd className="text-white">{asset.purchaseDate}</dd>
                    </div>
                    <div>
                        <dt className="text-sm text-gray-400">Assigned To</dt>
                        <dd className="text-white">{asset.assignedToUserId || 'Not assigned'}</dd>
                    </div>
                </dl>
            </div>
        </Layout>
    );
}
