import * as React from "react";
import { NewWelcome } from "./molecules/landing/NewWelcome";

export const Home = (): React.ReactElement => (
    <div>
        <NewWelcome isLoggedIn={true} />
    </div>
);
