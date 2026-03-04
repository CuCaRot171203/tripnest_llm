import React, { lazy, Suspense } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import HomePage from '../pages/HomePage';
import LoginPage from '../features/auth/LoginPage';
import RegisterPage from '../features/auth/RegisterPage';
import ResetPasswordPage from '../features/auth/ResetPasswordPage';
import ResetPasswordConfirmPage from '../features/auth/ResetPasswordConfirmPage';

// lazy-loaded feature pages
const SearchPage = lazy(() => import('../features/search/SearchPage'));
const PropertyDetailPage = lazy(
    () =>
        import('../features/property/PropertyDetailPage').catch(() => ({
            default: () => <div>Property detail not implemented</div>,
        }))
);

export default function AppRoutes() {
    return (
        <Suspense fallback={<div>Loading Page...</div>}>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/about" element={<div>Dang o trang about</div>} />

                {/* Auth */}
                <Route path="/auth/login" element={<LoginPage />} />
                <Route path="/auth/register" element={<RegisterPage />} />
                <Route path="/auth/reset-password" element={<ResetPasswordPage />} />
                <Route
                    path="/auth/reset-password/confirm/:token?"
                    element={<ResetPasswordConfirmPage />}
                />

                {/* Search & Listing */}
                <Route path="/search" element={<SearchPage />} />

                {/* Property detail (lazy, with fallback if missing) */}
                <Route path="/properties/:id" element={<PropertyDetailPage />} />

                {/* Fallback */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Suspense>
    );
}