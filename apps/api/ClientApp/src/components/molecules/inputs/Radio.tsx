import * as React from "react";
import "./inputs.css";

interface RadioOption {
  value?: string;
  text?: string;
}
interface RadioProps {
  defaultValue?: string;
  name: string;
  label?: string;
  options: RadioOption[];
  onChange: (value: string) => void;
}
interface RadioState {
  value?: string;
}

export class Radio extends React.PureComponent<RadioProps, RadioState> {
  /**
   *
   */
  constructor(props: RadioProps) {
    super(props);
    this.state = {
      value: props.defaultValue,
    };
  }
  private onRadioChanged(e: React.ChangeEvent<HTMLInputElement>) {
    if (this.props.onChange) {
      this.props.onChange(e.target.value);
    }
    this.setState({
      value: e.target.value,
    });
  }

  renderOption(o: RadioOption): JSX.Element {
    return (
      <React.Fragment key={o.value}>
        <span className="radio-option">
          <input
            onChange={(e) => this.onRadioChanged(e)}
            checked={o.value === this.state.value}
            key={o.value}
            type="radio"
            name={this.props.name}
            value={o.value}
          ></input>
          <label>{o.text}</label>
        </span>
      </React.Fragment>
    );
  }

  render() {
    return (
      <div className="input-outer">
        <div>
          <strong>{this.props.label}</strong>
        </div>
        {this.props.options.map((o) => this.renderOption(o))}
      </div>
    );
  }
}
