import * as React from "react";
import { ToastTrigger } from "./model";
import { ToastBase } from "./ToastBase";
import { toast } from "react-toastify";

class Error extends ToastBase {
    render() {
        if (this.props.content) {
            return (
                <React.Fragment>
                    <div className={`${this.outerDivClass}`}>
                        {this.renderIconColumn("exclamation-circle")}
                        <div className="col">{this.props.content.text}</div>
                    </div>
                </React.Fragment>
            );
        } else {
            return <React.Fragment></React.Fragment>;
        }
    }
}

export const error: ToastTrigger = (content, options) => {
    toast(<Error content={content}></Error>, {
        ...options,
        className: "toast-error",
    });
};
