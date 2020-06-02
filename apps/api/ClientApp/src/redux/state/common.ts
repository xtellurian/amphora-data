export interface StringToEntityMap<T> {
  [id: string]: T;
}

export interface Cache<T> {
  store: StringToEntityMap<T>;
  lastUpdated?: Date;
}

export function emptyCache<T>(): Cache<T> {
  return {
    store: {}
  };
}
