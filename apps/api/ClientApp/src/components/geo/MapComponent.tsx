import * as React from "react";
import * as atlas from "azure-maps-control";
import "azure-maps-control/dist/atlas.min.css";
import "./geo.css";
import { LoadingState } from "../molecules/empty/LoadingState";

const defaultCenter = [144.9631, -37.8136];
const defaultZoom = 8;

interface Point {
    lat: number;
    lon: number;
}

interface MapComponentProps {
    subscriptionKey: string;
    center?: Point;
    points: MapPoint[];
}

export interface MapPoint extends Point {
    label: string;
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

export class MapComponent extends React.PureComponent<MapComponentProps> {
    componentDidMount() {
        const map = new atlas.Map("myMap", {
            center: this.getCenter(),
            zoom: this.getZoom(),
            language: "en-AU",
            authOptions: {
                authType: "subscriptionKey" as atlas.AuthenticationType,
                subscriptionKey: this.props.subscriptionKey,
            },
        });
        addPoint(map, this.props.points);
        addStyleSelector(map);
    }

    private getCenter(): number[] {
        if (this.props.center) {
            return [this.props.center.lon, this.props.center.lat];
        } else if (this.props.points && this.props.points.length > 0) {
            return [this.props.points[0].lon, this.props.points[0].lat];
        } else {
            return defaultCenter;
        }
    }
    private getZoom(): number {
        return defaultZoom;
    }
    render() {
        return (
            <div>
                {this.props.subscriptionKey ? null : <LoadingState />}
                <div className="map" id="myMap"></div>
                <small>Interactive Map</small>
            </div>
        );
    }
}
