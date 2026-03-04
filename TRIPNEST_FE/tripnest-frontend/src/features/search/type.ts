export interface PropertyItem {
    propertyId: number;
    title: string;
    address?: string;
    city?: string;
    priceBase?: number;
    latitude?: number;
    longitude?: number;
    thumbnailUrl?: string;
    distanceKm?: number;
    minCapacity?: number;
}

export interface PropertiesResponse {
    items: PropertyItem[];
    total: number;
    page: number;
    pageSize: number;
}

export interface SuggestionItem {
    text: string;
    type?: string;
    propertyId?: number;
}