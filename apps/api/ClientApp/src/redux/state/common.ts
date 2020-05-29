export interface StringToEntityMap<T> {
    [id: string]: T;
}

export function emptyCache<T>(): StringToEntityMap<T> {
    return {};
} 