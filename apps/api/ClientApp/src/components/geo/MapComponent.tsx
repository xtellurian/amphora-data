import * as React from "react";
import * as atlas from "azure-maps-control";
import "azure-maps-control/dist/atlas.min.css";
import "./geo.css";
import { LoadingState } from "../molecules/empty/LoadingState";
import { useAmphoraClients } from "react-amphora";
import { Point, MapPoint } from "./model";
const defaultCenter = [144.9631, -37.8136];
const defaultZoom = 8;

interface MapComponentProps {
    center?: Point;
    points: MapPoint[];
}

function addStyleSelector(map: atlas.Map) {
    //Wait until the map resources are ready.
    map.events.add("ready", function () {
        /*Add the Style Control to the map*/
        map.controls.add(
            new atlas.control.StyleControl({
                mapStyles: [
                    "road",
                    "grayscale_dark",
                    "night",
                    "road_shaded_relief",
                ],
            }),
            {
                position: "top-right" as atlas.ControlPosition,
            }
        );
    });
}

function addPoint(map: atlas.Map, points: MapPoint[]) {
    map.events.add("ready", function () {
        //Create a data source and add it to the map.
        const dataSource = new atlas.source.DataSource();
        map.sources.add(dataSource);

        //Create a symbol layer to render icons and/or text at points on the map.
        const layer = new atlas.layer.SymbolLayer(dataSource);

        //Add the layer to the map.
        map.layers.add(layer);
        const features = points.map((p) => {
            return new atlas.data.Feature(
                new atlas.data.Point([p.lon, p.lat]),
                {
                    label: p.label,
                }
            );
        });
        dataSource.add(features);

        //Add a layer for rendering point data as symbols.
        map.layers.add(
            new atlas.layer.SymbolLayer(dataSource, "points", {
                iconOptions: {},
                textOptions: {
                    //Convert the temperature property of each feature into a string and concatenate "°F".
                    textField: ["concat", ["get", "label"]],

                    //Offset the text so that it appears on top of the icon.
                    offset: [0, -2],
                },
            })
        );
    });
}

interface MapComponentState {
    key?: string;
    error?: any;
    map?: atlas.Map;
}

const getCenter = (points: MapPoint[], center?: Point): number[] => {
    if (center) {
        return [center.lon, center.lat];
    } else if (points && points.length > 0) {
        return [points[0].lon, points[0].lat];
    } else {
        return defaultCenter;
    }
};

const getZoom = (): number => {
    return defaultZoom;
};

export const MapComponent: React.FC<MapComponentProps> = ({
    center,
    points,
}) => {
    const clients = useAmphoraClients();
    const [state, setState] = React.useState<MapComponentState>({});

    React.useEffect(() => {
        if (clients.isAuthenticated && !state.key) {
            clients.axios
                .get("/api/maps/key")
                .then((r) => {
                    setState({
                        key: r.data,
                    });
                })
                .catch((e) => {
                    setState({
                        error: e,
                    });
                });
        }
    });

    React.useEffect(() => {
        if (state.key && !state.map) {
            const map = new atlas.Map("myMap", {
                center: getCenter(points, center),
                zoom: getZoom(),
                language: "en-AU",
                authOptions: {
                    authType: "subscriptionKey" as atlas.AuthenticationType,
                    subscriptionKey: state.key,
                },
            });
            addPoint(map, points);
            addStyleSelector(map);
            setState({
                ...state,
                map,
            });
        }
    });

    if (state.key) {
        return (
            <div>
                <div className="map" id="myMap"></div>
                <small>Interactive Map</small>
            </div>
        );
    } else {
        return <LoadingState />;
    }
};
