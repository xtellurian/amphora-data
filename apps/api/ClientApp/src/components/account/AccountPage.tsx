import * as React from "react";
import { Header } from "../layout/AccountLayout";
import { PageContainer } from "../layout/PageContainer";
import { Tabs, activeTab } from "../molecules/tabs";

const tabs = [{ id: "transactions" }, { id: "invoices" }];

export const AccountPage: React.FC = () => {
    return (
        <PageContainer>
            <Header title="Account"></Header>

            <Tabs default={tabs[0].id}  tabs={tabs} />
        </PageContainer>
    );
};
