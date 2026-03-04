import React, { useState, useEffect } from "react";
import { Form, Input, Button, Typography, notification } from "antd";
import { LockOutlined } from "@ant-design/icons";
import { useNavigate, useLocation, useParams } from "react-router-dom";
import AuthLayout from "../../shared/components/AuthLayout";
import apiClient from "../../services/apiClient";
import { useTranslation } from "../../contexts/LanguageContext";

const { Title, Text } = Typography;

function useQuery() {
    return new URLSearchParams(useLocation().search);
}

const ResetPasswordConfirmPage: React.FC = () => {
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const params = useParams<{ token?: string }>();
    const query = useQuery();
    const i18n = useTranslation("auth/reset-password-confirm");

    // token: try route param first, then query string
    const [token, setToken] = useState<string | null>(() => {
        try {
            return params?.token ?? null;
        } catch {
            return null;
        }
    });

    useEffect(() => {
        if (!token) {
            const q = query.get("token");
            if (q) setToken(q);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const onFinish = async (values: { password: string; confirm: string }) => {
        if (!token) {
            notification.error({
                message: i18n.t ? i18n.t("noTokenTitle", "Mã không hợp lệ") : "Invalid token",
                description: i18n.t ? i18n.t("noTokenDesc", "Thiếu token đặt lại mật khẩu. Vui lòng kiểm tra email của bạn.") : "Missing reset token. Please check your email.",
            });
            return;
        }

        setLoading(true);
        try {
            const payload = {
                token,
                newPassword: values.password,
            };

            const resp = await apiClient.post("/api/auth/reset-password", payload);

            // success (200)
            if (resp && resp.status === 200) {
                notification.success({
                    message: i18n.t ? i18n.t("successTitle", "Đổi mật khẩu thành công") : "Password reset successful",
                    description: i18n.t ? i18n.t("successDesc", "Bạn có thể dùng mật khẩu mới để đăng nhập.") : "You can now sign in with your new password.",
                });
                navigate("/auth/login", { replace: true });
                return;
            }

            // fallback
            notification.error({
                message: i18n.t ? i18n.t("submitFail", "Thất bại") : "Failed",
                description: i18n.t ? i18n.t("submitFailDesc", "Không thể đặt lại mật khẩu. Vui lòng thử lại.") : "Could not reset password. Please try again.",
            });
        } catch (err: any) {
            const resp = err?.response;
            if (resp && resp.status === 400) {
                // expected validation / token errors
                const desc = resp.data?.message || resp.data?.error || JSON.stringify(resp.data);
                notification.error({
                    message: i18n.t ? i18n.t("badRequestTitle", "Yêu cầu không hợp lệ") : "Bad request",
                    description: desc || (i18n.t ? i18n.t("badRequestFallback", "Dữ liệu không hợp lệ.") : "Invalid input."),
                });
            } else {
                const desc = err?.message || (i18n.t ? i18n.t("genericError", "Đã xảy ra lỗi. Vui lòng thử lại sau.") : "An error occurred. Please try again later.");
                notification.error({
                    message: i18n.t ? i18n.t("submitFail", "Thất bại") : "Failed",
                    description: desc,
                });
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthLayout title={i18n.t ? i18n.t("pageTitle", "Đặt lại mật khẩu") : "Reset password"}>
            <div className="p-6 sm:p-8">
                <Title level={3} className="mb-2">{i18n.t ? i18n.t("heading", "Đặt lại mật khẩu") : "Reset password"}</Title>
                <Text type="secondary">
                    {i18n.t ? i18n.t("subtitle", "Nhập mật khẩu mới. Liên kết đặt lại sẽ hết hạn sau một thời gian.") : "Enter your new password. The reset link may expire."}
                </Text>

                <Form
                    layout="vertical"
                    className="mt-6"
                    onFinish={onFinish}
                    initialValues={{ password: "", confirm: "" }}
                >
                    <Form.Item
                        name="password"
                        label={i18n.t ? i18n.t("passwordLabel", "Mật khẩu mới") : "New password"}
                        rules={[
                            { required: true, message: i18n.t ? i18n.t("errors.requiredPassword", "Vui lòng nhập mật khẩu") : "Please enter a password" },
                            { min: 6, message: i18n.t ? i18n.t("errors.minLength", "Mật khẩu phải có ít nhất 6 ký tự") : "Password must be at least 6 characters" },
                        ]}
                        hasFeedback
                    >
                        <Input.Password prefix={<LockOutlined />} placeholder={i18n.t ? i18n.t("placeholders.password", "Mật khẩu mới") : "New password"} />
                    </Form.Item>

                    <Form.Item
                        name="confirm"
                        label={i18n.t ? i18n.t("confirmLabel", "Xác nhận mật khẩu") : "Confirm password"}
                        dependencies={["password"]}
                        hasFeedback
                        rules={[
                            { required: true, message: i18n.t ? i18n.t("errors.requiredConfirm", "Vui lòng xác nhận mật khẩu") : "Please confirm your password" },
                            ({ getFieldValue }) => ({
                                validator(_, value) {
                                    if (!value || getFieldValue("password") === value) {
                                        return Promise.resolve();
                                    }
                                    return Promise.reject(new Error(i18n.t ? i18n.t("errors.match", "Mật khẩu không khớp") : "Passwords do not match"));
                                },
                            }),
                        ]}
                    >
                        <Input.Password prefix={<LockOutlined />} placeholder={i18n.t ? i18n.t("placeholders.confirm", "Xác nhận mật khẩu") : "Confirm password"} />
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" block loading={loading}>
                            {i18n.t ? i18n.t("submit", "Đặt lại mật khẩu") : "Reset password"}
                        </Button>
                    </Form.Item>

                    <div className="text-center">
                        <Button type="link" onClick={() => navigate("/auth/login")}>
                            {i18n.t ? i18n.t("links.backToLogin", "Quay lại đăng nhập") : "Back to login"}
                        </Button>
                    </div>
                </Form>
            </div>
        </AuthLayout>
    );
};

export default ResetPasswordConfirmPage;
