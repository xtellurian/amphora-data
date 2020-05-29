import * as React from 'react';
import { connect } from 'react-redux';
import ReactMarkdown from 'react-markdown';
import { Spinner } from 'reactstrap';
import { AmphoraDetailProps, mapStateToProps } from './props';
import { LoadingState } from '../../molecules/empty/LoadingState';


class Description extends React.PureComponent<AmphoraDetailProps> {

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.cache[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <div>
                        Description:
                        <ReactMarkdown>
                            {amphora.description}
                        </ReactMarkdown>
                    </div>
                    <div>
                        Price: {amphora.price}
                    </div>
                </React.Fragment>

            )
        } else {
            return <LoadingState/>
        }

    }
}

export default connect(
    mapStateToProps,
    null,
)(Description);