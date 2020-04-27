
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface AmphoraState {
    isLoading: boolean;
    startDateIndex?: number;
    amphoras: AmphoraModel[];
}

export interface AmphoraModel {
    id: string;
    name: string;
    price: number;
    description: string;
}
