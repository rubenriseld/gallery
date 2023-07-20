import { useOktaAuth } from '@okta/okta-react';
import { NavLink } from 'react-router-dom/cjs/react-router-dom.min';

        
export default function Header(){
    const { oktaAuth, authState } = useOktaAuth();
    const logout = async () => { await oktaAuth.signOut(); }
    const login = async () => { await oktaAuth.signInWithRedirect({originalUri: "/admin"}); }

    const userText = authState?.isAuthenticated
        ? <button onClick={logout}>Logout</button>
        : <button onClick={login}>Sign In</button>
    return(
        <>
        <header>

        <nav className="menu">
        <NavLink to="/">Home</NavLink>
        <NavLink to="/admin">Private</NavLink>
      </nav>
      {userText}
        </header>
        </>
    )
}