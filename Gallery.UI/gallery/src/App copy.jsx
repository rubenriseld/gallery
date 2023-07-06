import './App.css'
import React, {useState, useEffect} from 'react';
import Test from './components/Test.jsx';
import axios from 'axios';
import ImageEditor from './components/ImageManager';

export default function App() {
    // const [imageList, setImageList] = useState([])
    // const [recordForEdit, setRecordForEdit] = useState(null);

    // useEffect(()=>{
    //     refreshImageList();

    // }, []);

    // const galleryAPI = (url = 'https://localhost:6001/api/Images') => {
    //     return {
    //         fetchAll: () => axios.get(url),
    //         // create: newRecord => axios.post(url, newRecord),
    //         create: newRecord => axios.post(url, newRecord, {
    //             headers: {
    //                 'Content-Type': 'multipart/form-data'
    //             }
    //         }),
    //         update: (id, updatedRecord) => axios.put(url +"/"+ id, updatedRecord, {
    //             headers: {
    //                 'Content-Type': 'multipart/form-data'
    //             }
    //         }),
    //         delete: id => axios.delete(url+"/"+ id),
    //     }
    // }
    
    // function refreshImageList() {
    //     galleryAPI().fetchAll()
    //     .then(response => {
    //         // console.log(response.data);
    //         setImageList(response.data)})
    //     .catch(error => console.log(error));


    // }

    // const addOrEdit = (formData, onSuccess) => {
    //     console.log("id: " + formData.get("id"));
    //     //create
    //     if(formData.get("id")== "0"){

    //         galleryAPI().create(formData)
    //         .then(response => {
    //             // console.log(response);
    //             onSuccess();
    //             refreshImageList();
    //         })
    //         .catch(error => console.log(error));
    //     }
    //     //update
    //     else {
    //         galleryAPI().update(formData.get("id"),formData)
    //         .then(response => {
    //             console.log(response);
    //             onSuccess();
    //             refreshImageList();
    //         })
    //         .catch(error => console.log(error));
    //     }
    // }
    // const showRecordDetails = (data) => {
    //     console.log("record for edit:");
    //     console.log(data);
    //     setRecordForEdit(data);
    // }
    // const onDelete = (e, id) => {
    //     e.stopPropagation();
    //     if(window.confirm('are you sure you wanna delete this???')){
    //         galleryAPI().delete(id)
    //         .then(result => refreshImageList())
    //         .catch(error => console.log(error));

    //     }
    // }
    // const imageCard = (data) => {
    //     return(
    //     <div onClick={() => showRecordDetails(data)}>
    //         <img src={data.imageSrc} width="200px"></img>
    //         <div>
    //             <p>id: {data.id}</p>
    //             <h5>{data.title}</h5>
    //             <span>{data.description}</span>
    //             <button className='delete-button' onClick={e => onDelete(e, parseInt(data.id))}>delete</button>
    //         </div>
    //     </div>
    //     )
    // }
  return (
    <>
      <h1>Gallery App</h1>
      <div>
        <button>Images</button>
        <button>Image Collections</button>
      </div>
      <ImageEditor></ImageEditor>
      {/* <Test addOrEdit={addOrEdit} recordForEdit={recordForEdit}></Test>
      <div>
                <h1>results</h1>
                        {
                            // [...Array(Math.ceil(imageList.length/3))].map((e, i) =>
                            imageList.map((e, i) =>
                                <div key={i}>{imageCard(imageList[i])}</div>
                            )
                        }
            </div> */}
    </>
  )
}
