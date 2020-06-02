import * as React from 'react';
import { connect } from 'react-redux';
import { AmphoraDetailProps, mapStateToProps } from './props';
import { LoadingState } from '../../molecules/empty/LoadingState';


class Integrate extends React.PureComponent<AmphoraDetailProps> {

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        if (amphora) {
            return (

                <React.Fragment>
                    {id}
                    <h3>{amphora.name}</h3>
                    Integrete
                </React.Fragment>

            )
        } else {
            return <LoadingState />
        }

    }
}

export default connect(
    mapStateToProps,
    null,
)(Integrate);