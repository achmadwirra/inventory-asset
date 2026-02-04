import { AssetStatus } from '../../types';

interface StatusBadgeProps {
    status: AssetStatus | string;
}

const statusConfig: Record<AssetStatus, { bg: string; text: string; label: string }> = {
    [AssetStatus.InStock]: {
        bg: 'bg-green-500/20',
        text: 'text-green-400',
        label: 'In Stock',
    },
    [AssetStatus.Assigned]: {
        bg: 'bg-blue-500/20',
        text: 'text-blue-400',
        label: 'Assigned',
    },
    [AssetStatus.Maintenance]: {
        bg: 'bg-yellow-500/20',
        text: 'text-yellow-400',
        label: 'Maintenance',
    },
    [AssetStatus.Retired]: {
        bg: 'bg-gray-500/20',
        text: 'text-gray-400',
        label: 'Retired',
    },
};

// String to enum mapping for backend returning string status
const stringToEnum: Record<string, AssetStatus> = {
    'InStock': AssetStatus.InStock,
    'Assigned': AssetStatus.Assigned,
    'Maintenance': AssetStatus.Maintenance,
    'Retired': AssetStatus.Retired,
};

export default function StatusBadge({ status }: StatusBadgeProps) {
    // Handle both numeric enum and string status
    let normalizedStatus: AssetStatus;
    if (typeof status === 'string') {
        normalizedStatus = stringToEnum[status] ?? AssetStatus.InStock;
    } else {
        normalizedStatus = status;
    }

    const config = statusConfig[normalizedStatus] ?? statusConfig[AssetStatus.InStock];

    return (
        <span
            className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${config.bg} ${config.text}`}
        >
            {config.label}
        </span>
    );
}
