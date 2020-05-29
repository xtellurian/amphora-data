
export interface ColumnElement {
    key: string;
    name: string;
}

export interface RowElement {
    id: string;
    [index: string]: string | number | boolean;
    length: number;
}
