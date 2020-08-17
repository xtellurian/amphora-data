import * as React from "react";
import { Spinner } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUser } from "@fortawesome/free-solid-svg-icons";

import { IdentityContext } from "react-amphora";

interface AvatarProps {
    className?: string;
}

export const Avatar: React.FC<AvatarProps> = (props) => {
    const idState = IdentityContext.useIdentityState();

    if (idState.user && idState.user.profile && idState.user.profile.name) {
        return (
            <div
                className={`${props.className} ml-2 d-flex align-items-center`}
            >
                <FontAwesomeIcon
                    className="align-middle mr-2"
                    size="sm"
                    icon={faUser}
                />
                {`Hi ${idState.user.profile.name}`}
            </div>
        );
    } else if (idState.user) {
        return (
            <div
                className={`${props.className} ml-2 d-flex align-items-center`}
            >
                <FontAwesomeIcon
                    className="align-middle"
                    size="lg"
                    icon={faUser}
                />
            </div>
        );
    } else {
        return <Spinner className={props.className} color="light" />;
    }
};
