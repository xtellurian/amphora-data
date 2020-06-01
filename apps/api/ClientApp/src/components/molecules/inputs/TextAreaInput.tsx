import * as React from "react";
import { TextInputBase } from "./TextInputBase";
import { InputProps } from "./inputProps";

interface TextAreaInputProps extends InputProps<string> {
  rows?: number;
  cols?: number;
}

export class TextAreaInput extends TextInputBase<TextAreaInputProps> {
  public render() {
    return (
      <div className="input-outer">
        <span>
          <strong>{this.props.label}</strong>
        </span>
        <div className="input-inner">
          <textarea
            className="form-control"
            rows={this.props.rows || 5}
            cols={this.props.cols || 20}
            name="description"
            onChange={(e) => this.handleChange(e)}
            onBlur={(e) => this.handleBlur(e)}
          >
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
