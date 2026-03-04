import React from 'react';
import type { PropertyItem } from './types';
import { Card } from 'antd';
import { useTheme } from '../../contexts/ThemeContext';
import { Link } from 'react-router-dom';

type Props = { item: PropertyItem };

export const ListingCard: React.FC<Props> = ({ item }) => {
    const { theme } = useTheme();
    const cover = (
        <div className="w-full h-40 overflow-hidden bg-gray-100 flex items-center justify-center">
            <img src={item.thumbnailUrl ?? '/placeholder.png'} alt={item.title} className="w-full h-full object-cover" />
        </div>
    );

    return (
        <Card
            hoverable
            cover={cover}
            style={{ background: theme.card, borderColor: theme.border }}
            bodyStyle={{ padding: 12, color: theme.text }}
        >
            <div className="flex justify-between items-start gap-4">
                <div>
                    <Link to={`/properties/${item.propertyId}`} className="text-lg font-semibold hover:underline" style={{ color: theme.text }}>
                        {item.title}
                    </Link>
                    <div className="text-sm text-gray-500">{item.city} {item.address ? `• ${item.address}` : ''}</div>
                </div>

                <div className="text-right">
                    <div style={{ fontWeight: 700 }}>
                        {item.priceBase ? `${item.priceBase.toLocaleString()} VND` : '-'}
                    </div>
                    <div className="text-sm text-gray-500">{item.distanceKm ? `${item.distanceKm} km` : null}</div>
                </div>
            </div>

            <div className="mt-3 text-xs text-gray-500">
                {item.minCapacity ? `${item.minCapacity} guests` : ''}
            </div>
        </Card>
    );
};

export default ListingCard;
