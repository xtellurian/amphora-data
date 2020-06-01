export interface StringToEntityMap<T> {
  [id: string]: T;
}

export interface StateCachedFromServer {
  lastLoaded?: Date | undefined;
}

export function emptyCache<T>(): StringToEntityMap<T> {
  return {};
}
