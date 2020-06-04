import * as React from "react";
import { connect } from "react-redux";
import ReactMarkdown from "react-markdown";
import { AmphoraDetailProps, mapStateToProps } from "./props";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Header } from "./Header";

class Description extends React.PureComponent<AmphoraDetailProps> {
    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <Header title="Description">
                        <div>Price: ${amphora.price}</div>
                    </Header>

                    <ReactMarkdown>{amphora.description}</ReactMarkdown>
                </React.Fragment>
            );
        } else {
            return <LoadingState />;
        }
    }
}

export default connect(mapStateToProps, null)(Description);
