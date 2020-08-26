import * as React from "react";
import { ToastTrigger } from "./model";
import { ToastBase } from "./ToastBase";

import { toast } from "react-toastify";

export class Success extends ToastBase {
    render() {
        if (this.props.content) {
            return (
                <div className={`${this.outerDivClass}`}>
                    {this.renderIconColumn("check")}
                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div>
            );
        } else {
            return <React.Fragment></React.Fragment>;
        }
    }
}

export const success: ToastTrigger = (content, options?) => {
    if (!options) {
        options = {
            autoClose: 1000,
        };
    }
    if (typeof content === "string") {
        const newContent = { text: content };
        toast(<Success content={newContent}></Success>, {
            ...options,
            className: "toast-success",
        });
    } else {
        toast(<Success content={content}></Success>, {
            ...options,
            className: "toast-success",
        });
    }
};
