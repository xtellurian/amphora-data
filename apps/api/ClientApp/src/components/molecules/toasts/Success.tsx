import * as React from 'react';
import { ToastContent } from './model';
import { ToastBase } from './ToastBase';

import { toast } from 'react-toastify';

export class Success extends ToastBase {
    render() {
        if (this.props.content) {
            return (
                <div className={`${this.outerDivClass}`}>
                    {this.renderIconColumn("check")}
                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div>);
        } else {
            return <React.Fragment></React.Fragment>
        }
    }
}

export function success(content: ToastContent, autoClose: number | undefined = 5000) {
    toast((<Success content={content}></Success>), {
        className: "toast-success",
        autoClose
    });
}
