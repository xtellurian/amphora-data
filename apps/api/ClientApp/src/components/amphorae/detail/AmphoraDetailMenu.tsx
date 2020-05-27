import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../../redux/state';
import { actionCreators } from '../../../redux/actions/plugins/burgerMenu';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Link } from 'react-router-dom';

type DetailMenuProps =
    { toggleMenu: (isOpen: boolean) => void }
    & { id: string }
    & { isOpen: boolean }
    & typeof actionCreators; // ... plus action creators we've requested

const iconBackground: React.CSSProperties = {
    textAlign: "end",
    backgroundColor: "var(--turquoise)"
}
const icon: React.CSSProperties = {
    cursor: "pointer",
}

const baseLink = "/amphora/detail";

class AmphoraDetailMenu extends React.PureComponent<DetailMenuProps> {

    private pages = [
        "Files",
        "Signals",
        "Integrate",
        "Terms",
        "Location",
        "Quality",
    ];

    private getLink(page: string) {
        return `${baseLink}/${this.props.id}/${page}`;
    }

    private onItemClick() {
        // this.props.close();
    }

    private renderLinks(): JSX.Element {
        return (<React.Fragment>
            {this.pages.map(p => <div key={p} className="menu-item"><Link to={this.getLink(p)}>{p}</Link></div>)}
        </React.Fragment>)
    }

    private renderOpen(): JSX.Element {
        return (
            <div className="modal-menu open">
                <div style={iconBackground}>
                    <FontAwesomeIcon style={icon} className="m-2" size="lg" onClick={() => this.props.toggleMenu(false)} icon="arrow-left" />
                </div>
                <div className="menu-items">
                    <div className="menu-item">
                        <Link to={this.getLink("")}>Description</Link>
                    </div>
                   {this.renderLinks()}
                </div>
            </div>
        );
    }

    private renderClosed(): JSX.Element {
        return (
            <div className="modal-menu closed">
                <div style={iconBackground}>
                    <FontAwesomeIcon style={icon} className="m-2" size="lg" onClick={() => this.props.toggleMenu(true)} icon="arrow-right" />
                </div>
            </div>
        );
    }


    public render() {
        return this.props.isOpen ? this.renderOpen() : this.renderClosed();
    }
}

function mapStateToProps(state: ApplicationState) {
    return {
    };
}

export default connect(
    mapStateToProps,
    actionCreators
)(AmphoraDetailMenu);