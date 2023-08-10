import React, { useState, useEffect } from "react"
import axios from 'axios';


export default function ImageCollectionManager() {
    const initialFieldValues = {
        id: 0,
        name: ""
    }

    const [values, setValues] = useState(initialFieldValues);
    const [errors, setErrors] = useState({});

    const [collectionList, setCollectionList] = useState([]);
    const [recordForEdit, setRecordForEdit] = useState(null);

    useEffect(() => {
        refreshImageCollectionList();
    }, []);

    function refreshImageCollectionList() {
        galleryAPI().fetchAll()
        .then(response => {
            console.log(response.data);
            setCollectionList(response.data);
        })
    }

    useEffect(() => {
            if (recordForEdit != null) {
                // console.log("record for edit test:")
                // console.log(recordForEdit)
                setValues(recordForEdit);
            }
        }, [recordForEdit])

    const galleryAPI = (url = 'https://galleryapiappservice.azurewebsites.net/api/ImageCollections') => {
        return {
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
    const validate = () => {
        let temp = {};
        temp.name = values.name == "" ? false : true;
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

    const handleFormSubmit = e => {
        e.preventDefault();
        if (validate()) {
            var formData = new FormData();
            formData.append("id", values.id);
            formData.append("name", values.name);
            addOrEdit(formData, resetForm);
        }
        else {
            alert("invalid input");
        }
    }
    const resetForm = () => {
        setValues(initialFieldValues);
        setErrors({});
    }

    //set form to selected record
    const showRecordDetails = (data) => {
        // console.log("record for edit:");
        // console.log(data);
        setRecordForEdit(data);
    }
    const onDelete = (e, id) => {
        e.stopPropagation();
        if (window.confirm('are you sure you wanna delete this???')) {
            galleryAPI().delete(id)
                .then(result => refreshImageCollectionList())
                .catch(error => console.log(error));

        }
    }
    const addOrEdit = (formData, onSuccess) => {
        // console.log("id: " + formData.get("id"));
        //create
        if (formData.get("id") == "0") {

            galleryAPI().create(formData)
                .then(response => {
                    // console.log(response);
                    onSuccess();
                    refreshImageCollectionList();
                })
                .catch(error => console.log(error));
        }
        //update
        else {
            galleryAPI().update(formData.get("id"), formData)
                .then(response => {
                    console.log(response);
                    onSuccess();
                    refreshImageCollectionList();
                })
                .catch(error => console.log(error));
        }
    }

    //image cards in list of records
    const imageCollectionCard = (data) => {
        return (
            <div onClick={() => showRecordDetails(data)}>
                <div className="record-details-container">

                <div className="record-details">
                    <p>id: {data.id}</p>
                    <h5>{data.name}</h5>
                    {/* <span>{data.description}</span> */}
                    </div>
                    <button className='record-delete-btn' onClick={e => onDelete(e, parseInt(data.id))}><span className="material-symbols-outlined">delete</span></button>
                </div>
            </div>
        )
    }
    return (
        <>
        <div className="collection-manager-container">
            <form className="collection-manager-form"autoComplete="off" noValidate onSubmit={handleFormSubmit}>
                
                <p>id: {values.id}</p>
                <div>
                    <div>
                        <p>collection name</p>
                        <input placeholder="Collection Name" name="name" value={values.name} onChange={handleInputChange} />
                    </div>
                    
                    <button type="submit">submit</button>
                </div>
            </form>
            <div className="collection-results-container">
                {/* <h3 className="collection-results-title">results</h3> */}
                {
                    collectionList.map((e, i) =>
                        <div className="collection-card" key={i}>{imageCollectionCard(collectionList[i])}</div>
                    )
                }
            </div>
            </div>
        </>
    )
}