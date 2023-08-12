import { NavLink, Route } from 'react-router-dom/cjs/react-router-dom.min';
import { useState, useEffect } from "react";
import "./header.css";


export default function Header() {

    const [showMobileMenu, setShowMobileMenu] = useState(false);
    const [shadow, setShadow] = useState(false);
    
    const toggleBoxShadow = () => {
        window.addEventListener('scroll', () => {
            // currentScroll är användarens position i Y-led, från toppen av dokumentet 
            const currentScroll = window.scrollY;
            if (currentScroll > 10) {
                setShadow(true);
                return;
            }
            else{
                setShadow(false);
                return;
            }
        })
    }
    useEffect(() => {
        const handleScrollEvent = () => {
            toggleBoxShadow();
        }
        window.addEventListener('scroll', handleScrollEvent);
        return () => {
            window.addEventListener('scroll', handleScrollEvent);
        };
    }, []);

    return (
        <>
        <header className={`${shadow ? "header-shadow" : ""} `}>
            <div className='header-main'>

                    <NavLink to="/" onClick={()=> setShowMobileMenu(false)} className="logo">
                        Ruben Riseld
                    </NavLink>
                <button
                    className="burger-btn"
                    onClick={() => setShowMobileMenu(!showMobileMenu)}>

                    {!showMobileMenu? 
                        <span className="material-symbols-outlined">
                        menu
                        </span>:
                        <span className="material-symbols-outlined">
                        close
                        </span>
                    }
                </button>
                    
            </div>
                <nav className={`${showMobileMenu ? "menu-show" : "menu-hidden"} `}>

                    <NavLink to="/" onClick={()=> setShowMobileMenu(false)}>
                        Home
                    </NavLink>
                    <NavLink to="/testCollection" onClick={()=> setShowMobileMenu(false)}>
                        test collection
                    </NavLink>
                        </nav>
            </header>
        </>
    )
}