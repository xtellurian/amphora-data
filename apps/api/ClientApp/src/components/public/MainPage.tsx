import * as React from 'react';
import { connect } from 'react-redux';

import { isLocalhost } from '../../utlities';

class MainPage extends React.PureComponent {


    public render() {
        if (isLocalhost) {
            return (<div>
                Placeholder for a redirect
            </div>);
        } else {
           //  window && (window.location.href = 'https://amphoradata.com');
            return (
                <div> </div>
            );
        }
    }
}

function mapStateToProps() {
    return {}
}

export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    null // Selects which action creators are merged into the component's props
)(MainPage as any);
