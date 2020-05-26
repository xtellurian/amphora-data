import * as React from 'react';
import { ToastContent } from './model';
import { ToastBase } from './ToastBase';
import { toast } from 'react-toastify';

class Error extends ToastBase {
    render() {
        if (this.props.content) {
            return (
                <React.Fragment>
                    <div className={`${this.outerDivClass}`}>
                        {this.renderIconColumn("exclamation-circle")}
                        <div className="col">
                            {this.props.content.text}
                        </div>
                    </div>
                </React.Fragment>
                );

        } else {
            return <React.Fragment></React.Fragment>
        }
    }
}

export function error(content: ToastContent, autoClose: number | undefined = 5000) {
    toast((<Error content={content}></Error>), {
        className: "toast-error",
        autoClose
    });
}
