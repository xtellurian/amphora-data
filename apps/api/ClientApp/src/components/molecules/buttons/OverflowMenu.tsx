import React, { useState } from "react";
import { Dropdown, DropdownMenu, DropdownToggle } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export const OverflowMenuButton: React.FC = ({ children }) => {
    const [dropdownOpen, setDropdownOpen] = useState(false);

    const toggle = () => setDropdownOpen((prevState) => !prevState);

    return (
        <Dropdown isOpen={dropdownOpen} toggle={toggle}>
            <DropdownToggle
                tag="span"
                data-toggle="dropdown"
                aria-expanded={dropdownOpen}
            >
                <FontAwesomeIcon icon="ellipsis-v" />
            </DropdownToggle>
            <DropdownMenu positionFixed={true}>{children}</DropdownMenu>
        </Dropdown>
    );
};
