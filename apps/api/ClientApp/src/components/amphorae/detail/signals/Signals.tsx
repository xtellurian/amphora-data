import * as React from "react";
import { Link } from "react-router-dom";
import { connect } from "react-redux";
import { AmphoraDetailProps, mapStateToProps } from "../props";
import { actionCreators } from "../../../../redux/actions/signals/fetch";
import { LoadingState } from "../../../molecules/empty/LoadingState";
import { PrimaryButton } from "../../../molecules/buttons";
import { TsiComponent } from "./TsiComponent";
import { Signal } from "amphoradata";

type SignalsProps = AmphoraDetailProps & typeof actionCreators;
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
    console.log(signals);
    if (amphora) {
      return (
        <React.Fragment>
          <div className="text-right">
            <Link to={`/amphora/detail/${id}/signals/add`}>
              <PrimaryButton className="mr-2">Add Signal</PrimaryButton>
            </Link>
            <hr />
          </div>
          <TsiComponent amphoraId={id} signals={signals} />
        </React.Fragment>
      );
    } else {
      return <LoadingState />;
    }
  }
}

export default connect(mapStateToProps, actionCreators)(Signals);
