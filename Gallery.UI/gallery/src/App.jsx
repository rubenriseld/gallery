import './App.css'
import React, { useState, useEffect } from 'react';
import Test from './components/Test.jsx';
import axios from 'axios';
import ImageEditor from './components/ImageManager';
import ImageCollectionManager from './components/ImageCollectionManager';

import {
    NavLink,
    Outlet
  } from 'react-router-dom'

export default function App() {
    return (
        <>
        <header>
            <NavLink to="/">home</NavLink>
            <NavLink to="/admin">admin</NavLink>
        </header>
        <main>
            <Outlet/>
        </main>
        </>
    )
}
