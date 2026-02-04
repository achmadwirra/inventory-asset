import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
    useCategoriesQuery,
    useCreateCategoryMutation,
    useUpdateCategoryMutation,
    useDeleteCategoryMutation,
} from './categories.hooks';
import Layout from '../../shared/components/Layout';
import type { AssetCategory } from '../../types';

const categorySchema = z.object({
    name: z.string().min(1, 'Category name is required').max(100, 'Max 100 characters'),
});

type CategoryFormData = z.infer<typeof categorySchema>;

export default function CategoriesPage() {
    const { data: categories, isLoading, error } = useCategoriesQuery();
    const createMutation = useCreateCategoryMutation();
    const updateMutation = useUpdateCategoryMutation();
    const deleteMutation = useDeleteCategoryMutation();

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingCategory, setEditingCategory] = useState<AssetCategory | null>(null);

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<CategoryFormData>({
        resolver: zodResolver(categorySchema),
    });

    const openAddModal = () => {
        setEditingCategory(null);
        reset({ name: '' });
        setIsModalOpen(true);
    };

    const openEditModal = (category: AssetCategory) => {
        setEditingCategory(category);
        reset({ name: category.name });
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setIsModalOpen(false);
        setEditingCategory(null);
        reset({ name: '' });
    };

    const onSubmit = async (data: CategoryFormData) => {
        try {
            if (editingCategory) {
                await updateMutation.mutateAsync({ id: editingCategory.id, data });
            } else {
                await createMutation.mutateAsync(data);
            }
            closeModal();
        } catch (err) {
            // Error handled by mutation
        }
    };

    const handleDelete = async (category: AssetCategory) => {
        if (window.confirm(`Are you sure you want to delete "${category.name}"?`)) {
            try {
                await deleteMutation.mutateAsync(category.id);
            } catch (err) {
                alert('Cannot delete category: It may still have assets assigned.');
            }
        }
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
                    <p className="text-red-300">Failed to load categories.</p>
                </div>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="space-y-6">
                <div className="flex items-center justify-between">
                    <h1 className="text-2xl font-bold text-white">Asset Categories</h1>
                    <button
                        onClick={openAddModal}
                        className="px-4 py-2 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white font-semibold rounded-lg shadow-lg transition"
                    >
                        + Add Category
                    </button>
                </div>

                <div className="bg-white/5 backdrop-blur-lg rounded-xl border border-white/10 overflow-hidden">
                    <table className="min-w-full divide-y divide-white/10">
                        <thead className="bg-white/5">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Name
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-400 uppercase tracking-wider">
                                    Actions
                                </th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-white/10">
                            {categories?.map((category) => (
                                <tr key={category.id} className="hover:bg-white/5 transition">
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-white">
                                        {category.name}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-right space-x-3">
                                        <button
                                            onClick={() => openEditModal(category)}
                                            className="text-blue-400 hover:text-blue-300 transition"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => handleDelete(category)}
                                            disabled={deleteMutation.isPending}
                                            className="text-red-400 hover:text-red-300 transition disabled:opacity-50"
                                        >
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            ))}
                            {categories?.length === 0 && (
                                <tr>
                                    <td colSpan={2} className="px-6 py-8 text-center text-gray-400">
                                        No categories found. Add one to get started.
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
            </div>

            {/* Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50">
                    <div className="bg-slate-800 rounded-xl border border-white/10 p-6 w-full max-w-md shadow-2xl">
                        <h2 className="text-xl font-bold text-white mb-4">
                            {editingCategory ? 'Edit Category' : 'Add Category'}
                        </h2>
                        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-300 mb-2">
                                    Category Name
                                </label>
                                <input
                                    {...register('name')}
                                    type="text"
                                    className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    placeholder="Enter category name"
                                />
                                {errors.name && (
                                    <p className="mt-1 text-sm text-red-400">{errors.name.message}</p>
                                )}
                            </div>

                            {(createMutation.isError || updateMutation.isError) && (
                                <div className="bg-red-500/20 border border-red-500/50 rounded-lg p-3">
                                    <p className="text-sm text-red-300">
                                        {editingCategory
                                            ? 'Failed to update category.'
                                            : 'Failed to create category. Name may already exist.'}
                                    </p>
                                </div>
                            )}

                            <div className="flex justify-end space-x-3 pt-4">
                                <button
                                    type="button"
                                    onClick={closeModal}
                                    className="px-4 py-2 bg-white/10 hover:bg-white/20 text-white rounded-lg transition"
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    disabled={createMutation.isPending || updateMutation.isPending}
                                    className="px-4 py-2 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white font-semibold rounded-lg transition disabled:opacity-50"
                                >
                                    {createMutation.isPending || updateMutation.isPending
                                        ? 'Saving...'
                                        : editingCategory
                                            ? 'Update'
                                            : 'Create'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </Layout>
    );
}
