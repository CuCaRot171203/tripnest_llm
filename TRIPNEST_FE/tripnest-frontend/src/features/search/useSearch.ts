import { useCallback, useEffect, useMemo, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';

function useDebounce<T>(value: T, ms = 300) {
    const [v, setV] = useState(value);
    useEffect(() => {
        const t = setTimeout(() => setV(value), ms);
        return () => clearTimeout(t);
    }, [value, ms]);
    return v;
}

export type SearchFilters = {
    Q?: string;
    City?: string;
    Lat?: number;
    Lng?: number;
    Radius?: number;
    Checkin?: string;
    Checkout?: string;
    Amenties?: string[];
    MinPrice?: number;
    MaxPrice?: number;
    Sort?: string;
    Page?: number;
    PageSize?: number;
};

export function useSearch(initial?: Partial<SearchFilters>) {
    const [searchParams, setSearchParams] = useSearchParams();
    const navigate = useNavigate();

    const paramsToFilters = useCallback((): SearchFilters => {
        const out: any = { ...(initial ?? {}) };
        for (const [k, v] of searchParams.entries()) {
            if (k === 'Page' || k === 'PageSize') out[k] = parseInt(v, 10);
            else if (['MinPrice', 'MaxPrice', 'Radius', 'Lat', 'Lng'].includes(k)) out[k] = Number(v);
            else if (k === 'Amenties') out[k] = v.split(',').filter(Boolean);
            else out[k] = v;
        }
        return out;
    }, [searchParams, initial]);

    const [filters, setFilters] = useState<SearchFilters>(() => paramsToFilters());
    useEffect(() => setFilters(paramsToFilters()), [searchParams, paramsToFilters()]);

    const debouncedQ = useDebounce(filters.Q ?? '', 300);

    const setFilter = useCallback((patch: Partial<SearchFilters>, replaceUrl = false) => {
        const next = { ...filters, ...patch };
        setFilters(next);
        const sp = new URLSearchParams();
        Object.entries(next).forEach(([k, v]) => {
            if (v === undefined || v === null || v === '') return;
            if (Array.isArray(v)) sp.set(k, v.join(','));
            else sp.set(k, String(v));
        });
        if (replaceUrl) setSearchParams(sp, { replace: true });
        else {
            const qs = sp.toString();
            navigate({ pathname: '/search', search: qs ? `?${qs}` : '' }, { replace: false });
        }
    }, [filters, navigate, setSearchParams]);

    const resetFilters = useCallback(() => {
        setFilters(initial ?? {});
        setSearchParams({});
        navigate('/search', { replace: true });
    }, [initial, navigate, setSearchParams]);

    const queryArgs = useMemo(() => {
        const args: any = { ...filters, Q: debouncedQ };
        args.Page = args.Page ?? 1;
        args.PageSize = args.PageSize ?? 10;
        return args;
    }, [filters, debouncedQ]);

    return {
        filters,
        setFilter,
        resetFilters,
        queryArgs,
        debouncedQ,
    };
}
