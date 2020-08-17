import * as React from "react";
import { useAmphoraClients } from "react-amphora";
import { Header } from "../layout/ManagementLayout";
export const ApplicationsPage: React.FunctionComponent = () => {
    const clients = useAmphoraClients();

    return (
        <React.Fragment>
            <Header title="Applications"></Header>
        </React.Fragment>
    );
};
