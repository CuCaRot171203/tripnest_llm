import React, { useState } from "react";
import { Form, Input, Button, Typography, Divider, notification } from "antd";
import { MailOutlined } from "@ant-design/icons";
import AuthLayout from "../../shared/components/AuthLayout";
import apiClient from "../../services/apiClient";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "../../contexts/LanguageContext";

const { Title, Text } = Typography;

const ResetPasswordPage: React.FC = () => {
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const i18n = useTranslation("auth/reset-password");

    const onFinish = async (values: { email: string }) => {
        setLoading(true);
        try {
            const resp = await apiClient.post("/api/auth/forgot-password", { email: values.email });

            // Expect backend to return 200 on success
            if (resp && resp.status === 200) {
                notification.success({
                    message: i18n.t ? i18n.t("successTitle", "Yêu cầu thành công") : "Request successful",
                    description: i18n.t
                        ? i18n.t("successDesc", "Một email chứa hướng dẫn đặt lại mật khẩu đã được gửi nếu email tồn tại.")
                        : "An email with password reset instructions has been sent if the email exists.",
                });
                // optionally redirect user to login page
                try {
                    navigate("/auth/login", { replace: true });
                } catch { /* ignore navigation failures */ }
                return;
            }

            // fallback: treat other statuses as failure
            notification.error({
                message: i18n.t ? i18n.t("submitFail", "Yêu cầu thất bại") : "Request failed",
                description: i18n.t
                    ? i18n.t("submitFailDesc", "Không thể gửi yêu cầu reset mật khẩu. Vui lòng thử lại.")
                    : "Could not send reset request. Please try again.",
            });
        } catch (err: any) {
            // handle known 400 response (validation)
            const resp = err?.response;
            if (resp && resp.status === 400) {
                // backend may return { message } or validation errors
                const desc = resp.data?.message || resp.data?.error || JSON.stringify(resp.data);
                notification.error({
                    message: i18n.t ? i18n.t("badRequestTitle", "Yêu cầu không hợp lệ") : "Bad request",
                    description: desc || (i18n.t ? i18n.t("badRequestFallback", "Dữ liệu gửi lên không hợp lệ.") : "Invalid input."),
                });
            } else {
                // generic network / server error
                const desc = err?.message || (i18n.t ? i18n.t("genericError", "Đã xảy ra lỗi. Vui lòng thử lại sau.") : "An error occurred. Please try again later.");
                notification.error({
                    message: i18n.t ? i18n.t("submitFail", "Yêu cầu thất bại") : "Request failed",
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
                    {i18n.t ? i18n.t("subtitle", "Nhập email của bạn. Chúng tôi sẽ gửi hướng dẫn đặt lại mật khẩu.") : "Enter your email. We'll send reset instructions."}
                </Text>

                <Form layout="vertical" className="mt-6" onFinish={onFinish} initialValues={{}}>
                    <Form.Item
                        name="email"
                        label={i18n.t ? i18n.t("emailLabel", "Email") : "Email"}
                        rules={[
                            { required: true, message: i18n.t ? i18n.t("errors.requiredEmail", "Vui lòng nhập email") : "Please enter email" },
                            { type: "email", message: i18n.t ? i18n.t("errors.invalidEmail", "Email không hợp lệ") : "Invalid email" },
                        ]}
                    >
                        <Input prefix={<MailOutlined />} placeholder={i18n.t ? i18n.t("placeholders.email", "email@example.com") : "email@example.com"} />
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" block loading={loading}>
                            {i18n.t ? i18n.t("submit", "Gửi yêu cầu") : "Send request"}
                        </Button>
                    </Form.Item>

                    <Divider plain>{i18n.t ? i18n.t("or", "Hoặc") : "Or"}</Divider>

                    <div className="flex gap-2 justify-center">
                        <Button type="link" onClick={() => navigate("/auth/login")}>
                            {i18n.t ? i18n.t("links.backToLogin", "Quay lại đăng nhập") : "Back to login"}
                        </Button>
                    </div>
                </Form>
            </div>
        </AuthLayout>
    );
};

export default ResetPasswordPage;