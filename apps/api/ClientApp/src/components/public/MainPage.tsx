import * as React from "react";
import { connect } from "react-redux";
import userManager from "../../userManager";
import { isLocalhost } from "../../utlities";
import { PrimaryButton } from "../molecules/buttons";

class MainPage extends React.PureComponent {
    public render() {
        if (isLocalhost) {
            return (
                <div>
                    <h3>Welcome to Amphora Data</h3>
                    <p>
                        We're working on a new user interface. To switch back
                        to the old experience,{" "}
                        <a href="/challenge">click here</a>
                    </p>
                    <PrimaryButton
                        onClick={() =>
                            userManager.signinRedirect({ data: { path: "/" } })
                        }
                    >
                        Login
                    </PrimaryButton>
                </div>
            );
        } else {
            //  window && (window.location.href = 'https://amphoradata.com');
            return <div> </div>;
        }
    }
}

function mapStateToProps() {
    return {};
}

export default connect(
    mapStateToProps, // Selects which state properties are merged into the component's props
    null // Selects which action creators are merged into the component's props
)(MainPage as any);
