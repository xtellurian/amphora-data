import * as React from "react";
import { OneAmphora } from "./props";
import ConnectedMapComponent from "../../geo/ConnectedMapComponent";
import { Header } from "./Header";

export const Location: React.FunctionComponent<OneAmphora> = (props) => {
    const renderNoLocationNotification = (): React.ReactNode | undefined => {
        if (props.amphora && !props.amphora.lat && !props.amphora.lon) {
            return <div className="alert alert-warning w-75">This Amphora has no defined location.</div>
        }
    };
    return (
        <React.Fragment>
            <Header title="Location">
                <span className="bg-light">
                    {props.amphora.lat},{props.amphora.lon}
                </span>
            </Header>
            {renderNoLocationNotification()}
            <ConnectedMapComponent amphora={[props.amphora]} />
        </React.Fragment>
    );
};
