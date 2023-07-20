import { NavLink, Route } from 'react-router-dom/cjs/react-router-dom.min';

export default function Header() {

    return (
        <>
            <header>

                <h2 className="logo">Ruben Riseld</h2>
                <nav className="menu">
                    <NavLink to="/">Home</NavLink>
                    <NavLink to="/testCollection">Collection</NavLink>
                </nav>
            </header>
        </>
    )
}