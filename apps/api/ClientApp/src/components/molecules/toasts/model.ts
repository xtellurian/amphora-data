import { ToastOptions } from "react-toastify";

export interface ToastContent {
    text: string;
    path?: string;
}

export type ToastTrigger = (content: ToastContent | string, options?: ToastOptions) => void