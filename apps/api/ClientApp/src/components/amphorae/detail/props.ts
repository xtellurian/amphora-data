import { RouteComponentProps } from "react-router";
import { DetailedAmphora, TermsOfUse } from "amphoradata";

export type AmphoraDetailProps = {
    permissions: PermissionState;
} & RouteComponentProps<{ id: string }>;

export interface OneAmphora {
    amphora: DetailedAmphora;
    terms?: TermsOfUse | null | undefined;
    maxPermissionLevel?: number | null | undefined;
    isLoading?: boolean;
}
