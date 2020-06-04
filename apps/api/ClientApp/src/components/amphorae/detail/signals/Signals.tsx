import * as React from "react";
import { Link } from "react-router-dom";
import { connect } from "react-redux";
import { AmphoraDetailProps, mapStateToProps } from "../props";
import { actionCreators } from "../../../../redux/actions/signals/fetch";
import { LoadingState } from "../../../molecules/empty/LoadingState";
import { PrimaryButton } from "../../../molecules/buttons";
import { TsiComponent } from "./TsiComponent";
import { Signal } from "amphoradata";
import { ApplicationState } from "../../../../redux/state";
import { EmptyState } from "../../../molecules/empty/EmptyState";
import { Header } from "../Header";

type SignalsProps = AmphoraDetailProps &
    typeof actionCreators & { token: string };
class Signals extends React.PureComponent<SignalsProps> {
    private getSignals(id: string): Signal[] {
        const signals = this.props.amphora.signals.store[id];
        if (!signals) {
            this.props.fetchSignals(id);
            return [];
        } else {
            return [...signals];
        }
    }
    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        const signals = this.getSignals(id);
        if (amphora) {
            return (
                <React.Fragment>
                    <Header title="Signals">
                        <Link to={`/amphora/detail/${id}/signals/add`}>
                            <PrimaryButton>
                                Add Signal
                            </PrimaryButton>
                        </Link>
                    </Header>

                    {signals && signals.length > 0 ? (
                        <TsiComponent
                            token={this.props.token}
                            amphoraId={id}
                            signals={signals}
                        />
                    ) : (
                        <EmptyState>There are no signals.</EmptyState>
                    )}
                </React.Fragment>
            );
        } else {
            return <LoadingState />;
        }
    }
}

function mapActualStateToProps(state: ApplicationState) {
    const common = mapStateToProps(state);
    return {
        ...common,
        token: state.oidc.user ? state.oidc.user.access_token : "",
    };
}
export default connect(mapActualStateToProps, actionCreators)(Signals);
