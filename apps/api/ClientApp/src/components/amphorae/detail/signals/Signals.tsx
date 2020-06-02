import * as React from "react";
import { Route } from "react-router";
import { connect } from "react-redux";
import { AmphoraDetailProps, mapStateToProps } from "../props";
import { LoadingState } from "../../../molecules/empty/LoadingState";
import { PrimaryButton } from "../../../molecules/buttons";

import { Link } from "react-router-dom";

class Signals extends React.PureComponent<AmphoraDetailProps> {
  public render() {
    const id = this.props.match.params.id;
    const amphora = this.props.amphora.cache[id];
    if (amphora) {
      return (
        <React.Fragment>
          <div className="text-right">
            <Link to={`/amphora/detail/${id}/signals/add`}>
              <PrimaryButton>Add Signal</PrimaryButton>
            </Link>
            <hr />
          </div>
        </React.Fragment>
      );
    } else {
      return <LoadingState />;
    }
  }
}

export default connect(mapStateToProps, null)(Signals);
