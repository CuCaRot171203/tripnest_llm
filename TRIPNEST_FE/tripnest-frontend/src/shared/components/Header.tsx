import React, { useEffect, useState } from "react";
import { useTheme } from "../../contexts/ThemeContext";
import { useLanguage, useTranslation } from "../../contexts/LanguageContext";
import logoImage from "../../assets/images/Home/TRIPNEST_LOGO_BG_REMOVE.png";
import { BulbOutlined, ShoppingCartOutlined, LogoutOutlined, UserOutlined } from "@ant-design/icons";
import { Avatar, Badge, Dropdown, Menu, Button } from "antd";
import { useAuth } from "../../hooks/useAuth";
import { useNavigate } from "react-router-dom";

const MOBILE_BREAKPOINT = 768;

const Header: React.FC = () => {
    const { themeName, theme, toggleTheme } = useTheme();
    const { lang, strings, toggleLang } = useLanguage();
    const headerStrings = useTranslation("components/header");
    const [query, setQuery] = useState("");
    const [isMobile, setIsMobile] = useState<boolean>(() =>
        typeof window !== "undefined" ? window.innerWidth <= MOBILE_BREAKPOINT : false
    );
    const [mobileSearchOpen, setMobileSearchOpen] = useState(false);
    const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

    const { auth, logout } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        const onResize = () => {
            setIsMobile(window.innerWidth <= MOBILE_BREAKPOINT);
            if (window.innerWidth > MOBILE_BREAKPOINT) {
                setMobileSearchOpen(false);
                setMobileMenuOpen(false);
            }
        };
        window.addEventListener("resize", onResize);
        return () => window.removeEventListener("resize", onResize);
    }, []);

    const onSearch = (e?: React.FormEvent) => {
        e?.preventDefault();
        // Execute logic find
        console.log("Search: ", query);
    };

    const headerStyle: React.CSSProperties = {
        background: theme.bg,
        color: theme.text,
        borderBottom: `1px solid ${theme.border}`,
        boxShadow: `0 1px 0 rgba(0,0,0,0.03)`,
    };

    const containerStyle: React.CSSProperties = {
        maxWidth: 1200,
        margin: "0 auto",
        display: "flex",
        alignItems: "center",
        gap: 12,
    };

    const logoStyle: React.CSSProperties = {
        display: "flex",
        alignItems: "center",
    };

    const buttonStyle: React.CSSProperties = {
        background: "transparent",
        color: theme.text,
        padding: "6px 10px",
        cursor: "pointer",
    };

    const subTextStyle: React.CSSProperties = {
        background: "transparent",
        color: theme.text,
        cursor: "pointer",
    };

    const buttonRegisterStyle: React.CSSProperties = {
        color: "white",
        padding: "6px 10px",
        cursor: "pointer",
    };

    // Helper for initials
    const initials = (() => {
        const name = auth?.user?.fullName || auth?.user?.email || "";
        const parts = String(name).trim().split(/\s+/);
        if (parts.length === 0) {
            return "U";
        }
        if (parts.length === 1) {
            return parts[0].slice(0, 2).toUpperCase();
        }
        const lastTwo = parts.slice(-2)
            .map((p: string) => p[0])
            .join("");
        return lastTwo.toUpperCase();
    })();

    const cartCount = auth?.cartCount ?? 0;

    const profileMenu = (
        <Menu>
            <Menu.Item key="profile" onClick={() => navigate("/profile")}>
                {headerStrings.profile || "Hồ sơ"}
            </Menu.Item>
            <Menu.Item key="orders" onClick={() => navigate("/orders")}>
                {headerStrings.orders || "Đơn hàng"}
            </Menu.Item>
            <Menu.Divider />
            <Menu.Item key="logout" icon={<LogoutOutlined />} onClick={() => { logout(); navigate("/"); }}>
                {headerStrings.logout || "Đăng xuất"}
            </Menu.Item>
        </Menu>
    );

    return (
        <header style={headerStyle} className="w-[100vw]">
            <div style={containerStyle} className="flex flex-row justify-between">
                {/* Logo */}
                <div style={logoStyle} className="hover:cursor-pointer">
                    <img src={logoImage} alt="" className="w-[9vw]" />
                </div>

                {/* Desktop Actions */}
                {!isMobile && (
                    <div className="flex flex-row">
                        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                            <a href="/contact" style={{ textDecoration: "none" }}>
                                <button style={buttonStyle}>{headerStrings.contact || "Contact"}</button>
                            </a>
                            <a href="/#" style={{ textDecoration: "none" }}>
                                <button style={buttonStyle}>{headerStrings.hotel || "Hotel"}</button>
                            </a>

                            <Badge count={cartCount} overflowCount={99}>
                                <Button
                                    type="text"
                                    onClick={() => navigate("/cart")}
                                    icon={<ShoppingCartOutlined style={{ fontSize: 18 }} />}
                                />
                            </Badge>

                            {auth?.isAuthenticated && auth.user ? (
                                <Dropdown overlay={profileMenu} placement="bottomRight" trigger={["click"]}>
                                    <div className="flex items-center gap-2 cursor-pointer ml-2">
                                        {auth.user.profilePhotoUrl ? (
                                            <Avatar src={auth.user.profilePhotoUrl} />
                                        ) : (
                                            <Avatar>{initials}</Avatar>
                                        )}
                                        <div className="hidden md:block" style={{ fontSize: 12 }}>
                                            {auth.user.fullName || auth.user.email}
                                        </div>
                                    </div>
                                </Dropdown>
                            ) : (
                                <div className="flex gap-2 ml-2">
                                    <button style={buttonStyle}
                                        onClick={() => navigate("/auth/login")}
                                        className="outline-none flex flex-row 
                                        items-center justify-center
                                        border-blue-700
                                        text-[0.9vw] w-[7.5vw] h-[2.5vw]">
                                        <UserOutlined className="mr-[5px]" />
                                        {headerStrings.login || "Login"}
                                    </button>
                                    <button style={buttonRegisterStyle}
                                        onClick={() => navigate("/auth/register")}
                                        className="outline-none flex flex-row 
                                        items-center justify-center
                                        bg-blue-700 text-[0.9vw]
                                        h-[2.5vw] w-[6.5vw] hover:cursor-pointer
                                        hover:bg-blue-900 transition-all">
                                        {headerStrings.register || "Register"}
                                    </button>
                                </div>
                            )}
                        </div>

                        <div className="absolute right-[10px] space-x-[0.5vw]">
                            <button aria-label="toggle-language" onClick={() => toggleLang()} style={buttonStyle}>
                                {lang === "en" ? "EN" : "VI"}
                            </button>

                            <button className="outline-none" aria-label="toggle-theme" onClick={toggleTheme} style={buttonStyle}>
                                <BulbOutlined />
                                {themeName === "light" ? "" : ""}
                            </button>
                        </div>
                    </div>
                )}
            </div>
            <div className="w-full h-[0.5px] bg-[#ccc]"></div>
            <div className="w-full h-[2vw]" style={headerStyle}>
                <div className="flex flex-row justify-start space-x-[10px]" style={containerStyle}>
                    <a href="/#" className="w-auto h-[1.8vw] flex justify-center items-center 
                        hover:bg-[#ccc] px-[2px]  text-[0.9vw]" style={subTextStyle}>
                        {headerStrings.hotel || "Hotel"}
                    </a>
                    <a href="/#" className="w-auto h-[1.8vw] flex justify-center items-center 
                        hover:bg-[#ccc] px-[2px]  text-[0.9vw]" style={subTextStyle}>
                        {headerStrings.flight || "Flights"}
                    </a>
                    <a href="/#" className="w-auto h-[1.8vw] flex justify-center items-center 
                        hover:bg-[#ccc] px-[2px]  text-[0.9vw]" style={subTextStyle}>
                        {headerStrings.busandshuttle || "Bus & Shuttle"}
                    </a>
                    <a href="/#" className="w-auto h-[1.8vw] flex justify-center items-center 
                        hover:bg-[#ccc] px-[2px]  text-[0.9vw]" style={subTextStyle}>
                        {headerStrings.airporttransfer || "Airport Transfer"}
                    </a>
                    <a href="/#" className="w-auto h-[1.8vw] flex justify-center items-center 
                        hover:bg-[#ccc] px-[2px]  text-[0.9vw]" style={subTextStyle}>
                        {headerStrings.carrental || "Car Rental"}
                    </a>
                    <a href="/#" className="w-auto h-[1.8vw] flex justify-center items-center 
                        hover:bg-[#ccc] px-[2px]  text-[0.9vw]" style={subTextStyle}>
                        {headerStrings.thingstodo || "Things to Do"}
                    </a>
                </div>
            </div>
            <div className="w-full h-[0.5px] bg-[#ccc]"></div>

            {/* Mobile search panel (slides down) */}
            {isMobile && mobileSearchOpen && (
                <div style={{ padding: "8px 16px", borderTop: `1px solid ${theme.border}`, background: theme.bg }}>
                    <form onSubmit={onSearch} style={{ display: "flex", gap: 8 }}>
                        <input
                            aria-label={headerStrings.searchPlaceholder || "Search"}
                            value={query}
                            onChange={(e) => setQuery(e.target.value)}
                            placeholder={headerStrings.searchPlaceholder || "Search..."}
                            style={{
                                flex: 1,
                                padding: "8px 12px",
                                borderRadius: 8,
                                border: `1px solid ${theme.border}`,
                                background: theme.card,
                                color: theme.text,
                            }}
                        />
                        <button type="submit" style={buttonStyle}>
                            {headerStrings.searchButton || "Search"}
                        </button>
                    </form>
                </div>
            )}

            {/* Mobile menu panel */}
        </header>
    );
}

export default Header;