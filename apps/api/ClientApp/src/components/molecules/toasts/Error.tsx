import * as React from 'react';
import { ToastContent } from './model';
import { ToastBase } from './ToastBase';

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

export const error = (content: ToastContent): JSX.Element => (<Error content={content}></Error>)