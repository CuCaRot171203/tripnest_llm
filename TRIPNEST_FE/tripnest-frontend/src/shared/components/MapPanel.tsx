import React, { useEffect, useRef } from 'react';
import mapboxgl from 'mapbox-gl';

type Props = {
    lat?: number;
    lng?: number;
    markers?: { id: string; lat: number; lng: number; title?: string }[];
    token?: string;
    className?: string;
    style?: React.CSSProperties;
};

export const MapPanel: React.FC<Props> = ({ lat = 10.762622, lng = 106.660172, markers = [], token, className, style }) => {
    const ref = useRef<HTMLDivElement | null>(null);
    const mapRef = useRef<mapboxgl.Map | null>(null);

    useEffect(() => {
        if (!token) {
            console.warn('MapPanel: no mapbox token provided.');
            return;
        }
        mapboxgl.accessToken = token;
        if (!ref.current) return;

        mapRef.current = new mapboxgl.Map({
            container: ref.current,
            style: 'mapbox://styles/mapbox/streets-v11',
            center: [lng, lat],
            zoom: 11,
        });

        mapRef.current.addControl(new mapboxgl.NavigationControl({ visualizePitch: true }));
        return () => { mapRef.current?.remove(); mapRef.current = null; };
    }, [token]);

    useEffect(() => {
        if (!mapRef.current) return;
        // In production, manage marker refs to remove before adding; for MVP we add markers.
        markers.forEach((m) => {
            const el = document.createElement('div');
            el.style.width = '14px';
            el.style.height = '14px';
            el.style.borderRadius = '50%';
            el.style.background = '#2563eb';
            new mapboxgl.Marker(el).setLngLat([m.lng, m.lat]).setPopup(new mapboxgl.Popup().setText(m.title ?? '')).addTo(mapRef.current!);
        });
        if (markers.length === 0 && lat && lng) mapRef.current.setCenter([lng, lat]);
        else if (markers.length > 0) {
            const bounds = new mapboxgl.LngLatBounds();
            markers.forEach((m) => bounds.extend([m.lng, m.lat]));
            mapRef.current.fitBounds(bounds, { padding: 40, maxZoom: 14 });
        }
    }, [markers, lat, lng]);

    return <div ref={ref} className={className ?? 'w-full h-64 rounded-md'} style={style} />;
};