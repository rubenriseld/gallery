import React, { useState, useEffect, useRef } from "react"

export default function Modal({imageSrc, handleRotationRight, handleRotationLeft, setClickedImage, initialIsVisible}) {
    const [isComponentVisible, setIsComponentVisible] = useState(initialIsVisible);
    const ref = useRef(null);

    const [touchStart, setTouchStart] = useState(null)
    const [touchEnd, setTouchEnd] = useState(null)

    // the required distance between touchStart and touchEnd to be detected as a swipe
    const minSwipeDistance = 50 

    const onTouchStart = (e) => {
    setTouchEnd(null) // otherwise the swipe is fired even with usual touch events
    setTouchStart(e.targetTouches[0].clientX)
    }

    const onTouchMove = (e) => setTouchEnd(e.targetTouches[0].clientX)

    const onTouchEnd = () => {
    if (!touchStart || !touchEnd) return
    const distance = touchStart - touchEnd
    const isLeftSwipe = distance > minSwipeDistance
    const isRightSwipe = distance < -minSwipeDistance
    if (isLeftSwipe || isRightSwipe){
        console.log('swipe', isLeftSwipe ? 'left' : 'right')
        isLeftSwipe ? handleRotationLeft(): handleRotationRight()
    }
    // add your conditional logic here
    }

    useEffect(() =>{
        const Clickout = (e) => {
            if (ref.current && !ref.current.contains(e.target)) {
                const backdropDiv = document.querySelector("#backdrop");
                backdropDiv.classList.add("fadeOutFilter");
                document.body.classList.remove("stop-scrolling")
                setTimeout(function() {
                    document.body.removeChild(backdropDiv);
                }, 300);
                setIsComponentVisible(false);
                setClickedImage(null);
            }
        };
        document.addEventListener("mousedown", Clickout);
        // document.addEventListener("scroll", Clickout);
        return () => {
            document.removeEventListener("mousedown", Clickout);
        // document.removeEventListener("scroll", Clickout);
        };
    }, [ref]);

    const handleClick = (e) => {
        // if(e.target.classList.contains("lightbox-btn-close")){
            const backdropDiv = document.querySelector("#backdrop");
            backdropDiv.classList.add("fadeOutFilter");
            document.body.classList.remove("stop-scrolling")

            setTimeout(function() {
                document.body.removeChild(backdropDiv);
            }, 300);
            setClickedImage(null);
        // }
    }


    return(
        <>
            
        <div id="lightbox" ref={ref} >
            
            <img className="lightbox-img" src={imageSrc}  onTouchStart={onTouchStart} onTouchMove={onTouchMove} onTouchEnd={onTouchEnd}></img>
            <button className="lightbox-btn-left" onClick={handleRotationLeft}>
                <span className="material-symbols-outlined">arrow_back_ios</span>
            </button>
            <button className="lightbox-btn-close" onClick={handleClick}>
                <span className="material-symbols-outlined">close</span>
            </button>
            <button className="lightbox-btn-right" onClick={handleRotationRight}>
                <span className="material-symbols-outlined">arrow_forward_ios</span>
            </button>

        </div>
        </>
    )
}