import * as React from "react";
import "./info.css";

interface InfoPaneProps {
  title?: string;
}

export class InfoPane extends React.PureComponent<InfoPaneProps> {
  render() {
    return (
      <div className="infopane">
        <div className="title txt-med">{this.props.title || "Information"} </div>
        {this.props.children}
      </div>
    );
  }
}
