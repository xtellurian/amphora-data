import * as React from "react";
import { OneAmphora } from "./props";
import { MapComponent, getPoints } from "../../geo";
import { Header } from "./Header";

export const Location: React.FunctionComponent<OneAmphora> = (props) => {
    const renderNoLocationNotification = (): React.ReactNode | undefined => {
        if (props.amphora && !props.amphora.lat && !props.amphora.lon) {
            return (
                <div className="alert alert-warning w-75">
                    This Amphora has no defined location.
                </div>
            );
        }
    };
    const points = getPoints([props.amphora]);
    return (
        <React.Fragment>
            <Header title="Location">
                <span className="bg-light">
                    {props.amphora.lat},{props.amphora.lon}
                </span>
            </Header>
            {renderNoLocationNotification()}
            <MapComponent points={points} />
        </React.Fragment>
    );
};
