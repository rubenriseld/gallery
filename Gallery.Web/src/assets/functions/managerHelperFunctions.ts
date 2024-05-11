export function shouldRenderInput(formDataIndex: string): boolean {
    const excludedFormFields = ['tagId', 'imageId', 'imageCollectionId', 'tagIds', 'coverImageId', 'uri', 'imageCollectionName', 'coverImage', 'images']
    return !excludedFormFields.includes(formDataIndex)
}
export function capitalize(string: string): string {
    return (string).charAt(0).toUpperCase() + (string).slice(1)
}