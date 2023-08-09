import { LoginCallback, SecureRoute, Security } from '@okta/okta-react';
import { OktaAuth, toRelativeUrl } from '@okta/okta-auth-js';
import { useHistory } from 'react-router-dom';
import { Route } from 'react-router-dom';
import AdminPage from './components/AdminPage';
import IndexPage from './components/IndexPage';
import Header from './components/Header';
import SignIn from './components/SignIn';
import Collection from './components/Collection.jsx';

// const issuer = import.meta.env.VITE_ISSUER;
// const clientId = import.meta.env.VITE_CLIENT_ID;
const issuer = process.env.VITE_ISSUER;
const clientId = process.env.VITE_CLIENT_ID;

const oktaAuth = new OktaAuth({
    issuer: issuer,
    clientId: clientId,
    redirectUri: window.location.origin + '/login/callback'
});

export default function App() {
    const history = useHistory();
    const restoreOriginalUri = async (_oktaAuth, originalUri) => {
        history.replace(toRelativeUrl(originalUri, window.location.origin))
    };

    return (
        <>
            <Security oktaAuth={oktaAuth} restoreOriginalUri={restoreOriginalUri}>
                {/* add prop to header for signin/out-btn */}
            <Header />

                <Route path='/' component={LoginCallback} />
                <Route path='/' exact={true} component={IndexPage} />
                <Route path='/testCollection'  component={() => <Collection id={2} />} />
                <Route path='/admin' exact={true} component={SignIn} />
                <SecureRoute path='/admin/manager' exact={true} component={AdminPage} />
            </Security>
        </>
    )
}
