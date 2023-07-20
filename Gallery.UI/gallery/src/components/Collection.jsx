import React, { useState, useEffect } from "react"
import axios from 'axios';

export default function Collection({id}){
    
    const [collection, setCollection] = useState([]);

    useEffect(() => {
        getImageCollection();
    }, []);

    function getCollection(id) {
    }
    function getImageCollection(){
        // getCollection(id)
        axios.get('https://localhost:6001/api/ImageCollections/'+id)
        .then(response => {
            console.log(response.data);
            setCollection(response.data.images)
        })
        .catch(error => console.log(error));
    }

    
    // const galleryAPI = (url = 'https://localhost:6001/api/Images') => {
    //     return {
    //         getCollections: () => axios.get('https://localhost:6001/api/ImageCollections'),
    //         fetchAll: () => axios.get(url),
    //         // create: newRecord => axios.post(url, newRecord),
    //         create: newRecord => axios.post(url, newRecord, {
    //             headers: {
    //                 'Content-Type': 'multipart/form-data'
    //             }
    //         }),
    //         update: (id, updatedRecord) => axios.put(url + "/" + id, updatedRecord, {
    //             headers: {
    //                 'Content-Type': 'multipart/form-data'
    //             }
    //         }),
    //         delete: id => axios.delete(url + "/" + id),
    //     }
    // }
    return(
        <>
        <div>
            {collection.map((i) => {
                // <img key={i.}></img>
                return(
                    
                    <img id={i.id} src={i.imageSrc} width={300} height={300}/>
                )
            })}
        </div>
        </>
        )
}