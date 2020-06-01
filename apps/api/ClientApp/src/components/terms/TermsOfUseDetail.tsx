import * as React from "react";
import { connect } from "react-redux";
import { RouteComponentProps } from "react-router";
import { ApplicationState } from "../../redux/state";
import { TermsOfUseState } from "../../redux/state/terms";
import { LoadingState } from "../molecules/empty/LoadingState";
import { emptyCache } from "../../redux/state/common";
import { TermsOfUse } from "amphoradata";
import ReactMarkdown from "react-markdown";
import { ModalWrapper } from "../molecules/modal/ModalWrapper";

// At runtime, Redux will merge together...
type TermsOfUseDetailProps = TermsOfUseState & {
  onCloseRedirectTo?: string;
} & RouteComponentProps<{ id: string }>; // ... plus incoming routing parameters

class TermsOfUseDetailComponent extends React.Component<TermsOfUseDetailProps> {
  // rendering methods
  public render() {
    const terms = this.props.cache[this.props.match.params.id];
    if (!terms) {
      return <LoadingState />;
    } else {
      return (
        <React.Fragment>
          <ModalWrapper
            isOpen={true}
            onCloseRedirectTo={this.props.onCloseRedirectTo || "/terms"}
          >
            <div className="m-3">
              <div className="txt-xxl">Terms of Use</div>
              <div className="txt-lg">{terms.name}</div>
              <hr />

              <div className="p-3">
                <ReactMarkdown>{terms.contents}</ReactMarkdown>
              </div>
            </div>
          </ModalWrapper>
        </React.Fragment>
      );
    }
  }
}

function mapStateToProps(state: ApplicationState): TermsOfUseState {
  if (state.terms) {
    return state.terms;
  } else {
    return {
      cache: emptyCache<TermsOfUse>(),
      isLoading: true,
      termIds: [],
    };
  }
}

export default connect(
  mapStateToProps, // Selects which state properties are merged into the component's props
  null // Selects which action creators are merged into the component's props
)(TermsOfUseDetailComponent);
