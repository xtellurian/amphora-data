import * as React from "react";
import { NewWelcome } from "../molecules/landing/NewWelcome";


export const MainPage: React.FunctionComponent = (props) => {
    return (
        <React.Fragment>
            <div>
                <NewWelcome isLoggedIn={false} />
            </div>
        </React.Fragment>
    )
}
