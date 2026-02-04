import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuthStore } from '../stores/auth.store';
import LoginPage from '../features/auth/LoginPage';
import AssetsPage from '../features/assets/AssetsPage';
import AssetDetailPage from '../features/assets/AssetDetailPage';
import CreateAssetPage from '../features/assets/CreateAssetPage';
import AuditLogsPage from '../features/auditLogs/AuditLogsPage';
import CategoriesPage from '../features/categories/CategoriesPage';
import type { ReactNode } from 'react';

interface ProtectedRouteProps {
    children: ReactNode;
    allowedRoles?: string[];
}

function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
    const { isAuthenticated, user } = useAuthStore();

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    if (allowedRoles && user && !allowedRoles.includes(user.role)) {
        return <Navigate to="/assets" replace />;
    }

    return <>{children}</>;
}

export function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route
                    path="/assets"
                    element={
                        <ProtectedRoute>
                            <AssetsPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/assets/new"
                    element={
                        <ProtectedRoute allowedRoles={['Admin', 'ITStaff']}>
                            <CreateAssetPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/assets/:id"
                    element={
                        <ProtectedRoute>
                            <AssetDetailPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/categories"
                    element={
                        <ProtectedRoute allowedRoles={['Admin', 'ITStaff']}>
                            <CategoriesPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/audit-logs"
                    element={
                        <ProtectedRoute allowedRoles={['Admin', 'ITStaff']}>
                            <AuditLogsPage />
                        </ProtectedRoute>
                    }
                />
                <Route path="*" element={<Navigate to="/assets" replace />} />
            </Routes>
        </BrowserRouter>
    );
}
