import React, { useMemo, useState } from 'react';
import { InputNumber, Button, Divider } from 'antd';
import type { SearchFilters } from './useSearch';
import { useGetSuggestionsQuery } from './searchApi';
import SearchBar from '../../shared/components/SearchBar';
import { useTheme } from '../../contexts/ThemeContext';
import { useTranslation } from '../../contexts/LanguageContext';

type Props = {
    filters: SearchFilters;
    setFilter: (patch: Partial<SearchFilters>) => void;
};

export const SearchFilters: React.FC<Props> = ({ filters, setFilter }) => {
    const [q, setQ] = useState(filters.Q ?? '');
    const { data: suggestions } = useGetSuggestionsQuery({ q, limit: 6 }, { skip: !q });
    const { theme } = useTheme();
    const t = useTranslation('components/searchFilters');

    const amenitiesList = useMemo(
        () => [
            { id: 'wifi', label: 'Wi-Fi' },
            { id: 'kitchen', label: 'Kitchen' },
            { id: 'parking', label: 'Parking' },
            { id: 'pool', label: 'Pool' },
        ],
        []
    );

    return (
        <aside style={{ background: theme.card, border: `1px solid ${theme.border}`, padding: 12, borderRadius: 8 }}>
            <div className="mb-3">
                <SearchBar
                    value={q}
                    onChange={(v) => { setQ(v); setFilter({ Q: v, Page: 1 }); }}
                    onSubmit={() => setFilter({ Q: q, Page: 1 })}
                    placeholder={t('placeholder') || 'Search...'}
                />
                {q && suggestions && suggestions.length > 0 && (
                    <ul className="mt-2 border rounded-md overflow-hidden" style={{ background: theme.bg }}>
                        {suggestions.map((s, i) => (
                            <li
                                key={`${s.text}-${i}`}
                                className="px-3 py-2 hover:bg-gray-50 cursor-pointer"
                                onClick={() => {
                                    if (s.type === 'city') setFilter({ City: s.text, Q: '', Page: 1 });
                                    else setFilter({ Q: s.text, Page: 1 });
                                    setQ('');
                                }}
                            >
                                {s.text} {s.type ? <span className="text-xs text-gray-400">({s.type})</span> : null}
                            </li>
                        ))}
                    </ul>
                )}
            </div>

            <Divider />

            <div className="mb-3">
                <label className="block text-sm mb-1">City</label>
                <input
                    placeholder="City..."
                    value={filters.City ?? ''}
                    onChange={(e) => setFilter({ City: e.target.value, Page: 1 })}
                    className="w-full px-3 py-2 border rounded-md"
                    style={{ background: theme.bg, color: theme.text, borderColor: theme.border }}
                />
            </div>

            <div className="mb-3 grid grid-cols-2 gap-2">
                <div>
                    <label className="block text-sm mb-1">Min price</label>
                    <InputNumber
                        style={{ width: '100%' }}
                        min={0}
                        value={filters.MinPrice ?? undefined}
                        onChange={(v) => setFilter({ MinPrice: v ?? undefined, Page: 1 })}
                        placeholder="Min"
                    />
                </div>
                <div>
                    <label className="block text-sm mb-1">Max price</label>
                    <InputNumber
                        style={{ width: '100%' }}
                        min={0}
                        value={filters.MaxPrice ?? undefined}
                        onChange={(v) => setFilter({ MaxPrice: v ?? undefined, Page: 1 })}
                        placeholder="Max"
                    />
                </div>
            </div>

            <div className="mb-3">
                <label className="block text-sm mb-2">Amenities</label>
                <div className="flex flex-wrap gap-2">
                    {amenitiesList.map((a) => {
                        const checked = (filters.Amenties || []).includes(a.id);
                        return (
                            <button
                                key={a.id}
                                type="button"
                                onClick={() => {
                                    const next = new Set(filters.Amenties || []);
                                    if (next.has(a.id)) next.delete(a.id);
                                    else next.add(a.id);
                                    setFilter({ Amenties: Array.from(next), Page: 1 });
                                }}
                                className={`px-3 py-1 rounded-full border ${checked ? 'bg-indigo-600 text-white' : 'bg-white'}`}
                                style={{ borderColor: theme.border }}
                            >
                                {a.label}
                            </button>
                        );
                    })}
                </div>
            </div>

            <div className="flex gap-2">
                <Button onClick={() => setFilter({ Sort: 'priceAsc', Page: 1 })}>Sort price ↑</Button>
                <Button onClick={() => setFilter({ Sort: 'priceDesc', Page: 1 })}>Sort price ↓</Button>
            </div>
        </aside>
    );
};
