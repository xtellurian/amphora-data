import * as React from "react";
import { connect } from "react-redux";
import { ApplicationState } from "../../../redux/state";
import { actionCreators } from "../../../redux/actions/plugins/burgerMenu";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Link } from "react-router-dom";

import "./detail.css";
import { IconProp } from "@fortawesome/fontawesome-svg-core";

type DetailMenuProps = { toggleMenu: (isOpen: boolean) => void } & {
  id: string;
} & { isOpen: boolean } & typeof actionCreators; // ... plus action creators we've requested

const iconBackground: React.CSSProperties = {
  textAlign: "end",
  backgroundColor: "var(--turquoise)",
};
const icon: React.CSSProperties = {
  cursor: "pointer",
};

const baseLink = "/amphora/detail";

class AmphoraDetailMenu extends React.PureComponent<DetailMenuProps> {
  private pages: {
    path: string;
    name: string;
    icon?: IconProp;
    iconText?: string;
  }[] = [
    { path: "", name: "Description", icon: "edit" },
    { path: "files", name: "Files", icon: "paperclip" },
    { path: "signals", name: "Signals", icon: "chart-line" },
    { path: "integrate", name: "Integrate", iconText: "</>" },
    { path: "terms", name: "Terms", icon: "file-signature" },
    { path: "location", name: "Location", icon: "map-marker-alt" },
    { path: "quality", name: "Quality", icon: "award" },
  ];

  private getLink(path: string) {
    return `${baseLink}/${this.props.id}/${path}`;
  }

  private onItemClick() {
    // this.props.close();
  }

  private renderLinks(): JSX.Element {
    return (
      <React.Fragment>
        {this.pages.map((p) => (
          <Link key={p.path}  to={this.getLink(p.path)}>
            <div className="menu-item txt-sm">
              <span className="menu-icon">
                {p.icon ? <FontAwesomeIcon icon={p.icon} /> : p.iconText}
              </span>
              {this.props.isOpen ? (
                <span className="menu-item-name">{p.name}</span>
              ) : null}
            </div>
          </Link>
        ))}
      </React.Fragment>
    );
  }

  private renderOpen(): JSX.Element {
    return (
      <div className="modal-menu open">
        <div style={iconBackground}>
          <FontAwesomeIcon
            style={icon}
            className="m-2"
            size="lg"
            onClick={() => this.props.toggleMenu(false)}
            icon="chevron-left"
          />
        </div>
        <div className="menu-items">{this.renderLinks()}</div>
      </div>
    );
  }

  private renderClosed(): JSX.Element {
    return (
      <div className="modal-menu closed">
        <div style={iconBackground}>
          <FontAwesomeIcon
            style={icon}
            className="m-2"
            size="lg"
            onClick={() => this.props.toggleMenu(true)}
            icon="chevron-right"
          />
        </div>
        {this.renderLinks()}
      </div>
    );
  }

  public render() {
    return this.props.isOpen ? this.renderOpen() : this.renderClosed();
  }
}

function mapStateToProps(state: ApplicationState) {
  return {};
}

export default connect(mapStateToProps, actionCreators)(AmphoraDetailMenu);
