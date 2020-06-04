import * as React from "react";
import { connect } from "react-redux";
import { AmphoraDetailProps, mapStateToProps } from "./props";
import ConnectedMapComponent from "../../geo/ConnectedMapComponent";
import { LoadingState } from "../../molecules/empty/LoadingState";

class Location extends React.PureComponent<AmphoraDetailProps> {
    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <div>
                        <div className="mr-1 row justify-content-between">
                            <div className="col-6 txt-lg align-text-bottom">
                                Location
                            </div>
                            <div className="col-3 bg-light">
                                {amphora.lat},{amphora.lon}
                            </div>
                        </div>
                        <hr />
                        <ConnectedMapComponent amphora={[amphora]} />
                    </div>
                </React.Fragment>
            );
        } else {
            return <LoadingState />;
        }
    }
}

export default connect(mapStateToProps, null)(Location);
