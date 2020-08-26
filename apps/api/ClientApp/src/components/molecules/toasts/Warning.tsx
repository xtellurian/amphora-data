import * as React from "react";
import { ToastTrigger } from "./model";
import { ToastBase } from "./ToastBase";
import { toast } from "react-toastify";

class Warning extends ToastBase {
    render() {
        if (this.props.content) {
            return (
                <div className={`${this.outerDivClass}`}>
                    {this.renderIconColumn("exclamation-triangle")}
                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div>
            );
        } else {
            return <React.Fragment></React.Fragment>;
        }
    }
}

export const warning: ToastTrigger = (content, options) => {
    if (!options) {
        options = {
            autoClose: 1000,
        };
    }
    if (typeof content === "string") {
        const newContent = {
            text: content,
        };
        toast(<Warning content={newContent}></Warning>, {
            ...options,
            className: "toast-warning",
        });
    } else {
        toast(<Warning content={content}></Warning>, {
            ...options,
            className: "toast-warning",
        });
    }
};
