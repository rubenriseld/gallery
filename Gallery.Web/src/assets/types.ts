export type ButtonType = 'primary' | 'secondary' | 'warning';
export type ModalType = 'confirm' | 'warning';

export type ImagePreview = {
    imageFile: File,
    imageSrc: string
}

export type ReorderImage = {
    imageId: string,
    orderInImageCollection: number
}

export type ImageCollection = {
    imageCollectionId: string,
    name: string,
    description?: string,
    shouldBeDisplayed: boolean,
    coverImage?: Image,
    images: Image[]
}
export type CreateImageCollection = {
    name: string,
    description?: string,
    shouldBeDisplayed: boolean
}
export type UpdateImageCollection = {
    coverImageId?: string,
    name: string,
    description?: string,
    shouldBeDisplayed: boolean,
    reorderImages: ReorderImage[]
}



export type Tag = {
    tagId: string,
    name?: string
}
export type CreateTag = {
    name?: string
}
export type UpdateTag = CreateTag

export type Image = {
    imageId: string,
    uri: string,
    title?: string,
    description?: string,
    imageCollectionId?: string,
    imageCollectionName?: string,
    orderInImageCollection: number,
    sold: boolean,
    tags: Tag[]
}
export type UpdateImage = {
    title?: string,
    description?: string,
    imageCollectionId?: string,
    sold: boolean,
    tagIds?: string[]
}
export type CreateImage = {
    images: File[]
}
