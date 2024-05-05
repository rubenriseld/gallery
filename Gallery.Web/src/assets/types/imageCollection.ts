import type { Image } from './image'

export type ImageCollectionFormFields = {
    imageCollectionId: string,
    name: string,
    description?: string,
}

export type ImageCollection = {
    imageCollectionId: string,
    name: string,
    description?: string,
    images: Image[]
}