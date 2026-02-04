import { z } from 'zod';

export const loginSchema = z.object({
    email: z.string().email('Invalid email address'),
    password: z.string().min(6, 'Password must be at least 6 characters'),
});

export const createAssetSchema = z.object({
    assetCode: z.string().min(1, 'Asset code is required').max(50),
    name: z.string().min(1, 'Name is required').max(100),
    categoryId: z.string().uuid('Invalid category'),
    purchaseDate: z.string().min(1, 'Purchase date is required'),
    location: z.string().min(1, 'Location is required'),
});

export const assignAssetSchema = z.object({
    userId: z.string().min(1, 'Pilih user terlebih dahulu').regex(/^[0-9a-fA-F-]{36}$/, 'User tidak valid'),
});

export type LoginFormData = z.infer<typeof loginSchema>;
export type CreateAssetFormData = z.infer<typeof createAssetSchema>;
export type AssignAssetFormData = z.infer<typeof assignAssetSchema>;
