import api from "@/api"
import type { ImageFormFields , ImageCollectionFormFields, TagFormFields } from "@/assets/types"

export async function updateCollection(imageCollection: ImageCollectionFormFields) {
    await api.put('imageCollections/' + imageCollection.imageCollectionId, JSON.stringify(imageCollection))
}
export async function createCollection(imageCollection: ImageCollectionFormFields) {
    await api.post('imageCollections/', JSON.stringify(imageCollection))
}
export async function deleteCollection(imageCollection: ImageCollectionFormFields) {
    await api.delete('imageCollections/' + imageCollection.imageCollectionId)
}

export async function updateImage(image: ImageFormFields) {
    await api.put('images/' + image.imageId, JSON.stringify(image))
}
export async function createImage(image: ImageFormFields) {
    await api.post('images/', image, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    })
}
export async function deleteImage(image: ImageFormFields) {
    await api.delete('images/' + image.imageId)
}

export async function updateTag(tag: TagFormFields) {
    await api.put('tags/' + tag.tagId, JSON.stringify(tag))
}
export async function createTag(tag: TagFormFields) {
    await api.post('tags/', JSON.stringify(tag))
}
export async function deleteTag(tag: TagFormFields) {
    await api.delete('tags/' + tag.tagId)
}