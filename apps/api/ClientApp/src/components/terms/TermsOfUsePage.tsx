import * as React from "react";
import { Route } from "react-router";
import { Link } from "react-router-dom";
import { ConnectedTermsOfUseDetail } from "./TermsOfUseDetail";
import { CreateTermsOfUseComponent } from "./CreateTermsOfUseComponent";
import { Header } from "../layout/ManagementLayout";
import { TermsOfUseList } from "./TermsOfUseList";
import { PrimaryButton } from "../molecules/buttons";
export const TermsOfUsePage: React.FunctionComponent = (props) => {
    return (
        <React.Fragment>
            <Header title="Terms of Use">
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
        </React.Fragment>
    );
};
