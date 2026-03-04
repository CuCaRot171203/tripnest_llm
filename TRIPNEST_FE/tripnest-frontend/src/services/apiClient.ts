import axios from "axios";
import { notification } from "antd";

const envProcess = typeof process !== "undefined" ? (process as any).env : undefined;
const viteEnv = typeof import.meta !== "undefined" ? (import.meta as any).env : undefined;

const BASE_URL =
    (envProcess && (envProcess.REACT_APP_BACKEND_API_URL || envProcess.BACKEND_API_URL)) ||
    (viteEnv && viteEnv.VITE_BACKEND_API_URL) ||
    "https://localhost:7140";

const apiClient = axios.create({
    baseURL: BASE_URL,
    headers: {
        "Content-Type": "application/json",
    },
    timeout: 15000,
});

// Helper to read tn_auth
function readAuthStorage() {
    try {
        const raw = localStorage.getItem("tn_auth");
        if (!raw) return null;
        return JSON.parse(raw);
    } catch {
        return null;
    }
}

// Attach token on every request (reads fresh from localStorage)
apiClient.interceptors.request.use((config) => {
    try {
        const parsed = readAuthStorage();
        if (parsed?.token) {
            config.headers = config.headers || {};
            (config.headers as any).Authorization = `Bearer ${parsed.token}`;
        }
    } catch (e) {
        // ignore
    }
    return config;
});

// Response interceptor with refresh-token retry queue
let isRefreshing = false;
let failedQueue: Array<{ resolve: (val?: any) => void; reject: (err: any) => void }> = [];

const processQueue = (error: any, token: string | null = null) => {
    failedQueue.forEach((p) => {
        if (error) p.reject(error);
        else p.resolve(token);
    });
    failedQueue = [];
};

apiClient.interceptors.response.use(
    (resp) => resp,
    async (error) => {
        const originalReq = error.config;
        const { response } = error;

        if (!response) {
            notification.error({ message: "Network error", description: "Không thể kết nối tới server." });
            return Promise.reject(error);
        }

        // If 401 and request not marked as _retry, try refresh-token flow
        if (response.status === 401 && !originalReq._retry) {
            originalReq._retry = true;

            if (isRefreshing) {
                // queue the request until refresh finishes
                return new Promise(function (resolve, reject) {
                    failedQueue.push({ resolve, reject });
                })
                    .then((token) => {
                        originalReq.headers["Authorization"] = "Bearer " + token;
                        return axios(originalReq);
                    })
                    .catch((err) => Promise.reject(err));
            }

            isRefreshing = true;

            try {
                const parsed = readAuthStorage();
                const refreshToken = parsed?.refreshToken;
                if (!refreshToken) {
                    // no refresh token -> logout handled by app
                    isRefreshing = false;
                    processQueue("No refresh token", null);
                    return Promise.reject(error);
                }

                // Call refresh endpoint (adjust path if your backend uses different field)
                const respRefresh = await axios.post(`${BASE_URL}/api/auth/refresh-token`, { refreshToken });
                const respData = respRefresh.data;

                // try to extract new tokens
                const newAccess =
                    respData?.accessToken || respData?.token || respData?.data?.accessToken || respData?.data?.token;
                const newRefresh =
                    respData?.refreshToken || respData?.data?.refreshToken || respData?.refresh_token || null;

                if (newAccess) {
                    // update localStorage tn_auth while preserving user/cart
                    try {
                        const current = readAuthStorage() || {};
                        const updated = { ...current, token: newAccess, refreshToken: newRefresh ?? current.refreshToken };
                        localStorage.setItem("tn_auth", JSON.stringify(updated));
                    } catch { /* ignore */ }

                    apiClient.defaults.headers.common["Authorization"] = "Bearer " + newAccess;
                    processQueue(null, newAccess);
                    // retry original request with new token
                    originalReq.headers["Authorization"] = "Bearer " + newAccess;
                    return axios(originalReq);
                } else {
                    processQueue("No new access token", null);
                    return Promise.reject(error);
                }
            } catch (errRefresh) {
                processQueue(errRefresh, null);
                // clear auth storage (optional: you may want to notify user)
                try { localStorage.removeItem("tn_auth"); } catch { }
                notification.error({ message: "Phiên đăng nhập hết hạn", description: "Vui lòng đăng nhập lại." });
                return Promise.reject(errRefresh);
            } finally {
                isRefreshing = false;
            }
        }

        // other statuses: show friendly messages
        if (response.status === 400) {
            const msg = (response.data && (response.data.message || response.data.error)) || "Dữ liệu gửi lên không hợp lệ.";
            notification.error({ message: "Bad request", description: String(msg) });
        } else if (response.status >= 500) {
            notification.error({ message: "Server error", description: "Lỗi phía server. Vui lòng thử lại sau." });
        } else if (response.status === 401) {
            // fallback (if retry not applicable)
            notification.error({ message: "Unauthorized", description: "Bạn cần đăng nhập lại." });
        }

        return Promise.reject(error);
    }
);

export default apiClient;