import React, { useState, useEffect } from "react"
import axios from 'axios';



export default function ImageManager() {
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

    const [values, setValues] = useState(initialFieldValues);
    const [errors, setErrors] = useState({});

    const [imageList, setImageList] = useState([])
    const [collectionList, setCollectionList] = useState([]);
    const [recordForEdit, setRecordForEdit] = useState(null);

    useEffect(() => {
        refreshImageList();
        getImageCollections();
    }, []);

    function refreshImageList() {
        galleryAPI().fetchAll()
            .then(response => {
                // console.log(response.data);
                setImageList(response.data)
            })
            .catch(error => console.log(error));
    }
    function getImageCollections(){
        galleryAPI().getCollections()
        .then(response => {
            console.log(response.data);
            setCollectionList(response.data)
        })
        .catch(error => console.log(error));
    }

    useEffect(() => {
        if (recordForEdit != null) {
            // console.log("record for edit test:")
            // console.log(recordForEdit)
            setValues(recordForEdit);
        }
    }, [recordForEdit])

    //axios methods
    // const galleryAPI = (url = 'https://localhost:6001/api/Images') => {
        
        const galleryAPI = (url = 'https://galleryapiappservice.azurewebsites.net/Images') => {
        return {
            // getCollections: () => axios.get('https://localhost:6001/api/ImageCollections'),
            getCollections: () => axios.get('https://galleryapiappservice.azurewebsites.net/ImageCollections'),
            fetchAll: () => axios.get(url),
            // create: newRecord => axios.post(url, newRecord),
            create: newRecord => axios.post(url, newRecord, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            }),
            update: (id, updatedRecord) => axios.put(url + "/" + id, updatedRecord, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            }),
            delete: id => axios.delete(url + "/" + id),
        }
    }

    //set preview image on upload
    const showPreview = e => {
        if (e.target.files && e.target.files[0]) {
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
        else {
            e.target.value = null;
            // console.log("canceled");
            setValues({
                ...values,
                imageFile: null,
                imageSrc: defaultImageSrc
            })
        }
    }

    //validator for form input (NOT FINISHED)
    const validate = () => {
        let temp = {};
        temp.title = values.title == "" ? false : true;
        temp.description = values.description == "" ? false : true;
        temp.imageCollectionId = values.imageCollectionId == "" ? false : true;
        temp.imageSrc = values.imageSrc == defaultImageSrc ? false : true;
        setErrors(temp);
        return Object.values(temp).every(x => x == true);
    }

    const handleInputChange = e => {
        const { name, value } = e.target;
        setValues({
            ...values,
            [name]: value
        })
    }
    const handleSelectChange = e => {
        console.log(e.target.value);
        const value = e.target.value;
        setValues({
            ...values,
            imageCollectionId: value
        })
        // console.log(values.imageCollectionId);
    }

    const handleFormSubmit = e => {
        e.preventDefault();
        if (validate()) {
            var formData = new FormData();
            formData.append("id", values.id);
            formData.append("title", values.title);
            formData.append("description", values.description);
            formData.append("imageCollectionId", values.imageCollectionId);
            formData.append("imageName", values.imageName);
            formData.append("imageFile", values.imageFile);
            addOrEdit(formData, resetForm);
            console.log(formData);
        }
        else {
            alert("invalid input");
        }
    }

    const resetForm = () => {
        setValues(initialFieldValues);
        document.getElementById('image-uploader').value = null;
        setErrors({});
        var select = document.querySelector('#imageCollectionSelect');
        select.value = 0;
    }

    //set form to selected record
    const showRecordDetails = (data) => {
        // console.log("record for edit:");
        // console.log(data);
        setRecordForEdit(data);
        var select = document.querySelector('#imageCollectionSelect');
        select.value = data.imageCollectionId;
    }

    //delete an image record
    const onDelete = (e, id) => {
        e.stopPropagation();
        if (window.confirm('are you sure you wanna delete this???')) {
            galleryAPI().delete(id)
                .then(result => refreshImageList())
                .catch(error => console.log(error));

        }
    }

    const addOrEdit = (formData, onSuccess) => {
        // console.log("id: " + formData.get("id"));
        //create
        if (formData.get("id") == "0") {

            galleryAPI().create(formData)
                .then(response => {
                    console.log(response);
                    onSuccess();
                    refreshImageList();
                })
                .catch(error => console.log(error));
        }
        //update
        else {
            galleryAPI().update(formData.get("id"), formData)
                .then(response => {
                    console.log(response);
                    onSuccess();
                    refreshImageList();
                })
                .catch(error => console.log(error));
        }
    }

    //image cards in list of records
    const imageCard = (data) => {
        return (
            <div onClick={() => showRecordDetails(data)}>
                <img className="record-image"src={data.imageSrc}></img>
                <div className="record-details-container">
                    <div className="record-details">
                        <p>id: {data.id}</p>
                        <h5>{data.title}</h5>
                        <span>{data.description}</span>
                    </div>
                    <button className='record-delete-btn' onClick={e => onDelete(e, parseInt(data.id))}><span className="material-symbols-outlined">delete</span></button>
                </div>
            </div>
        )
    }

    return (
        <>
            <div className="image-manager-container">
                <form className="image-manager-form" autoComplete="off" noValidate onSubmit={handleFormSubmit}>
                    <div>
                        <img src={values.imageSrc} width="200px" />
                        <input type="file" accept="image/*" autoComplete="off" onChange={showPreview} id="image-uploader" />
                    </div>
                    <p>id: {values.id}</p>
                    <div>
                        <div>
                            <p>image title</p>
                            <input placeholder="Image Title" name="title" value={values.title} onChange={handleInputChange} />
                        </div>
                        <div>
                            <p>image description</p>
                            <input placeholder="Image Description" name="description" value={values.description} onChange={handleInputChange} />
                        </div>
                        <div>
                            <p>colleciton id</p>
                            {/* <input placeholder="Image Collection Id" name="imageCollectionId" value={values.imageCollectionId} onChange={handleInputChange} /> */}
                            <select id="imageCollectionSelect" name="imageCollectionId" onChange={handleSelectChange} defaultValue={0}>
                                <option disabled value={0}>Select a collection</option>
                                {collectionList.map((i)=>
                                    <option key={i.id} value={i.id}>{i.name}</option>
                                    )}
                            </select>
                        </div>
                        <button type="submit">submit</button>
                    </div>
                </form>
                <section className="image-results-container">
                    {/* <h3 className="image-results-title">results</h3> */}
                    {
                        imageList.map((e, i) =>
                        <div className="image-card" key={i}>{imageCard(imageList[i])}</div>
                        )
                    }
                </section>
            </div>
        </>
    )
}