import * as React from 'react';
import { ToastContent } from './model';
import { ToastBase } from './ToastBase';
import { toast } from 'react-toastify';

class Warning extends ToastBase {
    render() {
        if (this.props.content) {
            return (
                <div className={`${this.outerDivClass}`} >
                    {this.renderIconColumn("exclamation-triangle")}
                    {this.renderContent()}
                    {this.renderActionsColumn()}
                </div >);
        } else {
            return <React.Fragment></React.Fragment>
        }
    }
}

export function warning(content: ToastContent, autoClose: number | undefined = 5000) {
    toast((<Warning content={content}></Warning>), {
        className: "toast-warning",
        autoClose
    });
}
