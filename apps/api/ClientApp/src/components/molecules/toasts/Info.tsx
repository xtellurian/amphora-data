import * as React from 'react';
import { ToastContent } from './model';
import { ToastBase } from './ToastBase';
import { toast } from 'react-toastify';

class Info extends ToastBase {
    render() {
        if (this.props.content) {
            return (
                <div className={`toast-info ${this.outerDivClass}`}>
                    {this.renderIconColumn("info-circle")}
                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div>);
        } else {
            return <React.Fragment></React.Fragment>
        }
    }
}


export function info(content: ToastContent, autoClose: number | undefined = 5000) {
    toast((<Info content={content}></Info>), {
        className: "toast-info",
        autoClose
    });
}
