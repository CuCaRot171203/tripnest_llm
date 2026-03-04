import React, { useState } from "react";
import { Form, Input, Button, Typography, Divider, Select } from "antd";
import { MailOutlined, LockOutlined, UserOutlined, PhoneOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import AuthLayout from "../../shared/components/AuthLayout";
import apiClient from "../../services/apiClient";
import { notification } from "antd";
import { useTranslation } from "../../contexts/LanguageContext";

const { Title, Text, Link } = Typography;
const { Option } = Select;

const RegisterPage: React.FC = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const t = useTranslation("auth/register");

    const handleSuccess = () => {
        notification.success({
            message: t.t("success.registered")
        });
        navigate("/auth/login", { replace: true });
    };

    const handleError = (err: any) => {
        const resp = err?.response;
        if (resp?.status === 400) {
            notification.error({
                message: t.t("errors.badRequest", "Dữ liệu không hợp lệ"),
                description: resp.data?.message
            });
            return;
        }

        notification.error({
            message: t.t("error", "Lỗi"),
            description: resp?.data?.message || "Có lỗi xảy ra."
        });
    };

    const onFinish = async (values: any) => {
        setLoading(true);
        try {
            const payload = {
                email: values.email,
                password: values.password,
                fullName: values.fullName,
                phone: values.phone,
                locale: values.locale
            };

            const resp = await apiClient.post("/api/Auth/register", payload);

            if (resp.status === 201 || resp.status === 200) {
                handleSuccess();
            } else {
                notification.error({
                    message: t.t("error", "Lỗi"),
                    description: t.t("unexpected", "Có lỗi bất ngờ xảy ra.")
                });
            }
        } catch (error) {
            handleError(error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthLayout title={t.t("title")}>
            {/* -> Không dùng lớp bg-white nữa, vì AuthLayout đã là nền trắng */}
            <div>
                <Title level={3} className="mb-2">
                    {t.t("title")}
                </Title>
                <Text type="secondary">{t.t("subtitle")}</Text>

                <Form layout="vertical" className="mt-6" onFinish={onFinish}>
                    {/* Grid: 2 columns trên md+, 1 column trên mobile */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {/* Full name */}
                        <Form.Item
                            name="fullName"
                            label={t.t("fullNameLabel")}
                            rules={[
                                { required: true, message: t.t("errors.requiredFullName") }
                            ]}
                        >
                            <Input prefix={<UserOutlined />} placeholder={t.t("placeholders.fullName")} />
                        </Form.Item>

                        {/* Email */}
                        <Form.Item
                            name="email"
                            label={t.t("emailLabel")}
                            rules={[
                                { required: true, message: t.t("errors.requiredEmail") },
                                { type: "email", message: t.t("errors.invalidEmail") }
                            ]}
                        >
                            <Input prefix={<MailOutlined />} placeholder={t.t("placeholders.email")} />
                        </Form.Item>

                        {/* Password */}
                        <Form.Item
                            name="password"
                            label={t.t("passwordLabel")}
                            rules={[
                                { required: true, message: t.t("errors.requiredPassword") },
                                { min: 6, message: t.t("errors.passwordMin") }
                            ]}
                            hasFeedback
                        >
                            <Input.Password prefix={<LockOutlined />} placeholder={t.t("placeholders.password")} />
                        </Form.Item>

                        {/* Confirm password */}
                        <Form.Item
                            name="confirmPassword"
                            label={t.t("confirmPasswordLabel")}
                            dependencies={["password"]}
                            hasFeedback
                            rules={[
                                { required: true, message: t.t("errors.requiredConfirm") },
                                ({ getFieldValue }) => ({
                                    validator(_: any, value: string) {
                                        if (!value || value === getFieldValue("password")) {
                                            return Promise.resolve();
                                        }
                                        return Promise.reject(new Error(t.t("errors.passwordMismatch")));
                                    }
                                })
                            ]}
                        >
                            <Input.Password prefix={<LockOutlined />} placeholder={t.t("placeholders.confirmPassword")} />
                        </Form.Item>

                        {/* Phone */}
                        <Form.Item
                            name="phone"
                            label={t.t("phoneLabel")}
                            rules={[
                                { required: true, message: t.t("errors.requiredPhone") },
                                { pattern: /^\+?\d{7,15}$/, message: t.t("errors.invalidPhone") }
                            ]}
                        >
                            <Input prefix={<PhoneOutlined />} placeholder={t.t("placeholders.phone")} />
                        </Form.Item>

                        {/* Locale */}
                        <Form.Item name="locale" label={t.t("localeLabel")} initialValue="vi">
                            <Select>
                                <Option value="vi">Tiếng Việt</Option>
                                <Option value="en">English</Option>
                            </Select>
                        </Form.Item>
                    </div>

                    {/* Submit */}
                    <Form.Item className="mt-4">
                        <Button type="primary" htmlType="submit" block loading={loading}>
                            {t.t("submit")}
                        </Button>
                    </Form.Item>
                    <Divider plain>{t.t("or", "Hoặc")}</Divider>
                    <div className="flex justify-between">
                        <Link onClick={() => navigate("/auth/login")}>{t.t("links.login")}</Link>
                    </div>
                </Form>
            </div>
        </AuthLayout>
    );
};

export default RegisterPage;