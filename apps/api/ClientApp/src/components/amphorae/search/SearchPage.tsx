import React from "react";
import { Route } from "react-router-dom";
import { PageContainer } from "../../layout/PageContainer";
import { SearchSection } from "./SearchSection";
import { ConnectedAmphoraModal } from "../ConnectedAmphoraModal";
const SearchPage: React.FC = () => {
    return (
        <PageContainer>
            <Route exact path="/search" component={SearchSection} />
            <Route
                path="/search/detail/:id"
                component={ConnectedAmphoraModal}
            />
        </PageContainer>
    );
};

export default SearchPage;
