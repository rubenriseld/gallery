import React, { useState, useEffect } from 'react';
import ImageEditor from './ImageManager';
import ImageCollectionManager from './ImageCollectionManager';
import SignIn from './SignIn';



export default function AdminPage() {
    const [selectedEditor, setSelectedEditor] = useState("images");
    
    return(
        <>
        <SignIn></SignIn>
            <div className='manager-page'>
                <div className='manager-header'>
                    <h1 className='manager-title'>Gallery Manager</h1>
                    <div className='manager-selector'>
                        <button onClick={x => setSelectedEditor("images")} className={selectedEditor == "images" ? "selected" : ""}>Images</button>
                        <button onClick={x => setSelectedEditor("imageCollections")} className={selectedEditor == "imageCollections" ? "selected" : ""}>Image Collections</button>
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