export type ButtonType = 'primary' | 'secondary' | 'warning'

export type TagFormFields = {
    tagId: string,
    name: string
}
export type ImageFormFields = {
    uri: string,
    imageId: string,
    title?: string,
    description?: string,
    imageCollectionId?: string,
    tagIds?: string[]
}
export type ImageCollectionFormFields = {
    imageCollectionId: string,
    coverImageId?: string,
    name: string,
    description?: string,
}

export type Tag = {
    tagId: string,
    name: string
}
export type Image = {
    imageId: string,
    uri: string,
    title?: string,
    description?: string,
    imageCollectionId?: string,
    imageCollectionName?: string,
    tags: Tag[]
}
export type ImagePreview = {
    imageFile : File,
    imageSrc : string
}
export type ImageCollection = {
    imageCollectionId: string,
    name: string,
    description?: string,
    coverImage?: Image,
    images: Image[]
}