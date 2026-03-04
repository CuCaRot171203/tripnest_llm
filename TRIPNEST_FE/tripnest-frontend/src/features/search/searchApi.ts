import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { PropertiesResponse, SuggestionItem } from './type';

export const searchApi = createApi({
    reducerPath: 'searchApi',
    baseQuery: fetchBaseQuery({ baseUrl: '/api' }),
    endpoints: (builder) => ({
        getProperties: builder.query<PropertiesResponse, Record<string, any>>({
            query: (args) => {
                const params = new URLSearchParams();
                Object.entries(args || {}).forEach(([k, v]) => {
                    if (v === undefined || v === null || v === '') return;
                    if (Array.isArray(v)) params.set(k, v.join(','));
                    else params.set(k, String(v));
                });
                return { url: `/Search?${params.toString()}`, method: 'GET' };
            },
        }),

        getSuggestions: builder.query<SuggestionItem[], { q: string; limit?: number }>({
            query: ({ q, limit }) => {
                const params = new URLSearchParams();
                if (q) params.set('q', q);
                if (limit !== undefined) params.set('limit', String(limit));
                return `/Search/suggestions?${params.toString()}`;
            },
        }),

        semanticSearch: builder.mutation<PropertiesResponse, { text: string; filters?: Record<string, any>; topK?: number }>({
            query: (body) => ({
                url: '/Search/semantic',
                method: 'POST',
                body,
            }),
        }),
    }),
});

export const {
    useGetPropertiesQuery,
    useGetSuggestionsQuery,
    useSemanticSearchMutation,
} = searchApi;

export default searchApi;
