import * as React from 'react';
import { ToastContent } from './model';
import { ToastBase } from './ToastBase';

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

export const success = (content: ToastContent): JSX.Element => (<Success content={content}></Success>)

