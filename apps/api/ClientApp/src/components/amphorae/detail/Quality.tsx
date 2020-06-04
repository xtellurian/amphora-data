import * as React from "react";
import { connect } from "react-redux";
import { AmphoraDetailProps, mapStateToProps } from "./props";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Header } from "./Header";
import { EmptyState } from "../../molecules/empty/EmptyState";

class Quality extends React.PureComponent<AmphoraDetailProps> {
    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <Header title="Quality"></Header>

                    <EmptyState>
                        There are no quality metrics. for this Amphora.
                    </EmptyState>
                </React.Fragment>
            );
        } else {
            return <LoadingState />;
        }
    }
}

export default connect(mapStateToProps, null)(Quality);
