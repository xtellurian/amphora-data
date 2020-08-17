import * as React from "react";
import { Spinner } from "reactstrap";
import userManager from "../../userManager";

export class Challenge extends React.PureComponent {
    componentDidMount() {
        userManager.signinRedirect({
            data: { path: "/" },
        });
    }
    render() {
        return (
            <div className="text-center">
                <Spinner />
                <p>Redirecting you to login...</p>
            </div>
        );
    }
}
