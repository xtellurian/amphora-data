import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../redux/state';
import { actionCreators } from '../../redux/actions/ui';
import { ModalWrapper } from '../molecules/modal/ModalWrapper';
import { RouteComponentProps } from 'react-router';
import ReactMarkdown from 'react-markdown';
import { AmphoraState } from '../../redux/state/amphora';
import { Spinner } from 'reactstrap';

const customStyles = {
    // content: {
    //     top: '50%',
    //     left: '50%',
    //     right: 'auto',
    //     bottom: 'auto',
    //     marginRight: '-50%',
    //     transform: 'translate(-50%, -50%)'
    // }
};

type ConnectedAmphoraModalProps =
    AmphoraState
    & typeof actionCreators
    & RouteComponentProps<{ id: string }>;


class ConnectedAmphoraModal extends React.PureComponent<ConnectedAmphoraModalProps> {

    public componentDidMount() {
        // alert("connected amphora model mounted");
        // TODO: refresh cache here
    }

    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.cache[id]
        if (amphora) {
            return (
                <ModalWrapper isOpen={true} onCloseRedirectTo="/amphora" >
                    {id}
                    <h3>{amphora.name}</h3>
                    <div>
                        Description:
                        <ReactMarkdown>
                            {amphora.description}
                        </ReactMarkdown>
                    </div>
                    <div>
                        Price: {amphora.price}
                    </div>

                </ModalWrapper>
            )
        } else {
            return <Spinner></Spinner>
        }
        
    }
}

function mapStateToProps(state: ApplicationState) {
    return {
        isLoading: state.amphora.isLoading,
        list: state.amphora.list,
        cache: state.amphora.cache,
    }
}

export default connect(
    mapStateToProps,
    actionCreators
)(ConnectedAmphoraModal as any);