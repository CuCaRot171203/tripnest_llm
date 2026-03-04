import React, { createContext, useContext, useEffect, useMemo, useState } from "react";

type Lang = string;
type Namespace = string;
type Strings = Record<string, any>;

interface LanguageContextValue {
    lang: Lang;
    availableLangs: string[];
    strings: Record<Namespace, Strings>;
    setLang: (l: Lang) => void;
    toggleLang: (next?: Lang) => void;
    t: (namespace: Namespace) => Strings;
}

const STORAGE_KEY = "app-lang";
const DEFAULT_LANG = "en";

const LanguageContext = createContext<LanguageContextValue | undefined>(undefined);

export const LanguageProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const modules = import.meta.glob("../i18n/**/*.json", { eager: true, import: "default" }) as Record<string, Strings>;

    const built = useMemo(() => {
        const map = new Map<Lang, Record<Namespace, Strings>>();
        Object.entries(modules).forEach(([path, content]) => {
            const normalized = path.replace(/^(\.\.\/)+/, "src/");
            const folderMatch = normalized.match(/src\/i18n\/([a-zA-Z-]+)\/(.+)\.json$/);
            if (folderMatch) {
                const lang = folderMatch[1];
                const ns = folderMatch[2].replace(/\/+/g, "/");
                const langMap = map.get(lang) || {};
                langMap[ns] = content;
                map.set(lang, langMap);
                return;
            }
            const fileMatch = normalized.match(/src\/i18n\/(.+)\/([^\/]+)([._-]([a-z]{2,}))\.json$/i);
            if (fileMatch) {
                const nsPath = fileMatch[1]; // e.g. components/header
                const lang = (fileMatch[4] || fileMatch[3] || DEFAULT_LANG).toLowerCase();
                // namespace: nsPath (folder) + basename without lang
                const basename = fileMatch[2].replace(new RegExp(`([._-]${fileMatch[4]})$`), "");
                const ns = `${nsPath}/${basename}`.replace(/\/+/g, "/");
                const langMap = map.get(lang) || {};
                langMap[ns] = content;
                map.set(lang, langMap);
                return;
            }
            const fallbackNs = path.split("/").slice(-2).join("/").replace(".json", "");
            const langMap = map.get(DEFAULT_LANG) || {};
            langMap[fallbackNs] = content;
            map.set(DEFAULT_LANG, langMap);
        });
        const obj: Record<string, Record<string, Strings>> = {};
        map.forEach((val, key) => (obj[key] = val));
        return obj;
    }, [modules]);

    const availableLangs = Object.keys(built);
    const initialLang = (typeof window !== "undefined" ? (localStorage.getItem(STORAGE_KEY) as Lang | null) : null) || (availableLangs[0] ?? DEFAULT_LANG);

    const [lang, setLangState] = useState<Lang>(initialLang);

    useEffect(() => {
        if (!lang) return;
        localStorage.setItem(STORAGE_KEY, lang);
    }, [lang]);

    const strings = built[lang] || {};

    const setLang = (l: Lang) => {
        if (built[l]) setLangState(l);
        else console.warn(`Language ${l} not available`);
    };

    const toggleLang = (next?: Lang) => {
        if (next) return setLang(next);
        const idx = availableLangs.indexOf(lang);
        const nextIdx = (idx + 1) % Math.max(1, availableLangs.length || 1);
        setLangState(availableLangs[nextIdx] || lang);
    };

    const t = (namespace: string) => {
        return strings[namespace] || {};
    };

    return (
        <LanguageContext.Provider value={{ lang, availableLangs, strings, setLang, toggleLang, t }}>
            {children}
        </LanguageContext.Provider>
    );
};

export const useLanguage = () => {
    const ctx = useContext(LanguageContext);
    if (!ctx) throw new Error("useLanguage must be used within LanguageProvider");
    return ctx;
};

export const useTranslation = (namespace: string) => {
    const { strings } = useLanguage();
    const nsObj = useMemo(() => {
        return (strings && strings[namespace]) ? strings[namespace] : {};
    }, [strings, namespace]);

    const t = (key?: string, fallback?: any) => {
        if (!key) return nsObj;
        const parts = key.split('.');
        let cur: any = nsObj;
        for (const p of parts) {
            if (cur && typeof cur === 'object' && p in cur) cur = cur[p];
            else return fallback ?? key;
        }
        return cur;
    };

    return { ...nsObj, t };
};