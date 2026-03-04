import React, { useState } from "react";
import { Form, Input, Button, Typography, Divider, notification } from "antd";
import { MailOutlined, LockOutlined, GoogleOutlined } from "@ant-design/icons";
import { useAuth } from "../../hooks/useAuth";
import { useNavigate } from "react-router-dom";
import AuthLayout from "../../shared/components/AuthLayout";
import { useTranslation } from "../../contexts/LanguageContext";

const { Title, Text, Link } = Typography;

const LoginPage: React.FC = () => {
    const { login } = useAuth();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const i18n = useTranslation("auth/login");

    const onFinish = async (values: { email: string; password: string }) => {
        setLoading(true);
        try {
            await login(values); // nếu loginWithApi đã notification.success thì OK, nhưng ta vẫn hiển thị ở đây để chắc chắn
            notification.success({
                message: i18n.t ? i18n.t("title", "Đăng nhập thành công") : "Đăng nhập thành công",
                description: i18n.t ? i18n.t("successDesc", "Chào mừng trở lại") : "Chào mừng trở lại",
            });
            navigate("/", { replace: true });
        } catch (err: any) {
            // lấy message từ response nếu có
            const resp = err?.response;
            let description = "Đã xảy ra lỗi khi đăng nhập.";
            if (resp?.data) {
                // backend có thể trả { message } hoặc errors
                description = resp.data?.message || resp.data?.error || JSON.stringify(resp.data);
            } else if (err?.message) {
                description = err.message;
            }

            notification.error({
                message: i18n.t ? i18n.t("submitFail", "Đăng nhập thất bại") : "Đăng nhập thất bại",
                description,
            });
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthLayout title={i18n.t ? i18n.t("title", "Chào mừng trở lại") : "Chào mừng trở lại"}>
            <div className="p-6 sm:p-8">
                <Title level={3} className="mb-2">{i18n.t ? i18n.t("title", "Đăng nhập") : "Đăng nhập"}</Title>
                <Text type="secondary">{i18n.t ? i18n.t("subtitle", "Đăng nhập bằng email và mật khẩu của bạn") : "Đăng nhập bằng email và mật khẩu của bạn"}</Text>

                <Form layout="vertical" className="mt-6" onFinish={onFinish} initialValues={{}}>
                    <Form.Item
                        name="email"
                        label={i18n.t ? i18n.t("emailLabel", "Email") : "Email"}
                        rules={[
                            { required: true, message: i18n.t ? i18n.t("errors.requiredEmail", "Vui lòng nhập email") : "Vui lòng nhập email" },
                            { type: "email", message: i18n.t ? i18n.t("errors.invalidEmail", "Email không hợp lệ") : "Email không hợp lệ" },
                        ]}
                    >
                        <Input prefix={<MailOutlined />} placeholder={i18n.t ? i18n.t("placeholders.email", "email@example.com") : "email@example.com"} />
                    </Form.Item>

                    <Form.Item
                        name="password"
                        label={i18n.t ? i18n.t("passwordLabel", "Mật khẩu") : "Mật khẩu"}
                        rules={[{ required: true, message: i18n.t ? i18n.t("errors.requiredPassword", "Vui lòng nhập mật khẩu") : "Vui lòng nhập mật khẩu" }]}
                    >
                        <Input.Password prefix={<LockOutlined />} placeholder={i18n.t ? i18n.t("placeholders.password", "Mật khẩu") : "Mật khẩu"} />
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" block loading={loading}>
                            {i18n.t ? i18n.t("submit", "Đăng nhập") : "Đăng nhập"}
                        </Button>
                    </Form.Item>

                    <div className="flex justify-between">
                        <Link onClick={() => navigate("/auth/register")}>{i18n.t ? i18n.t("links.register", "Đăng ký") : "Đăng ký"}</Link>
                        <Link onClick={() => navigate("/auth/forgot")}>{i18n.t ? i18n.t("links.forgot", "Quên mật khẩu?") : "Quên mật khẩu?"}</Link>
                    </div>

                    <Divider plain>{i18n.t ? i18n.t("or", "Hoặc") : "Hoặc"}</Divider>

                    <div className="flex gap-2 justify-center">
                        <Button block>{i18n.t ? i18n.t("social.google", "Đăng nhập với Google") : "Đăng nhập với Google"} <GoogleOutlined /></Button>
                    </div>
                </Form>
            </div>
        </AuthLayout>
    );
};

export default LoginPage;
