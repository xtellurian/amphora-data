import * as React from 'react';
import Modal from 'react-modal';
import { Redirect } from 'react-router-dom';

const customStyles: Modal.Styles = {

    overlay: {
        zIndex: 1500, // this should be on top of the hamburger menu.
    }
    // content: {
    //     top: '50%',
    //     left: '50%',
    //     right: 'auto',
    //     bottom: 'auto',
    //     marginRight: '-50%',
    //     transform: 'translate(-50%, -50%)'
    // }
};

interface ModalWrapperProps extends Modal.Props {
    onCloseRedirectTo: string;
}

interface ModalState {
    willClose: boolean;
}

export class ModalWrapper extends React.PureComponent<ModalWrapperProps, ModalState> {

    public componentWillMount() {
        Modal.setAppElement('#root'); // TODO: figure out where to put this
    }

    private close() {
        this.setState({ willClose: true });
    }

    public render() {
        if (this.state && this.state.willClose) {
            return (
                <Redirect to={this.props.onCloseRedirectTo} />
            )
        }

        return (
            <Modal isOpen={this.props.isOpen}
                onAfterOpen={this.props.onAfterOpen}
                onRequestClose={this.props.onRequestClose}
                style={customStyles}
                contentLabel="Example Modal"  >

                <button className="float-right" onClick={() => this.close()}> X </button>
                {this.props.children}
            </Modal>
        )
    }
}
