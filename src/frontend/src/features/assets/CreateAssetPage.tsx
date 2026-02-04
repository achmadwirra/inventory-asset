import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import Layout from '../../shared/components/Layout';
import { useCreateAssetMutation } from './assets.hooks';
import { useCategoriesQuery } from '../categories/categories.hooks';

const createAssetSchema = z.object({
    assetCode: z.string().min(1, 'Asset Code is required').max(50, 'Max 50 characters'),
    name: z.string().min(1, 'Name is required').max(100, 'Max 100 characters'),
    categoryId: z.string().min(1, 'Category is required'),
    purchaseDate: z.string().min(1, 'Purchase Date is required'),
    location: z.string().min(1, 'Location is required'),
});

type CreateAssetFormData = z.infer<typeof createAssetSchema>;

export default function CreateAssetPage() {
    const navigate = useNavigate();
    const createMutation = useCreateAssetMutation();
    const { data: categories } = useCategoriesQuery();

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<CreateAssetFormData>({
        resolver: zodResolver(createAssetSchema),
    });

    const onSubmit = (data: CreateAssetFormData) => {
        createMutation.mutate(data, {
            onSuccess: () => {
                alert('Asset berhasil dibuat');
                navigate('/assets');
            },
        });
    };

    return (
        <Layout>
            <div className="max-w-2xl mx-auto space-y-6">
                <div className="flex items-center justify-between">
                    <h1 className="text-2xl font-bold text-white">Create New Asset</h1>
                    <button
                        onClick={() => navigate('/assets')}
                        className="text-gray-400 hover:text-white transition"
                    >
                        Cancel
                    </button>
                </div>

                <div className="bg-slate-800 rounded-xl border border-white/10 p-6 shadow-2xl">
                    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                        {/* Asset Code */}
                        <div>
                            <label className="block text-sm font-medium text-gray-300 mb-2">
                                Asset Code
                            </label>
                            <input
                                {...register('assetCode')}
                                type="text"
                                className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="e.g. LAP-001"
                            />
                            {errors.assetCode && (
                                <p className="mt-1 text-sm text-red-400">{errors.assetCode.message}</p>
                            )}
                        </div>

                        {/* Name */}
                        <div>
                            <label className="block text-sm font-medium text-gray-300 mb-2">
                                Asset Name
                            </label>
                            <input
                                {...register('name')}
                                type="text"
                                className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="e.g. Dell Latitude 7420"
                            />
                            {errors.name && (
                                <p className="mt-1 text-sm text-red-400">{errors.name.message}</p>
                            )}
                        </div>

                        {/* Category */}
                        <div>
                            <label className="block text-sm font-medium text-gray-300 mb-2">
                                Category
                            </label>
                            <select
                                {...register('categoryId')}
                                className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            >
                                <option value="">Select Category</option>
                                {categories?.map((category) => (
                                    <option key={category.id} value={category.id}>
                                        {category.name}
                                    </option>
                                ))}
                            </select>
                            {errors.categoryId && (
                                <p className="mt-1 text-sm text-red-400">{errors.categoryId.message}</p>
                            )}
                        </div>

                        {/* Purchase Date */}
                        <div>
                            <label className="block text-sm font-medium text-gray-300 mb-2">
                                Purchase Date
                            </label>
                            <input
                                {...register('purchaseDate')}
                                type="date"
                                className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            {errors.purchaseDate && (
                                <p className="mt-1 text-sm text-red-400">{errors.purchaseDate.message}</p>
                            )}
                        </div>

                        {/* Location */}
                        <div>
                            <label className="block text-sm font-medium text-gray-300 mb-2">
                                Location
                            </label>
                            <input
                                {...register('location')}
                                type="text"
                                className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                placeholder="e.g. Head Office, Jakarta"
                            />
                            {errors.location && (
                                <p className="mt-1 text-sm text-red-400">{errors.location.message}</p>
                            )}
                        </div>

                        {/* Error Message */}
                        {createMutation.isError && (
                            <div className="bg-red-500/20 border border-red-500/50 rounded-lg p-3">
                                <p className="text-sm text-red-300">
                                    {(createMutation.error as any)?.response?.data?.message || 'Failed to create asset. Code might duplicate.'}
                                </p>
                            </div>
                        )}

                        {/* Actions */}
                        <div className="flex justify-end pt-4">
                            <button
                                type="submit"
                                disabled={createMutation.isPending}
                                className="px-6 py-3 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white font-semibold rounded-lg shadow-lg transition disabled:opacity-50"
                            >
                                {createMutation.isPending ? 'Creating...' : 'Create Asset'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </Layout>
    );
}
