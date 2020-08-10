import * as React from "react";
import { NewWelcome } from "./molecules/landing/NewWelcome";
import { DiagnosticPage } from "./hidden/DiagnosticPage";

export const Home = (): React.ReactElement => (
    <div>
        <DiagnosticPage />
        <NewWelcome isLoggedIn={true} />
    </div>
);
