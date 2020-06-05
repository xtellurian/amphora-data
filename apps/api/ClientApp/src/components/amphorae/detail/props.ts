import { AmphoraState } from "../../../redux/state/amphora";
import { RouteComponentProps } from "react-router";
import { ApplicationState } from "../../../redux/state";
import { TermsOfUseState } from "../../../redux/state/terms";

export type AmphoraDetailProps = {
  amphora: AmphoraState;
  terms: TermsOfUseState;
  permissions: PermissionState;
} & RouteComponentProps<{ id: string }>;

export function mapStateToProps(state: ApplicationState) {
  return {
    amphora: state.amphora,
    terms: state.terms,
    permissions: state.permissions,
  };
}
