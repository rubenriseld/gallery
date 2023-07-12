import '../App.css'
import React, { useState, useEffect } from 'react';
import ImageEditor from './ImageManager';
import ImageCollectionManager from './ImageCollectionManager';

export default function AdminPage() {
    const [selectedEditor, setSelectedEditor] = useState("images");

    return(
        <>
            <div className='manager-page'>
                <div className='manager-header'>
                    <h1 className='manager-title'>Gallery Manager</h1>
                    <div className='manager-selector'>
                        <button onClick={x => setSelectedEditor("images")}>Images</button>
                        <button onClick={x => setSelectedEditor("imageCollections")}>Image Collections</button>
                    </div>
                </div>
                <section className='manager-section'>
                    {selectedEditor == "images" ?
                        <ImageEditor></ImageEditor>
                        :
                        <ImageCollectionManager></ImageCollectionManager>
                    }
                </section>
            </div>
        </>
    )
}