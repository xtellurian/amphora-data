import * as React from 'react';
import { connect } from 'react-redux';
import { Spinner } from 'reactstrap';
import { AmphoraDetailProps, mapStateToProps } from './props';


class Signals extends React.PureComponent<AmphoraDetailProps> {

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.cache[id];
        if (amphora) {
            return (

                <React.Fragment>
                    {id}
                    <h3>{amphora.name}</h3>
                    Signals
                </React.Fragment>

            )
        } else {
            return <Spinner></Spinner>
        }

    }
}

export default connect(
    mapStateToProps,
    null,
)(Signals);