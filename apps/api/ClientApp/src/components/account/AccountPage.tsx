import * as React from "react";
import { Header, AccountNav } from "../layout/AccountLayout";
import { PageContainer } from "../layout/PageContainer";
import { useLocation, useHistory, Route } from "react-router";
import { MembersSection } from "./members/MembersSection";
import { InvitationModal } from "./members/InvitationModal";
import { TransactionSection } from "./transactions/TransactionSection";
import { InvoicesSection } from "./invoices/InvoicesSection";
import { PlanSection } from "./plan/PlanSection";
import { SelectPlanModal } from "./plan/SelectPlanModal";

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
            <Route path="/account/new-invitation" component={InvitationModal} />
            <Route
                path="/account/transactions"
                component={TransactionSection}
            />
            <Route path="/account/invoices" component={InvoicesSection} />
            <Route path="/account/plan" component={PlanSection} />
            <Route path="/account/select-plan" component={SelectPlanModal} />
        </PageContainer>
    );
};
