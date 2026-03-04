import React from 'react';
import { useTheme } from '../../contexts/ThemeContext';

type Props = {
    page: number;
    pageSize: number;
    total: number;
    onChange: (page: number) => void;
};

export const Pagination: React.FC<Props> = ({ page, pageSize, total, onChange }) => {
    const totalPages = Math.max(1, Math.ceil(total / pageSize));
    const pages: number[] = [];
    const start = Math.max(1, page - 2);
    const end = Math.min(totalPages, page + 2);
    for (let p = start; p <= end; p++) pages.push(p);
    const { theme } = useTheme();

    return (
        <div className="flex items-center gap-2 mt-4">
            <button onClick={() => onChange(Math.max(1, page - 1))} disabled={page <= 1} className="px-3 py-1 border rounded disabled:opacity-50" style={{ borderColor: theme.border }}>Prev</button>

            {start > 1 && <span className="px-2">...</span>}
            {pages.map((p) => (
                <button
                    key={p}
                    className={`px-3 py-1 border rounded ${p === page ? 'bg-indigo-600 text-white' : ''}`}
                    onClick={() => onChange(p)}
                    style={{ borderColor: theme.border }}
                >
                    {p}
                </button>
            ))}
            {end < totalPages && <span className="px-2">...</span>}

            <button onClick={() => onChange(Math.min(totalPages, page + 1))} disabled={page >= totalPages} className="px-3 py-1 border rounded disabled:opacity-50" style={{ borderColor: theme.border }}>Next</button>

            <div className="ml-auto text-sm text-gray-600">
                {Math.min((page - 1) * pageSize + 1, total)} - {Math.min(page * pageSize, total)} of {total}
            </div>
        </div>
    );
};

export default Pagination;
