import * as React from "react";

export const ModalContents: React.FC = (props) => {
    return <div className="modal-inner">{props.children}</div>;
};
