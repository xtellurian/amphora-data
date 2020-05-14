import * as React from 'react';
import Modal from 'react-modal';
import { connect } from 'react-redux';
import { ApplicationState } from '../../redux/state';
import { actionCreators } from '../../redux/actions/ui';
import { UiState } from '../../redux/state/ui';
import { AmphoraDetail } from './AmphoraDetail';

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

type ModalProps =
    UiState
    & typeof actionCreators;

class ConnectedAmphoraModal extends React.PureComponent<ModalProps> {

    public componentWillMount() {
        Modal.setAppElement('#root'); // TODO: figure out where to put this
    }

    private afterModalOpen() {

    }

    public render() {
        if (this.props.current) {
            return (
                <Modal isOpen={this.props.isAmphoraDetailOpen}
                    onAfterOpen={() => this.afterModalOpen()}
                    onRequestClose={() => this.setState({ isOpen: false })}
                    style={customStyles}
                    contentLabel="Example Modal"  >

                    <button className="float-right" onClick={() => this.props.close()}>close</button>
                    <AmphoraDetail amphora={this.props.current} />
                </Modal>
            )
        } else {
            return <div></div>
        }
    }
}

function mapStateToProps(state: ApplicationState): UiState {
    return state.ui || {
        isAmphoraDetailOpen: false
    };
}

export default connect(
    mapStateToProps,
    actionCreators
)(ConnectedAmphoraModal as any);