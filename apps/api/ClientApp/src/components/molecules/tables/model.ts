
export interface ColumnElement {
    key: string;
    name: string;
}

export interface RowElement {
    [index: string]: string | number | boolean;
    length: number;
}
