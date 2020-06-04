import * as React from "react";
import { connect } from "react-redux";
import { AmphoraDetailProps, mapStateToProps } from "./props";
import ConnectedMapComponent from "../../geo/ConnectedMapComponent";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Header } from "./Header";

class Location extends React.PureComponent<AmphoraDetailProps> {
    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <Header title="Location">
                        <span className="bg-light">
                            {amphora.lat},{amphora.lon}
                        </span>
                    </Header>

                    <ConnectedMapComponent amphora={[amphora]} />
                </React.Fragment>
            );
        } else {
            return <LoadingState />;
        }
    }
}

export default connect(mapStateToProps, null)(Location);
