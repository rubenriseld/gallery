import { useOktaAuth } from '@okta/okta-react';
import { Link } from 'react-router-dom';

export default function SignIn(){
    const { oktaAuth, authState } = useOktaAuth();
    const logout = async () => { await oktaAuth.signOut(); }
    const login = async () => { await oktaAuth.signInWithRedirect({originalUri: "/admin/manager"}); }

    const userText = authState?.isAuthenticated
        ?  
            <button className='admin-sign-out-btn' onClick={logout}>Sign Out</button>
        :   <>
                <button className='admin-sign-in-btn' onClick={login}>Sign In</button>
                <p className='admin-info'>Please sign in to use the admin manager.</p>
            </>  



    return(
        <>
            {userText}
            
        </>
    )
}