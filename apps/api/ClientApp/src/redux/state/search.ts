import { BasicAmphora } from "amphoradata";

export interface SearchQuery {
  term: string;
  page: number;
}
export interface SearchState {
  isLoading?: boolean;
  query: SearchQuery;
  results: BasicAmphora[];
}
