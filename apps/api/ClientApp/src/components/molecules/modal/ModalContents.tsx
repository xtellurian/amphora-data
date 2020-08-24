import * as React from "react";

export const ModalContents: React.FC = (props) => {
    return <div className="modal-inner overflow-auto">{props.children}</div>;
};

export const ModalFooter: React.FC = ({ children }) => {
    return <div className="modal-footer">{children}</div>;
};
