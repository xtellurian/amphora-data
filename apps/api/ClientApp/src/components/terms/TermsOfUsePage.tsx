import * as React from "react";
import { Route } from "react-router";
import { Link } from "react-router-dom";
import { ConnectedTermsOfUseDetail } from "./TermsOfUseDetail";
import { CreateTermsOfUseComponent } from "./CreateTermsOfUseComponent";
import { Header } from "../layout/ManagementLayout";
import { TermsOfUseList } from "./TermsOfUseList";
import { PrimaryButton } from "../molecules/buttons";
import { PageContainer } from "../layout/PageContainer";


export const TermsOfUsePage: React.FunctionComponent = (props) => {
    return (
        <PageContainer>
            <Header
                title="Terms of Use"
                helpText="Some data are associated with specific terms of use. You can create your own custom terms, and attach them to Amphora you create."
            >
                <Link to="/terms/create">
                    <PrimaryButton>Create Custom Terms</PrimaryButton>
                </Link>
            </Header>
            <hr />
            <Route exact path="/terms" render={(props) => <TermsOfUseList />} />
            <Route path="/terms/create" component={CreateTermsOfUseComponent} />
            <Route
                path="/terms/detail/:id"
                component={ConnectedTermsOfUseDetail}
            />
        </PageContainer>
    );
};
