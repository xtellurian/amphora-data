import * as React from "react";
import { Header, AccountNav } from "../layout/AccountLayout";
import { PageContainer } from "../layout/PageContainer";
import { useLocation, useHistory, Route } from "react-router";
import { MembersSection } from "./MembersSection";

export const AccountPage: React.FC = () => {
    const location = useLocation();
    const history = useHistory();

    if (location.pathname === "/account" || location.pathname === "/account/") {
        history.push("/account/members");
    }

    return (
        <PageContainer>
            <Header title="Account"></Header>
            <AccountNav />
            <Route path="/account/members" component={MembersSection} />
        </PageContainer>
    );
};
