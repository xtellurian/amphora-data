import * as React from 'react';
import { connect } from 'react-redux';

class MainPage extends React.PureComponent {


    public render() {
        return (
            <div>
                THIS IS THE PUBLIC PAGE
            </div>
        );
    }
}

function mapStateToProps() {
    return {}
}

export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    null // Selects which action creators are merged into the component's props
)(MainPage as any);
