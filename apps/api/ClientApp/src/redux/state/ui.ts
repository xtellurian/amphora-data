import { DetailedAmphora } from "amphoradata";

export interface UiState {
    isAmphoraDetailOpen: boolean;
    current: DetailedAmphora | undefined;
}