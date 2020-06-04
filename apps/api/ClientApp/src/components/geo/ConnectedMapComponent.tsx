import * as React from "react";
import { connect } from "react-redux";
import { actionCreators } from "../../redux/actions/maps/auth";
import { ApplicationState } from "../../redux/state";
import { MapComponent, MapPoint } from "./MapComponent";
import { LoadingState } from "../molecules/empty/LoadingState";
import { DetailedAmphora } from "amphoradata";

interface ConnectedMapComponentProps {
    subscriptionKey: string;
    amphora: DetailedAmphora[];
}

class ConnectedMapComponent extends React.PureComponent<
    ConnectedMapComponentProps & typeof actionCreators
> {
    componentDidMount() {
        if (!this.props.subscriptionKey) {
            this.props.fetchToken();
        }
    }

    private getPoints(): MapPoint[] {
        return this.props.amphora
            .filter((a) => a.lat && a.lon)
            .map((a) => {
                return {
                    label: a.name,
                    lat: a.lat as number,
                    lon: a.lon as number,
                };
            });
    }
    render() {
        if (this.props.subscriptionKey) {
            return (
                <MapComponent
                    points={this.getPoints()}
                    subscriptionKey={this.props.subscriptionKey}
                />
            );
        } else {
            return <LoadingState />;
        }
    }
}

function mapStateToProps(state: ApplicationState) {
    if (state && state.maps) {
        return {
            subscriptionKey: state.maps.subscriptionKey || "",
        };
    } else {
        return {
            subscriptionKey: "",
        };
    }
}

export default connect(mapStateToProps, { ...actionCreators })(
    ConnectedMapComponent
);
