import React from "react";
import Floater from "react-floater";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface InfoIconProps {
    content: React.ReactNode;
}

export const InfoIcon: React.FC<InfoIconProps> = ({ content }) => {
    return (
        <Floater showCloseButton={true} content={content}>
            <span className="txt-xs ml-2">
                <FontAwesomeIcon className="m-auto" size="xs" icon="info-circle" color="var(--turquoise)" />
            </span>
        </Floater>
    );
};
