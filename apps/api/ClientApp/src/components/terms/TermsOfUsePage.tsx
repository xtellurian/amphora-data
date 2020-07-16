import * as React from "react";
import { Route } from "react-router";
import { Link } from "react-router-dom";
import { ConnectedTermsOfUseDetail } from "./TermsOfUseDetail";
import { CreateTermsOfUseComponent } from "./CreateTermsOfUseComponent";
import { TermsOfUseList } from "./TermsOfUseList";
import { PrimaryButton } from "../molecules/buttons";
export const TermsOfUsePage: React.FunctionComponent = (props) => {
    return (
        <React.Fragment>
            <div className="row">
                <div className="col-lg-5">
                    <div className="txt-xxl">Terms of Use</div>
                </div>
                <div className="col-lg-7 text-right">
                    <Link to="/terms/create">
                        <PrimaryButton>Create Custom Terms</PrimaryButton>
                    </Link>
                </div>
            </div>
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
