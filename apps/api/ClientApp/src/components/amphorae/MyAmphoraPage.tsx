import * as React from "react";
import { Route, useLocation } from "react-router-dom";
import { DetailedAmphora } from "amphoradata";
import { MyAmphoraContext } from "react-amphora";
import { AmphoraTable } from "../tables/AmphoraTable";
import { ConnectedAmphoraModal } from "./ConnectedAmphoraModal";
import { Tabs, activeTab } from "../molecules/tabs";
import { PaginationComponent } from "../molecules/pagination/Pagination";
import { Toggle } from "../molecules/toggles/Toggle";
import { LoadingState } from "../molecules/empty/LoadingState";

type Scope = "self" | "organisation";
type AccessType = "created" | "purchased";
const perPage = 64;
interface MyAmphoraPageState {
    page: number;
    loading: boolean;
    scope: Scope;
    accessType: AccessType;
    results: DetailedAmphora[];
}

const getAccessType = (queryString: string): AccessType => {
    const accessType = activeTab(queryString, "accessType") as AccessType;
    return accessType || "created";
};

const getPage = (queryString: string): number => {
    const p = new URLSearchParams(queryString);
    const parsed = parseInt(p.get("page") || "1");
    if (isNaN(parsed)) {
        return 1;
    } else {
        return parsed;
    }
};

const MyAmphoraPage: React.FC = () => {
    const context = MyAmphoraContext.useMyAmphora();
    const location = useLocation();

    const [state, setState] = React.useState<MyAmphoraPageState>({
        loading: true,
        accessType: getAccessType(location.search),
        scope: "self",
        results: context.results,
        page: getPage(location.search),
    });

    // react to changes in query string (page, access type)
    React.useEffect(() => {
        if (context.isAuthenticated) {
            const accessType = getAccessType(location.search);
            const page = getPage(location.search);
            if (state.accessType !== accessType || state.page !== page) {
                setState({
                    ...state,
                    page,
                    accessType,
                });
            }
        }
    }, [context.isAuthenticated, location.search]);

    // react to changes in authenticated, acces type, and scope
    React.useEffect(() => {
        if (context.isAuthenticated) {
            context.dispatch({
                type: "my-amphora:fetch-list",
                payload: {
                    accessType: state.accessType,
                    scope: state.scope,
                    skip: (state.page - 1) * perPage,
                    take: perPage,
                },
            });
        }
    }, [context.isAuthenticated, state.accessType, state.scope, state.page]);

    // react to changes in results
    React.useEffect(() => {
        if (context.isAuthenticated) {
            setState({
                ...state,
                loading: false,
                results: context.results,
            });
        }
    }, [context.isAuthenticated, context.results]);

    // react to loading in results
    React.useEffect(() => {
        if (context.isAuthenticated) {
            setState({
                ...state,
                results: context.results,
                loading:
                    context.isLoading === undefined ? false : context.isLoading,
            });
        }
    }, [context.isAuthenticated, context.isLoading]);

    const scopeOptionSelected = (v: string) => {
        setState({
            ...state,
            scope: v as Scope,
        });
    };

    const renderTabs = () => {
        const tabs = [{ id: "created" }, { id: "purchased" }];
        return <Tabs name="accessType" default="created" tabs={tabs} />;
    };

    const renderList = (
        loading: boolean,
        page: number,
        results: DetailedAmphora[]
    ) => {
        if (loading) {
            return <LoadingState />;
        }
        let nPages = page;
        if (state.results.length === perPage) {
            nPages = nPages + 1;
        }
        return (
            <div>
                {renderTabs()}
                <AmphoraTable amphoras={results} />
                <PaginationComponent
                    page={page}
                    nPages={nPages}
                    baseTo="/amphora"
                    className="mt-5"
                />
            </div>
        );
    };

    return (
        <React.Fragment>
            <div className="row">
                <div className="col-lg-5">
                    <div className="txt-xxl tour-my-amphora">My Amphora</div>
                </div>
                <div className="col-lg-7">
                    <Toggle
                        options={[
                            { text: "My List", id: "self" },
                            { text: "My Organisation", id: "organisation" },
                        ]}
                        onOptionSelected={(v) => scopeOptionSelected(v)}
                    />
                </div>
            </div>
            <hr />
            {renderList(state.loading, state.page, state.results)}
            <Route
                path="/amphora/detail/:id"
                component={ConnectedAmphoraModal}
            />
        </React.Fragment>
    );
};

export default MyAmphoraPage;
