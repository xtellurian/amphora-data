import * as React from "react";

import "./info.css";

interface QualityPaneProps {
    title: string;
    level?: number | null | undefined;
    children?: React.ReactNode;
}

export const HarveyPane = (props: QualityPaneProps) => {
    return (
        <div className="quality-pane row justify-content-between">
            <div className="col-4">
                <h5>{props.title}</h5>
            </div>
            <div className="col-6 m-2">{props.children}</div>
        </div>
    );
};
