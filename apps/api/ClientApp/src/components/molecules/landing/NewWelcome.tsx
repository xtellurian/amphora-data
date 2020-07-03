import * as React from "react";
import { PrimaryButton } from "../buttons";
import { Link } from "react-router-dom";
import { SignInButton } from "react-amphora";

interface NewWelcomeProps {
    isLoggedIn: boolean;
}
export const NewWelcome: React.FunctionComponent<NewWelcomeProps> = (props) => {
    return (
        <React.Fragment>
            <h1>Howdy!</h1>
            <p>
                We're working on this new user interface. To switch back to the
                old experience, <a href="https://app.amphoradata.com/challenge">click here</a>
            </p>

            <img
                alt="The Amphora Data logo"
                className="img-fluid"
                src="/_content/sharedui/images/logos/amphoradata_black_rect_1.png"
            />
            <hr />
            {props.isLoggedIn ? (
                <React.Fragment>
                    <p>
                        If you're just getting started, try seaching for weather
                        data in Brisbane (it's often nice there)
                    </p>
                    <Link to="/search?term=Brisbane Weather">
                        <PrimaryButton>
                            Search: 'Brisbane Weather'
                        </PrimaryButton>
                    </Link>{" "}
                </React.Fragment>
            ) : (
                <React.Fragment>
                    <SignInButton>Sign In</SignInButton>
                </React.Fragment>
            )}
        </React.Fragment>
    );
};
