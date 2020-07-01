import * as React from "react";
import { connect } from "react-redux";
import { SignInButton } from "react-amphora";

class MainPage extends React.PureComponent {
    public render() {
        return (
            <div>
                <h3>Welcome to Amphora Data</h3>
                <p>
                    We're working on a new user interface. To switch back to the
                    old experience, <a href="/challenge">click here</a>
                </p>
                <div className="w-25">
                    <SignInButton>Sign In</SignInButton>
                </div>
            </div>
        );
    }
}

function mapStateToProps() {
    return {};
}

export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    null // Selects which action creators are merged into the component's props
)(MainPage as any);
