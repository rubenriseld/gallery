import type { Tag } from './tag';

export type ImageFormFields = {
    imageId: string,
    title?: string,
    description?: string,
    imageCollectionId?: string,
    tagIds: string[]
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