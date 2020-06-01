import * as React from "react";
import { connect } from "react-redux";
import { AmphoraDetailProps, mapStateToProps } from "./props";
import ReactMarkdown from "react-markdown";
import { actionCreators as listActions } from "../../../redux/actions/terms/list";
import { DetailedAmphora } from "amphoradata";
import { LoadingState } from "../../molecules/empty/LoadingState";

type TermsOfUseProps = AmphoraDetailProps & typeof listActions;

class TermsOfUse extends React.PureComponent<TermsOfUseProps> {
  private renderTerms(amphora: DetailedAmphora) {
    const termsId = amphora.termsOfUseId;
    if (!this.props.terms.lastLoaded) {
      this.props.listTerms();
      return (
        <React.Fragment>
          <LoadingState />
        </React.Fragment>
      );
    }
    const terms = termsId ? this.props.terms.cache[termsId] : null;
    if (terms) {
      return (
        <React.Fragment>
          <div className="txt-lg">Terms of Use</div>
          <div className="txt-med">{terms.name}</div>
          <hr/>
          <ReactMarkdown>{terms.contents}</ReactMarkdown>
        </React.Fragment>
      );
    } else {
      return <React.Fragment>There are no terms</React.Fragment>;
    }
  }

  public render() {
    const id = this.props.match.params.id;
    const amphora = this.props.amphora.cache[id];
    if (amphora) {
      return <React.Fragment>{this.renderTerms(amphora)}</React.Fragment>;
    } else {
      return <LoadingState />;
    }
  }
}

export default connect(mapStateToProps, { ...listActions })(TermsOfUse);
