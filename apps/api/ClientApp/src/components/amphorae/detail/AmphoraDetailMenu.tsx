import * as React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Link } from "react-router-dom";
import { baseLink } from "../ConnectedAmphoraModal";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { useLocation } from "react-router";
import "./detail.css";

interface DetailMenuProps {
    id: string;
    isOpen: boolean;
    maxPermissionLevel: number;
    toggleMenu: (isOpen: boolean) => void;
}

const iconBackground: React.CSSProperties = {
    textAlign: "end",
    backgroundColor: "var(--turquoise)",
};
const icon: React.CSSProperties = {
    cursor: "pointer",
};
const pages: {
    path: string;
    name: string;
    icon: IconProp;
}[] = [
    { path: "", name: "Description", icon: "edit" },
    { path: "files", name: "Files", icon: "paperclip" },
    { path: "signals", name: "Signals", icon: "chart-line" },
    { path: "integrate", name: "Integrate", icon: "code" },
    { path: "terms", name: "Terms", icon: "file-signature" },
    { path: "location", name: "Location", icon: "map-marker-alt" },
    { path: "quality", name: "Quality", icon: "award" },
];

export const AmphoraDetailMenu: React.FC<DetailMenuProps> = ({
    id,
    maxPermissionLevel,
    isOpen,
    toggleMenu,
}) => {
    const location = useLocation();

    const isEnabled = (path: string, maxPermissionLevel: number) => {
        if (path === "files" || path === "signals") {
            return maxPermissionLevel >= 32; // read contents
        } else {
            return true;
        }
    };

    const getLink = (path: string) => {
        return `${baseLink(location.pathname)}/${id}/${path}`;
    };

    const isPageActive = (path: string): boolean => {
        const actual = location.pathname;
        const link = getLink(path);
        return link === actual;
    };

    const renderLinks = () => {
        return (
            <React.Fragment>
                {pages
                    .filter((p) => isEnabled(p.path, maxPermissionLevel))
                    .map((p) => (
                        <Link key={p.path} to={getLink(p.path)}>
                            <div
                                className={`menu-item txt-sm ${
                                    isPageActive(p.path) ? "active" : ""
                                }`}
                            >
                                <span className="menu-icon">
                                    <FontAwesomeIcon icon={p.icon} />
                                </span>
                                {isOpen ? (
                                    <span className="menu-item-name">
                                        {p.name}
                                    </span>
                                ) : null}
                            </div>
                        </Link>
                    ))}
            </React.Fragment>
        );
    };

    const renderOpen = () => {
        return (
            <div className="modal-menu open">
                <div style={iconBackground}>
                    <FontAwesomeIcon
                        style={icon}
                        className="m-2"
                        size="lg"
                        onClick={() => toggleMenu(false)}
                        icon="chevron-left"
                    />
                </div>
                <div className="menu-items">{renderLinks()}</div>
            </div>
        );
    };

    const renderClosed = () => {
        return (
            <div className="modal-menu closed">
                <div style={iconBackground}>
                    <FontAwesomeIcon
                        style={icon}
                        className="m-2"
                        size="lg"
                        onClick={() => toggleMenu(true)}
                        icon="chevron-right"
                    />
                </div>
                {renderLinks()}
            </div>
        );
    };
    return isOpen ? renderOpen() : renderClosed();
};
