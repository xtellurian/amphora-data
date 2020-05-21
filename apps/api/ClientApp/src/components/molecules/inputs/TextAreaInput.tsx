import * as React from 'react';
import { TextInputBase } from './TextInputBase';


export class TextAreaInput extends TextInputBase {

    private characterCount(): React.ReactElement | undefined {
        return (
            <React.Fragment>
                {this.state.value.length} / 100
            </React.Fragment>
        )
    }

    public render() {
        return (
            <div className="input-outer">
                <span>
                    <strong>{this.props.label}</strong>
                </span>
                <div className="input-inner">
                    <textarea className="form-control" rows={5} cols={20} name="description"
                        onChange={(e) => this.handleChange(e)}
                        onBlur={(e) => this.handleBlur(e)}>
                        {this.props.children}
                    </textarea>
                </div>
                <span className="text-muted">
                    {this.validation() || this.helpText()}
                </span>
            </div>
        );
    }
}