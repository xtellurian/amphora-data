import { DetailedAmphora } from "amphoradata";

export const getPoints = (amphora: DetailedAmphora[]): MapPoint[] => {
    return amphora
        .filter((a) => a.lat && a.lon)
        .map((a) => {
            return {
                label: a.name,
                lat: a.lat as number,
                lon: a.lon as number,
            };
        });
};

export interface Point {
    lat: number;
    lon: number;
}
export interface MapPoint extends Point {
    label: string;
}
