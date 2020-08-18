import * as React from "react";
import "./tabs.css";

export const TabContainer: React.FC = ({ children }) => {
    return <div className="tabs row no-gutters text-sm">{children}</div>;
};

interface TabProps {
    isActive: () => boolean;
}
export const Tab: React.FC<TabProps> = ({ children, isActive }) => {
    const commonClassNames = "tab col-3";
    const className = isActive()
        ? `${commonClassNames} active txt-bold`
        : commonClassNames;

    return <div className={className}>{children}</div>;
};
