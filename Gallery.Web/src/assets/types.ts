export type ManagerObjectType = 'images' | 'collections' | 'tags'

export type TagFormFields = {
    tagId: string,
    name: string
}
export type ImageFormFields = {
    imageId: string,
    title?: string,
    description?: string,
    imageCollectionId?: string,
    tagIds: string[]
}
export type ImageCollectionFormFields = {
    imageCollectionId: string,
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
export type ImageCollection = {
    imageCollectionId: string,
    name: string,
    description?: string,
    images: Image[]
}