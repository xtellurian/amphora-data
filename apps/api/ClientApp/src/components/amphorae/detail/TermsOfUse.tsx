import * as React from "react";
import { connect } from "react-redux";
import { useAmphoraClients } from "react-amphora";
import { AmphoraDetailProps, mapStateToProps, OneAmphora } from "./props";
import ReactMarkdown from "react-markdown";
import { actionCreators as listActions } from "../../../redux/actions/terms/list";
import { DetailedAmphora } from "amphoradata";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { EmptyState } from "../../molecules/empty/EmptyState";
import { Header } from "./Header";

// type TermsOfUseProps = AmphoraDetailProps & typeof listActions;

export const TermsOfUsePage: React.FunctionComponent<OneAmphora> = (props) => {
    if (props.isLoading) {
        return (
            <React.Fragment>
                <LoadingState />
            </React.Fragment>
        );
    } else if (props.terms) {
        return (
            <React.Fragment>
                <div className="txt-lg">
                    Terms of Use (other use cant see terms yet)
                </div>
                <div className="txt-med">{props.terms.name}</div>
                <hr />
                <ReactMarkdown>{props.terms.contents}</ReactMarkdown>
            </React.Fragment>
        );
    } else {
        return (
            <React.Fragment>
                <EmptyState>There are no terms</EmptyState>
            </React.Fragment>
        );
    }
};

// class TermsOfUseClassy extends React.PureComponent<TermsOfUseProps> {
//     private renderTerms(amphora: DetailedAmphora) {
//         const termsId = amphora.termsOfUseId;
//         if (
//             this.props.terms.cache.lastUpdated === undefined &&
//             !this.props.terms.isLoading
//         ) {
//             this.props.listTerms();
//             return (
//                 <React.Fragment>
//                     <LoadingState />
//                 </React.Fragment>
//             );
//         }
//         const terms = termsId ? this.props.terms.cache.store[termsId] : null;
//         if (terms) {
//             return (
//                 // TODO: get working for other user
//                 <React.Fragment>
//                     <div className="txt-lg">
//                         Terms of Use (other use cant see terms yet)
//                     </div>
//                     <div className="txt-med">{terms.name}</div>
//                     <hr />
//                     <ReactMarkdown>{terms.contents}</ReactMarkdown>
//                 </React.Fragment>
//             );
//         } else {
//             return (
//                 <React.Fragment>
//                     <EmptyState>There are no terms</EmptyState>
//                 </React.Fragment>
//             );
//         }
//     }

//     public render() {
//         const id = this.props.match.params.id;
//         const amphora = this.props.amphora.metadata.store[id];
//         if (amphora) {
//             return (
//                 <React.Fragment>
//                     <Header title="Terms of Use" />
//                     {this.renderTerms(amphora)}
//                 </React.Fragment>
//             );
//         } else {
//             return <LoadingState />;
//         }
//     }
// }

// export default connect(mapStateToProps, { ...listActions })(
//     TermsOfUseClassy as any
// );
