import * as React from "react";
import { FeedComponent } from "../../feeds/FeedComponent";
import { PrimaryButton } from "../buttons";
import { Link } from "react-router-dom";
import { SignInButton } from "react-amphora";

interface NewWelcomeProps {
    isLoggedIn: boolean;
}
export const NewWelcome: React.FunctionComponent<NewWelcomeProps> = (props) => {
    if (!props.isLoggedIn) {
        return <SignInButton>Sign In</SignInButton>;
    } else
        return (
            <React.Fragment>
                <h1>Feed</h1>
                <p>
                    We're working on this new user interface. To switch back to
                    the old experience,{" "}
                    <a href="https://app.amphoradata.com/challenge">
                        click here
                    </a>
                </p>
                <hr/>
                <FeedComponent />

            </React.Fragment>
        );
};
