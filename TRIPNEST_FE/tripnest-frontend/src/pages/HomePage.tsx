import React, { useEffect, useState } from "react";
import Header from "../shared/components/Header";
import imageBanner from "../assets/images/Home/BANNER_TRIPNEST_LANDING (1).png"

const HomePage: React.FC = () => {
    const [isMobile, setIsMobile] = useState<boolean>(() => {
        if (typeof window === "undefined") {
            return false;
        }
        return window.innerWidth < 768;
    })

    useEffect(() => {
        const handleResize = () => {
            setIsMobile(window.innerWidth < 768);
        }
        window.addEventListener("resize", handleResize);
        return () => {
            window.removeEventListener("resize", handleResize);
        }
    }, []);

    return (
        <div className="min-h-screen flex flex-col">
            <Header />
            <main className="flex-1">
                <div className="w-full h-[35vw] relative overflow-hidden">
                    <img src={imageBanner} alt="" className="absolute bottom-0" />
                    <div className="w-full h-[25vw] absolute top-0 bg-gradient-to-b from-[#00000060] to-transparent"></div>
                </div>
            </main>

        </div>
    );
}

export default HomePage;