import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loginSchema, type LoginFormData } from '../../shared/utils/schemas';
import { useLogin } from './auth.hooks';

export default function LoginPage() {
    const loginMutation = useLogin();

    const {
        register,
        handleSubmit,
        formState: { errors, isSubmitting },
    } = useForm<LoginFormData>({
        resolver: zodResolver(loginSchema),
    });

    const onSubmit = (data: LoginFormData) => {
        loginMutation.mutate(data);
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 to-slate-800">
            <div className="bg-white/10 backdrop-blur-lg rounded-2xl shadow-2xl p-8 w-full max-w-md border border-white/20">
                <h1 className="text-3xl font-bold text-white text-center mb-8">
                    IT Asset Management
                </h1>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div>
                        <label className="block text-sm font-medium text-gray-300 mb-2">
                            Email
                        </label>
                        <input
                            {...register('email')}
                            type="email"
                            className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition"
                            placeholder="Enter your email"
                        />
                        {errors.email && (
                            <p className="mt-1 text-sm text-red-400">{errors.email.message}</p>
                        )}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-300 mb-2">
                            Password
                        </label>
                        <input
                            {...register('password')}
                            type="password"
                            className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition"
                            placeholder="Enter your password"
                        />
                        {errors.password && (
                            <p className="mt-1 text-sm text-red-400">{errors.password.message}</p>
                        )}
                    </div>

                    {loginMutation.isError && (
                        <div className="bg-red-500/20 border border-red-500/50 rounded-lg p-3">
                            <p className="text-sm text-red-300">Invalid email or password</p>
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={isSubmitting || loginMutation.isPending}
                        className="w-full py-3 px-4 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white font-semibold rounded-lg shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {loginMutation.isPending ? 'Signing in...' : 'Sign In'}
                    </button>
                </form>
            </div>
        </div>
    );
}
