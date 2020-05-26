import * as React from 'react';
import { ToastContent } from './model';
import { ToastBase } from './ToastBase';

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

export const warning = (content: ToastContent): JSX.Element => (<Warning content={content}></Warning>)