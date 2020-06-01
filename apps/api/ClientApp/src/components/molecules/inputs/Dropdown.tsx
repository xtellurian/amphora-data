import * as React from "react";
import "./inputs.css";

interface DropdownOption {
  value?: string;
  text?: string;
}
interface DropdownProps {
  label?: string;
  options: DropdownOption[];
  onChange: (value: string) => void;
}

export class Dropdown extends React.PureComponent<DropdownProps> {
  render() {
    return (
      <div className="input-outer">
        <span>
          <strong>{this.props.label}</strong>
        </span>

        <select
          className="dropdown"
          onChange={(e) => this.props.onChange(e.target.value)}
        >
          {this.props.options.map((o) => (
            <option key={o.value} value={o.value}>
              {o.text || o.value}
            </option>
          ))}
        </select>
      </div>
    );
  }
}
