import { AmphoraState } from "../../../redux/state/amphora";
import { RouteComponentProps } from "react-router";
import { ApplicationState } from "../../../redux/state";
import { TermsOfUseState } from "../../../redux/state/terms";
import { DetailedAmphora, TermsOfUse } from "amphoradata";

export type AmphoraDetailProps = {
  amphora: AmphoraState;
  terms: TermsOfUseState;
  permissions: PermissionState;
} & RouteComponentProps<{ id: string }>;

export interface OneAmphora {
  amphora: DetailedAmphora;
  terms?: TermsOfUse | null | undefined;
  maxPermissionLevel?: number | null | undefined;
  isLoading?: boolean;
}