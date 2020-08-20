import * as React from "react";
import { Transaction } from "amphoradata";
import { useAmphoraClients } from "react-amphora";
import { EmptyState, LoadingState } from "../../molecules/empty";
import { TransactionsList } from "./TransactionsList";

import { testTransactions } from "./test.data";

interface TransactionSectionState {
    loading: boolean;
    error?: any;
    transactions: Transaction[];
}

const useTestData = false;

export const TransactionSection: React.FC = (props) => {
    const [state, setState] = React.useState<TransactionSectionState>({
        loading: true,
        transactions: [],
    });

    const clients = useAmphoraClients();

    React.useEffect(() => {
        if (clients.isAuthenticated) {
            setState({
                ...state,
                loading: true,
            });
            clients.accountApi
                .transactionsGetTransactions()
                .then((r) => {
                    setState({
                        loading: false,
                        transactions: useTestData
                            ? testTransactions(20)
                            : r.data.items || [],
                    });
                })
                .catch((e) => {
                    setState({
                        transactions: state.transactions,
                        loading: false,
                        error: e,
                    });
                });
        }
    }, [clients.isAuthenticated]);

    if (state.loading) {
        return <LoadingState />;
    } else if (state.transactions.length == 0) {
        return (
            <EmptyState>There are no recent transactions to display</EmptyState>
        );
    } else {
        return (
            <React.Fragment>
                <TransactionsList transactions={state.transactions} />
            </React.Fragment>
        );
    }
};
