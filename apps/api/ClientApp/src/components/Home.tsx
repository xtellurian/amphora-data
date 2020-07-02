import * as React from "react";
import { connect } from "react-redux";
import { NewWelcome } from "./molecules/landing/NewWelcome";

export const Home = (): React.ReactElement => (
    <div>
        <NewWelcome isLoggedIn={true} />
    </div>
);
