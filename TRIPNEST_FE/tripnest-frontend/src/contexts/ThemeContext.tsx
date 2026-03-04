import React, { createContext, useContext, useEffect, useState } from "react";
import { lightTheme, darkTheme } from "../styles/theme";
import type { Theme } from "../styles/theme";

type ThemeName = "light" | "dark";

interface ThemeContextValue {
    themeName: ThemeName;
    theme: Theme;
    toggleTheme: () => void;
    setThemeName: (name: ThemeName) => void;
}

const ThemeContext = createContext<ThemeContextValue | undefined>(undefined);

const STORAGE_KEY = "app-theme";

export const ThemeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [themeName, setThemeNameState] = useState<ThemeName>(() => {
        const saved = typeof window !== "undefined" ? localStorage.getItem(STORAGE_KEY) : null;
        return (saved as ThemeName) || "light";
    });

    useEffect(() => {
        localStorage.setItem(STORAGE_KEY, themeName);
    }, [themeName]);

    const setThemeName = (name: ThemeName) => setThemeNameState(name);

    const toggleTheme = () => setThemeNameState(prev => (prev === "light" ? "dark" : "light"));

    const theme = themeName === "light" ? lightTheme : darkTheme;

    // APPLY CSS
    useEffect(() => {
        const root = document.documentElement;
        Object.entries(theme).forEach(([k, v]) => {
            root.style.setProperty(`--theme-${k}`, String(v));
        });
    }, [theme]);

    return (
        <ThemeContext.Provider value={{ themeName, theme, toggleTheme, setThemeName }}>
            {children}
        </ThemeContext.Provider>
    );
};

export const useTheme = (): ThemeContextValue => {
    const ctx = useContext(ThemeContext);
    if (!ctx) {
        throw new Error("useTheme must be used within ThemeProvider");
    }
    return ctx;
};