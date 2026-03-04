import React from "react";
import leftImageSrc from "../../assets/images/Auth/leftImageSrc.png";

type Props = {
    children: React.ReactNode;
    title?: string;
    centerVertically?: boolean;
};

const AuthLayout: React.FC<Props> = ({ children, title, centerVertically = true }) => {
    return (
        <div className="fixed inset-0 flex bg-transparent">
            <div
                className="hidden md:block md:w-1/2 min-h-screen relative"
                style={{ backgroundColor: "#0ea5a1" }}
            >
                <img src={leftImageSrc} alt="" className="h-full w-auto absolute top-0" />
            </div>

            <div
                className={`w-full md:w-1/2 min-h-screen flex ${centerVertically ? "items-center" : "items-start"} justify-center py-8`}
                style={{ backgroundColor: "#ffffff" }}
            >
                <div className="w-full px-4 sm:px-8 md:px-12 max-w-lg">
                    {children}
                </div>
            </div>
        </div>
    );
};

export default AuthLayout;