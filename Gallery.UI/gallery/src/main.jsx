import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.jsx'
import './index.css'
// import {
//     createBrowserRouter,
//     RouterProvider,
//     Route,
//     Link,
//   } from "react-router-dom";
// import AdminPage from './components/AdminPage.jsx';
// import IndexPage from './components/IndexPage.jsx';
// import ErrorPage from './ErrorPage.jsx';


import { BrowserRouter as Router } from 'react-router-dom';
// const router = createBrowserRouter([
//     {
//       path: "/",
//       element: <App />,
//       errorElement: <ErrorPage />,
//       children: [
//         {
//           path: "/",
//           element: <IndexPage />,
//         },
//         {
//           path: "/admin", //routing för enskild receptsida
//           element: <AdminPage />,
//         }
//       ],
//     },
//   ]);

ReactDOM.createRoot(document.getElementById('root')).render(
    <Router>
        <App/>
    </Router>
)
// <React.StrictMode>
{/* <RouterProvider router={router}/> */}
//  </React.StrictMode>
