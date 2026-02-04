import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { assignAssetSchema, type AssignAssetFormData } from '../../shared/utils/schemas';
import { useAssignAssetMutation } from './assets.hooks';
import { useUsersByRoleQuery } from '../users/users.hooks';
import type { Asset } from '../../types';

interface AssignAssetModalProps {
    asset: Asset;
    onClose: () => void;
}

export default function AssignAssetModal({ asset, onClose }: AssignAssetModalProps) {
    const assignMutation = useAssignAssetMutation();
    const { data: employees, isLoading: isLoadingUsers } = useUsersByRoleQuery('Employee');

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<AssignAssetFormData>({
        resolver: zodResolver(assignAssetSchema),
    });

    const onSubmit = (data: AssignAssetFormData) => {
        if (!window.confirm(`Are you sure you want to assign "${asset.name}" to this user?`)) {
            return;
        }
        assignMutation.mutate(
            { id: asset.id, data: { assetId: asset.id, userId: data.userId } },
            {
                onSuccess: () => {
                    alert('Asset berhasil di-assign');
                    onClose();
                },
            }
        );
    };

    return (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50">
            <div className="bg-slate-800 rounded-xl border border-white/10 p-6 w-full max-w-md shadow-2xl">
                <h2 className="text-xl font-bold text-white mb-4">
                    Assign Asset: {asset.name}
                </h2>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-300 mb-2">
                            Select Employee
                        </label>
                        <select
                            {...register('userId')}
                            defaultValue=""
                            className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
                            disabled={isLoadingUsers}
                        >
                            <option value="">-- Select Employee --</option>
                            {employees?.map((user: any) => {
                                // Robust property discovery
                                const keys = Object.keys(user);
                                const idKey = keys.find(k => k.toLowerCase() === 'id');
                                const emailKey = keys.find(k => k.toLowerCase() === 'email');

                                const userId = idKey ? String(user[idKey]) : "";
                                const userEmail = emailKey ? String(user[emailKey]) : "Unknown";

                                return (
                                    <option key={userId} value={userId}>
                                        {userEmail}
                                    </option>
                                );
                            })}
                        </select>
                        {errors.userId && (
                            <p className="mt-1 text-sm text-red-400">{errors.userId.message}</p>
                        )}
                        {isLoadingUsers && <p className="text-xs text-gray-400 mt-1">Loading employees...</p>}
                    </div>

                    {assignMutation.isError && (
                        <div className="bg-red-500/20 border border-red-500/50 rounded-lg p-3">
                            <p className="text-sm text-red-300">
                                {(assignMutation.error as any)?.response?.data?.message || 'Failed to assign asset'}
                            </p>
                        </div>
                    )}

                    <div className="flex justify-end space-x-3 pt-4">
                        <button
                            type="button"
                            onClick={onClose}
                            className="px-4 py-2 bg-white/10 hover:bg-white/20 text-white rounded-lg transition"
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={assignMutation.isPending || isLoadingUsers}
                            className="px-4 py-2 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white font-semibold rounded-lg shadow-lg transition disabled:opacity-50"
                        >
                            {assignMutation.isPending ? 'Assigning...' : 'Assign Asset'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
