import * as React from 'react';

import './empty.css';

export class EmptyState extends React.PureComponent {
    render() {
        return (<React.Fragment>
            <div className="emptystate text-center">
                <div id="emptystate-img" className="oval">
                    <img className="img-fluid m-2" alt="An empty state placeholder" src="/_content/sharedui/images/logos/amphora_white_transparent.svg" />
                </div>
                <div>
                    {this.props.children}
                </div>
            </div>
        </React.Fragment>)
    }
}