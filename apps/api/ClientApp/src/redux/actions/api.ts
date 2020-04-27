import { IAction } from "./action";

export const API_REQUEST = '[app] API Request';

export const apiRequest = (method: string, url: string, body: any, onSuccess: string, onError: string) => ({
  type: API_REQUEST,
  payload: body,
  meta: { method, url, onSuccess, onError }
}); 