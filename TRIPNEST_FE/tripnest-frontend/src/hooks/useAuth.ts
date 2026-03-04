import { useCallback, useEffect, useState } from "react";
import { notification } from "antd";
import apiClient from "../services/apiClient";
import { useNavigate } from "react-router-dom";

type NavigateFn = ((to: string,
    options?: { replace?: boolean }) => void) | undefined;

export type AuthUser = {
    id?: string | number;
    email?: string;
    fullName?: string | null;
    profilePhotoUrl?: string | null;
    [k: string]: any;
};

export type AuthState = {
    user: AuthUser | null;
    token: string | null;
    refreshToken?: string | null;
    isAuthenticated: boolean;
    cartCount?: number;
};

const LS_KEY = "tn_auth";

class AuthStore {
    private state: AuthState;
    private listeners: Set<(s: AuthState) => void>;

    constructor() {
        this.listeners = new Set();
        this.state = this.loadFromStorage();
    }

    private loadFromStorage(): AuthState {
        try {
            const raw = localStorage.getItem(LS_KEY);
            if (!raw) return { user: null, token: null, refreshToken: null, isAuthenticated: false, cartCount: 0 };
            const parsed = JSON.parse(raw);
            return {
                user: parsed.user || null,
                token: parsed.token || null,
                refreshToken: parsed.refreshToken || null,
                isAuthenticated: !!parsed.token,
                cartCount: typeof parsed.cartCount === "number" ? parsed.cartCount : 0,
            };
        } catch {
            return { user: null, token: null, refreshToken: null, isAuthenticated: false, cartCount: 0 };
        }
    }

    private persist() {
        const toStore = {
            user: this.state.user,
            token: this.state.token,
            refreshToken: this.state.refreshToken,
            cartCount: this.state.cartCount || 0,
        };
        try {
            localStorage.setItem(LS_KEY, JSON.stringify(toStore));
        } catch { }
    }

    getState(): AuthState {
        return this.state;
    }

    setState(partial: Partial<AuthState>) {
        this.state = {
            ...this.state,
            ...partial,
            isAuthenticated: !!(partial.token ?? this.state.token),
        };
        this.persist();
        this.emit();
    }

    clear() {
        this.state = { user: null, token: null, refreshToken: null, isAuthenticated: false, cartCount: 0 };
        try {
            localStorage.removeItem(LS_KEY);
        } catch { }
        this.emit();
    }

    subscribe(fn: (s: AuthState) => void) {
        this.listeners.add(fn);
        // call immediately with current state
        try { fn(this.state); } catch { }
        return () => this.listeners.delete(fn);
    }

    private emit() {
        for (const l of Array.from(this.listeners)) {
            try {
                l(this.state);
            } catch { }
        }
    }
}

const store = new AuthStore();

function extractTokenAndUser(respData: any) {
    if (!respData) return { token: null, refreshToken: null, user: null };
    const token =
        respData?.token ||
        respData?.data?.token ||
        respData?.accessToken ||
        respData?.data?.accessToken ||
        (typeof respData === "string" ? respData : undefined);

    const refreshToken =
        respData?.refreshToken ||
        respData?.data?.refreshToken ||
        respData?.refresh_token ||
        respData?.data?.refresh_token ||
        null;

    const user = respData?.user || respData?.data?.user || respData?.data || null;
    return { token: token ?? null, refreshToken: refreshToken ?? null, user: user ?? null };
}

export async function loginWithApi(payload: { email: string; password: string }) {
    try {
        const resp = await apiClient.post("/api/auth/login", payload);
        const { token, refreshToken, user } = extractTokenAndUser(resp.data);
        if (!token) {
            notification.error({ message: "Lỗi đăng nhập", description: "Server không trả token." });
            throw new Error("No token in response");
        }
        store.setState({ token, refreshToken, user, isAuthenticated: true });
        notification.success({ message: "Đăng nhập thành công" });
        return store.getState();
    } catch (err: any) {
        const resp = err?.response;
        if (resp && resp.status === 401) {
            notification.error({ message: "Đăng nhập thất bại", description: "Email hoặc mật khẩu không đúng." });
        } else if (resp && resp.status === 400) {
            const msg = resp.data?.message || "Dữ liệu không hợp lệ.";
            notification.error({ message: "Đăng nhập thất bại", description: msg });
        } else {
            notification.error({ message: "Lỗi", description: "Đã xảy ra lỗi khi đăng nhập." });
        }
        throw err;
    }
}

export function logoutLocal(navigate?: NavigateFn, redirectTo = "/auth/login") {
    store.clear();
    notification.info({ message: "Bạn đã đăng xuất" });
    if (navigate) {
        try {
            navigate(redirectTo, { replace: true });
        } catch { }
    }
}

export function useAuth() {
    const [auth, setAuth] = useState<AuthState>(() => store.getState());
    const navigate = useNavigate();

    useEffect(() => {
        const unsub = store.subscribe((s) => {
            setAuth(s);
        });
        return () => unsub();
    }, []);

    const login = useCallback(async (payload: { email: string; password: string }) => {
        return loginWithApi(payload);
    }, []);

    const logout = useCallback(() => {
        logoutLocal(navigate as unknown as NavigateFn, "/auth/login");
    }, [navigate]);

    const setUser = useCallback((u: AuthUser | null) => {
        store.setState({ user: u });
    }, []);

    const setCartCount = useCallback((n: number) => {
        store.setState({ cartCount: n });
    }, []);

    const refreshStoredAccessToken = useCallback(async (): Promise<string | null> => {
        // try to refresh using saved refreshToken
        const s = store.getState();
        const rToken = s.refreshToken;
        if (!rToken) {
            return null;
        }
        try {
            // call refresh endpoint (adapt path if different)
            const resp = await apiClient.post("/api/auth/refresh-token", { refreshToken: rToken });
            const { token: newToken, refreshToken: newRefresh } = extractTokenAndUser(resp.data);
            if (newToken) {
                store.setState({ token: newToken, refreshToken: newRefresh ?? s.refreshToken });
                return newToken;
            }
            return null;
        } catch (err) {
            // refresh failed => clear auth
            store.clear();
            return null;
        }
    }, []);

    return {
        auth,
        login,
        logout,
        setUser,
        setCartCount,
        refreshStoredAccessToken,
    };
}