import React, {useState, useEffect } from "react"

const defaultImageSrc = '/img/webp.png';
const initialFieldValues = {
    id: 0,
    title: "",
    description: "",
    imageCollectionId: 0,
    imageName: "",
    imageSrc: defaultImageSrc,
    imageFile: null
}

export default function Test(props) {

    const {addOrEdit, recordForEdit} = props;
    
    const [values, setValues] = useState(initialFieldValues);
    const [errors, setErrors] = useState({});
    const handleInputChange = e => {
        const {name, value } = e.target;
        setValues({
            ...values,
            [name]:value
        })
    }
    
    useEffect(()=>{
        if(recordForEdit != null){

            console.log("record for edit test:")
            console.log(recordForEdit)
            setValues(recordForEdit);
        }
        
    }, [recordForEdit])



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
        temp.title = values.title==""?false:true;
        temp.description = values.description==""?false:true;
        temp.imageCollectionId = values.imageCollectionId==""?false:true;
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
            var formData = new FormData();
            formData.append("id",values.id);
            formData.append("title",values.title);
            formData.append("description",values.description);
            formData.append("imageCollectionId",values.imageCollectionId);
            formData.append("imageName",values.imageName);
            formData.append("imageFile",values.imageFile);
            addOrEdit(formData, resetForm);
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
                <p>id: {values.id}</p>
                <div>
                    <div>
                    <p>image title</p>
                    <input placeholder="Image Title" name="title" value={values.title} onChange={handleInputChange}/>   
                    </div>
                    <div>
                    <p>image description</p>
                    <input placeholder="Image Description" name="description" value={values.description} onChange={handleInputChange}/>   

                    </div>
                    <div>
                    <p>colleciton id</p>
                    <input placeholder="Image Collection Id" name="imageCollectionId" value={values.imageCollectionId} onChange={handleInputChange}/>   
                    </div>
                    <button type="submit">submit</button>
                </div>
            </form>

            
        </>
    )
}