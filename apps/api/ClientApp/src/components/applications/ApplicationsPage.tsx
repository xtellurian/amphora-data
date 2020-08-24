import React from "react";
import { Link, Route } from "react-router-dom";
import { Header } from "../layout/ManagementLayout";
import { PrimaryButton } from "../molecules/buttons";
import { PageContainer } from "../layout/PageContainer";
import { CreateOrUpdateApplicationModal } from "./CreateOrUpdateApplicationModal";
import { ApplicationsList } from "./ApplicationsList";
import { DeleteApplicationModal } from "./DeleteApplicationModal";

export const ApplicationsPage: React.FunctionComponent = () => {
    return (
        <PageContainer>
            <Header
                title="Applications"
                helpText="Applications help you connect external apps to Amphora."
            >
                <Link to="/applications/edit">
                    <PrimaryButton>New Application</PrimaryButton>
                </Link>
            </Header>
            <hr />
            <Route
                path="/applications/edit"
                component={CreateOrUpdateApplicationModal}
            />
            <Route exact path="/applications" component={ApplicationsList} />
            <Route
                path="/applications/delete"
                component={DeleteApplicationModal}
            />
        </PageContainer>
    );
};
