import React, { useState, useEffect } from "react"
import Modal from "./Modal";
import axios from 'axios';

export default function Collection({id}){
    
    const [collection, setCollection] = useState([]);
    const [clickedImage, setClickedImage] = useState(null);
    const [currentIndex, setCurrentIndex] = useState(null);
    const backdropDiv = document.createElement("div");
    backdropDiv.id = "backdrop";
    backdropDiv.classList.add("fadeInFilter");
    
    useEffect(() => {
        getImageCollection();
        
    }, []);

    function getImageCollection(){
        // getCollection(id)
        axios.get('https://localhost:6001/api/ImageCollections/'+id)
        .then(response => {
            console.log(response.data);
            setCollection(response.data.images)
        })
        .catch(error => console.log(error));
    }

    const handleClick = (image, index) => {
        setCurrentIndex(index);
        setClickedImage(image.imageSrc);
        document.body.appendChild(backdropDiv);
        document.body.classList.add("stop-scrolling")

    }

    const handleRotationRight = () => {
        const totalLength = collection.length;
        if(currentIndex +1 >= totalLength){
            setCurrentIndex(0);
            const newImageSrc = collection[0].imageSrc;
            setClickedImage(newImageSrc);
            return;
        }
        const newIndex = currentIndex+1;
        setCurrentIndex(newIndex);
        const newUrl = collection[newIndex].imageSrc;
        setClickedImage(newUrl);
    }
    const handleRotationLeft = () => {
        const totalLength = collection.length;
        if(currentIndex === 0){
            setCurrentIndex(totalLength-1);
            const newImageSrc = collection[totalLength-1].imageSrc;
            setClickedImage(newImageSrc);
            return;
        }
        const newIndex = currentIndex-1;
        setCurrentIndex(newIndex);
        const newUrl = collection[newIndex].imageSrc;
        setClickedImage(newUrl);
    }

    return(
        <>
        <div className="collection-container">
            {collection.map((image, index) => {
                return(
                    
                    <img key={image.id} className="collection-thumbnail-img" src={image.imageSrc} onClick={() => handleClick(image, index)}/>
                )
            })}
        </div>
        {clickedImage != null ? 
            <Modal 
                imageSrc={clickedImage} 
                setClickedImage={setClickedImage} 
                handleRotationRight={handleRotationRight} 
                handleRotationLeft={handleRotationLeft}
                initialIsVisible={true}
            />
            : <></>}
        </>
        )
}