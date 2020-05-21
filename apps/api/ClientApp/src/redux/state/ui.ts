import { DetailedAmphora } from "amphoradata";

export const SUCCESS = "SUCCESS";
export const INFO = "INFO";
export const WARNING = "WARNING";
export const ERROR = "ERROR";

type AlertLevel = (typeof SUCCESS) | (typeof INFO)| (typeof WARNING)| (typeof ERROR);

export interface Alert {
    id: string;
    content: string;
    level: AlertLevel;
}

export interface UiState {
    isAmphoraDetailOpen: boolean;
    current: DetailedAmphora | undefined;
    alerts?: Alert[];
}