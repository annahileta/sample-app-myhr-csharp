export function popSavedDataFromSrorage(key: string): string {
    const savedData = sessionStorage.getItem(key)

    if (savedData) {
        sessionStorage.removeItem(key)
    }
    return savedData
}
