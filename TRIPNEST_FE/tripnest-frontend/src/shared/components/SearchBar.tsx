import React from 'react';
import { Input, Button } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import { useTheme } from '../../contexts/ThemeContext';

type Props = {
    value?: string;
    placeholder?: string;
    onChange?: (v: string) => void;
    onSubmit?: () => void;
    className?: string;
};

export const SearchBar: React.FC<Props> = ({ value = '', placeholder = 'Search...', onChange, onSubmit, className }) => {
    const { theme } = useTheme();

    return (
        <form
            onSubmit={(e) => {
                e.preventDefault();
                onSubmit?.();
            }}
            style={{ display: 'flex', gap: 8 }}
            className={className ?? 'w-full'}
        >
            <Input
                value={value}
                onChange={(e) => onChange?.(e.target.value)}
                placeholder={placeholder}
                allowClear
                style={{
                    background: theme.card,
                    color: theme.text,
                    borderColor: theme.border,
                }}
            />
            <Button htmlType="submit" type="primary" icon={<SearchOutlined />}>
                Search
            </Button>
        </form>
    );
};

export default SearchBar;