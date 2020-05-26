import * as React from 'react';
import { ToastContent } from './model';
import { ToastBase } from './ToastBase';

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

export const info = (content: ToastContent): JSX.Element => (<Info content={content}></Info>)