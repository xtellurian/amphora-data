import React from "react";
import { useAmphoraClients } from "react-amphora";
import { CollectionResponseOfFeedItem } from "amphoradata";
import { LoadingState, EmptyState } from "../molecules/empty";
import { FeedList } from "./FeedList";
import { Link } from "react-router-dom";

interface FeedState {
    response?: CollectionResponseOfFeedItem;
    loading: boolean;
}
export const FeedComponent: React.FC = (props) => {
    const clients = useAmphoraClients();
    const [state, setState] = React.useState<FeedState>({
        loading: true,
    });

    React.useEffect(() => {
        if (clients.isAuthenticated) {
            clients.feedsApi.feedGetFeed().then((r) => {
                setState({
                    response: r.data,
                    loading: false,
                });
            });
        }
    }, [clients.isAuthenticated]);

    if (state.loading) {
        return <LoadingState>Loading Feed</LoadingState>;
    } else if (
        state.response &&
        state.response.items &&
        state.response.items.length > 0
    ) {
        return <FeedList items={state.response.items} />;
    } else {
        return <EmptyState>Your feed empty. Try <Link to="/create">creating an Amphora</Link>.</EmptyState>;
    }
};
