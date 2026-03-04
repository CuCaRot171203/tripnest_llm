import React from 'react';
import { useSearch } from './useSearch';
import { useGetPropertiesQuery } from './searchApi';
import { SearchFilters } from './SearchFilters';
import ListingCard from './ListingCard';
import Pagination from './Pagination';
import { MapPanel } from '../../shared/components/MapPanel';
import type { PropertyItem } from './type';
import { Spin, Empty } from 'antd';
import { useTheme } from '../../contexts/ThemeContext';

const MAPBOX_TOKEN = (import.meta.env.VITE_MAPBOX_TOKEN as string) ?? '';

const SearchPage: React.FC = () => {
    const { theme } = useTheme();
    const { filters, setFilter, queryArgs } = useSearch({
        Page: 1,
        PageSize: 10,
    });

    const { data, isLoading, isFetching, error } = useGetPropertiesQuery(queryArgs);

    const items: PropertyItem[] = data?.items ?? [];
    const total = data?.total ?? 0;

    return (
        <div style={{ background: theme.bg, color: theme.text }}>
            <div className="container mx-auto p-4">
                <div className="flex flex-col md:flex-row gap-4">
                    <div className="hidden md:block">
                        <SearchFilters filters={filters} setFilter={setFilter} />
                    </div>

                    <div className="flex-1">
                        <div className="mb-4 flex items-center justify-between">
                            <h2 className="text-2xl font-semibold">Search results</h2>
                            <div className="text-sm text-gray-500">{isFetching ? 'Loading...' : `${total} results`}</div>
                        </div>

                        <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
                            <div className="lg:col-span-2 space-y-3">
                                {isLoading ? (
                                    <div className="p-8 text-center"><Spin /></div>
                                ) : items.length === 0 ? (
                                    <div className="p-8"><Empty description="No results found" /></div>
                                ) : (
                                    items.map((it) => <ListingCard key={it.propertyId} item={it} />)
                                )}

                                <Pagination
                                    page={filters.Page ?? 1}
                                    pageSize={filters.PageSize ?? 10}
                                    total={total}
                                    onChange={(p) => setFilter({ Page: p })}
                                />
                            </div>

                            <div className="lg:col-span-1">
                                <div className="sticky top-20">
                                    <MapPanel
                                        token={MAPBOX_TOKEN}
                                        lat={filters.Lat !== undefined ? Number(filters.Lat) : undefined}
                                        lng={filters.Lng !== undefined ? Number(filters.Lng) : undefined}
                                        markers={items.map((i) => ({ id: String(i.propertyId), lat: i.latitude ?? 0, lng: i.longitude ?? 0, title: i.title }))}
                                        className="w-full h-96"
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                {error && <div className="mt-4 text-red-600">Error loading results. Please try again later.</div>}
            </div>
        </div>
    );
};

export default SearchPage;
