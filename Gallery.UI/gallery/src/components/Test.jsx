import React, {useState, useEffect } from "react"

const defaultImageSrc = '/img/webp.png';
const initialFieldValues = {
    imageId: 0,
    imageTitle: "",
    imageDescription: "",
    imageCollectionId: 0,
    imageSrc: defaultImageSrc,
    imageFile: null
}

export default function Test(props) {

    const {addOrEdit} = props;
    const [values, setValues] = useState(initialFieldValues);
    const [errors, setErrors] = useState({});
    const handleInputChange = e => {
        const {name, value } = e.target;
        setValues({
            ...values,
            [name]:value
        })
    }
    //set preview image on upload
    const showPreview = e => {
        if(e.target.files && e.target.files[0]){
            let imageFile = e.target.files[0];
            const reader = new FileReader();
            reader.onload = x => {
                setValues({
                    ...values,
                    imageFile: imageFile,
                    imageSrc: x.target.result
                })
            }
            reader.readAsDataURL(imageFile);
        }
        //if file upload canceled (not working on firefox)
        else{
            e.target.value = null;
            console.log("canceled");
            setValues({
                ...values,
                imageFile: null,
                imageSrc: defaultImageSrc
            })
        }
    }

    const validate =() => {
        let temp = {};
        temp.imageTitle = values.imageTitle==""?false:true;
        temp.imageSrc = values.imageSrc==defaultImageSrc?false:true;
        setErrors(temp);
        return Object.values(temp).every(x => x==true);
    }
    const resetForm = () => {
        setValues(initialFieldValues);
        document.getElementById('image-uploader').value = null;
        setErrors({});
    }
    const handleFormSubmit = e => {
        e.preventDefault();
        if(validate()){
            const formData = new FormData();
            formData.append("imageTitle",values.imageTitle);
            formData.append("imageDescription",values.imageTitle);
            formData.append("imageCollectionId",values.imageTitle);
            formData.append("imageName",values.imageName);
            formData.append("imageSrc",values.imageSrc);
            addOrEdit(formData.resetForm);
        }
        else{
            alert("invalid input");
        }
    }
    return(
        <>
            <form autoComplete="off" noValidate onSubmit={handleFormSubmit}>
                <div>
                    <img src={values.imageSrc} width="200px"/>
                    <input type="file" accept="image/*" autoComplete="off" onChange={showPreview} id="image-uploader"/> 
                </div>
                <div>
                    <input placeholder="Image Title" name="imageTitle" value={values.imageTitle} onChange={handleInputChange}/>   
                    <input placeholder="Image Description" name="imageDescription" value={values.imageDescription} onChange={handleInputChange}/>   
                    <input placeholder="Image Collection Id" name="imageCollectionId" value={values.imageCollectionId} onChange={handleInputChange}/>   
                    {/* <input placeholder="Image Src" name="imageSrc" value={values.imageSrc} onChange={handleInputChange}/>   */}
                    <button type="submit">submit</button>
                </div>
                
            </form>
        </>
    )
}