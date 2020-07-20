import * as React from "react";
import Modal from "react-modal";
import { Redirect } from "react-router-dom";

import "./modal.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const customStyles: Modal.Styles = {
  overlay: {
    zIndex: 1500, // this should be on top of the hamburger menu.
  },
  content: {
    position: "absolute",
    top: "40px",
    left: "40px",
    right: "40px",
    bottom: "40px",
    border: "1px solid rgb(204, 204, 204)",
    background: "rgb(255, 255, 255)",
    overflow: "auto",
    borderRadius: "4px",
    outline: "var(--marble) solid 2px",
    padding: "0px",
  },
};

interface ModalWrapperProps extends Modal.Props {
  onCloseRedirectTo: string;
}

interface ModalState {
  willClose: boolean;
  isOpen: boolean;
}

const xStyle: React.CSSProperties = {
  cursor: "pointer",
  color: "var(--blue-garnet)",
};

export class ModalWrapper extends React.PureComponent<
  ModalWrapperProps,
  ModalState
> {
  constructor(props: ModalWrapperProps) {
    super(props);
    this.state = {
      isOpen: props.isOpen,
      willClose: false,
    };
  }

  private close() {
    this.setState({ willClose: true, isOpen: false });
  }

  public render() {
    if (this.state && this.state.willClose) {
      return <Redirect to={this.props.onCloseRedirectTo} />;
    }

    if (this.props && !this.props.isOpen) {
      return <Redirect to={this.props.onCloseRedirectTo} />;
    }

    return (
      <Modal
        isOpen={this.state.isOpen}
        shouldCloseOnEsc={true}
        shouldCloseOnOverlayClick={true}
        onAfterOpen={this.props.onAfterOpen}
        onRequestClose={(e) => this.close()}
        style={customStyles}
        contentLabel="Example Modal"
      >
        <FontAwesomeIcon
          style={xStyle}
          className="m-3 float-right"
          size="2x"
          icon="times"
          onClick={() => this.close()}
        />
        {this.props.children}
      </Modal>
    );
  }
}
