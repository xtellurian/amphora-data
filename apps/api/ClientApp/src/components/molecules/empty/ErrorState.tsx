import * as React from "react";

import "./empty.css";

export const ErrorState: React.FC = ({ children }) => {
    return (
        <div className="emptystate text-center">
            <div id="emptystate-img" className="oval">
                <img
                    className="img-fluid m-2"
                    alt="An error state placeholder"
                    src="/_content/sharedui/images/undraw/undraw_feeling_blue_4b7q.svg"
                />
            </div>
            <div>{children}</div>
        </div>
    );
};
