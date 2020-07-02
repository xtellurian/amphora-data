import * as React from "react";
import { RouteComponentProps } from "react-router";
import { Route } from "react-router-dom";
import MyAmphoraTable from "../tables/MyAmphoraTable";
import { ConnectedAmphoraModal } from "./ConnectedAmphoraModal";
import { Tabs, activeTab } from "../molecules/tabs";
import { Toggle } from "../molecules/toggles/Toggle";
import { LoadingState } from "../molecules/empty/LoadingState";
import { MyAmphoraContext } from "react-amphora";

type Scope = 'self' | 'organisation'
type AccessType = 'created' | 'purchased'
// At runtime, Redux will merge together...
type MyAmphoraeProps = MyAmphoraContext.MyAmphoraState &
    MyAmphoraContext.FetchMyAmphoraDispatch &
    RouteComponentProps<{ accessType: string; scope: string }>; // ... plus incoming routing parameters

interface MyAmphoraeState {
    scope: Scope;
    retryCount: number;
}

class MyAmphorae extends React.Component<MyAmphoraeProps, MyAmphoraeState> {
    /**
     *
     */
    constructor(props: MyAmphoraeProps) {
        super(props);
        this.state = {
            scope: "self",
            retryCount: 0,
        };
    }
    componentDidMount() {
        this.loadList();
    }
    public componentDidUpdate(prevProps: MyAmphoraeProps) {
        if (this.state.retryCount <= 2 && this.props.error) {
            this.loadList();
            this.setState({
                scope: this.state.scope,
                retryCount: this.state.retryCount + 1,
            });
        }
        if (
            this.getAccessType(prevProps.location.search) !==
            this.getAccessType()
        ) {
            // check if the old access type is not the same as this one.
            this.loadList();
        }
    }

    private getAccessType(queryString?: string | undefined): AccessType {
        const accessType = activeTab(
            queryString || this.props.location.search,
            "accessType"
        ) as AccessType;
        return accessType || "created";
    }

    private scopeOptionSelected(v: string) {
        this.setState({ scope: v as Scope });
        this.loadList();
    }

    private loadList(): void {
        const scope = this.state.scope;
        const accessType = this.getAccessType();
        this.props.dispatch({ type: "fetch", payload: { accessType, scope } });
    }

    // rendering methods
    public render() {
        return (
            <React.Fragment>
                <div className="row">
                    <div className="col-lg-5">
                        <div className="txt-xxl">My Amphora</div>
                    </div>
                    <div className="col-lg-7">
                        <Toggle
                            options={[
                                { text: "My List", id: "self" },
                                { text: "My Organisation", id: "organisation" },
                            ]}
                            onOptionSelected={(v) =>
                                this.scopeOptionSelected(v)
                            }
                        />
                    </div>
                </div>
                <hr />
                {this.renderList()}

                <Route
                    path="/amphora/detail/:id"
                    component={ConnectedAmphoraModal}
                />
            </React.Fragment>
        );
    }

    private renderTabs() {
        const tabs = [{ id: "created" }, { id: "purchased" }];
        return (
            <React.Fragment>
                <Tabs name="accessType" default="created" tabs={tabs} />
            </React.Fragment>
        );
    }

    private renderList() {
        if (this.props.isLoading) {
            return <LoadingState />;
        }
        return (
            <div>
                {this.renderTabs()}
                <MyAmphoraTable
                    scope={this.props.scope || "self"}
                    accessType={this.getAccessType()}
                    {...this.props}
                />
            </div>
        );
    }
}

// function mapStateToProps(state: ApplicationState): AmphoraState {
//     if (state.amphora) {
//         return state.amphora;
//     } else {
//         return {
//             isLoading: true,
//             collections: {
//                 organisation: {
//                     created: [],
//                     purchased: [],
//                 },
//                 self: {
//                     created: [],
//                     purchased: [],
//                 },
//             },
//             metadata: emptyCache<DetailedAmphora>(),
//             signals: emptyCache<Signal[]>(),
//         };
//     }
// }

export default MyAmphoraContext.withMyAmphora(MyAmphorae);
