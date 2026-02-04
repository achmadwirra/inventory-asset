import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../stores/auth.store';
import type { ReactNode } from 'react';

interface LayoutProps {
    children: ReactNode;
}

export default function Layout({ children }: LayoutProps) {
    const { user, logout } = useAuthStore();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const canViewAuditLogs = user?.role === 'Admin' || user?.role === 'ITStaff';

    return (
        <div className="min-h-screen bg-gradient-to-br from-slate-900 to-slate-800">
            <nav className="bg-white/5 backdrop-blur-lg border-b border-white/10">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex items-center justify-between h-16">
                        <div className="flex items-center space-x-8">
                            <span className="text-xl font-bold text-white">IT Asset</span>
                            <div className="flex space-x-4">
                                <Link
                                    to="/assets"
                                    className="text-gray-300 hover:text-white px-3 py-2 rounded-md text-sm font-medium transition"
                                >
                                    Assets
                                </Link>
                                {canViewAuditLogs && (
                                    <>
                                        <Link
                                            to="/categories"
                                            className="text-gray-300 hover:text-white px-3 py-2 rounded-md text-sm font-medium transition"
                                        >
                                            Categories
                                        </Link>
                                        <Link
                                            to="/audit-logs"
                                            className="text-gray-300 hover:text-white px-3 py-2 rounded-md text-sm font-medium transition"
                                        >
                                            Audit Logs
                                        </Link>
                                    </>
                                )}
                            </div>
                        </div>
                        <div className="flex items-center space-x-4">
                            <span className="text-sm text-gray-400">
                                {user?.email} ({user?.role})
                            </span>
                            <button
                                onClick={handleLogout}
                                className="text-gray-300 hover:text-white px-3 py-2 rounded-md text-sm font-medium transition"
                            >
                                Logout
                            </button>
                        </div>
                    </div>
                </div>
            </nav>
            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                {children}
            </main>
        </div>
    );
}
