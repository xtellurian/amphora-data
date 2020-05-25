import { AmphoraState } from "../../../redux/state/amphora";
import { RouteComponentProps } from "react-router";
import { ApplicationState } from "../../../redux/state";

export type AmphoraDetailProps =
    AmphoraState
    & RouteComponentProps<{ id: string }>;

export function mapStateToProps(state: ApplicationState) {
    return {
        isLoading: state.amphora.isLoading,
        list: state.amphora.list,
        cache: state.amphora.cache,
    }
}