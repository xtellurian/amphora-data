import React, { useState } from "react";
import { Dropdown, DropdownMenu, DropdownToggle } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export const OverflowMenuButton: React.FC = ({ children }) => {
    const [dropdownOpen, setDropdownOpen] = useState(false);

    const toggle = () => setDropdownOpen((prevState) => !prevState);

    return (
        <Dropdown className="w-100" isOpen={dropdownOpen} toggle={toggle}>
            <DropdownToggle
                className="cursor-pointer w-100"
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
