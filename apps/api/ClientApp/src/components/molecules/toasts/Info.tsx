import * as React from "react";
import { ToastTrigger } from "./model";
import { ToastBase } from "./ToastBase";
import { toast } from "react-toastify";

class Info extends ToastBase {
    render() {
        if (this.props.content) {
            return (
                <div className={`toast-info ${this.outerDivClass}`}>
                    {this.renderIconColumn("info-circle")}
                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div>
            );
        } else {
            return <React.Fragment></React.Fragment>;
        }
    }
}

export const info: ToastTrigger = (content, options) => {
    toast(<Info content={content}></Info>, {
        ...options,
        className: "toast-info",
    });
};
