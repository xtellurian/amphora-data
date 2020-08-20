import * as React from "react";
import { Invoice } from "amphoradata";
import { useAmphoraClients } from "react-amphora";
import { LoadingState, EmptyState, ErrorState } from "../../molecules/empty";
import { InvoicesList } from "./InvoicesList";
import { testInvoices } from "./test.data";
interface InvoicesSectionState {
    loading: boolean;
    invoices: Invoice[];
    error?: any;
}

const useTestData = false;

export const InvoicesSection: React.FC = (props) => {
    const clients = useAmphoraClients();
    const [state, setState] = React.useState<InvoicesSectionState>({
        loading: true,
        invoices: [],
    });

    React.useEffect(() => {
        if (clients.isAuthenticated) {
            setState({
                loading: true,
                invoices: state.invoices,
            });
            clients.accountApi
                .invoicesGetInvoices()
                .then((r) => {
                    setState({
                        loading: false,
                        invoices: useTestData
                            ? testInvoices(10)
                            : r.data.items || [],
                    });
                })
                .catch((e) => {
                    setState({
                        loading: false,
                        invoices: [],
                        error: e,
                    });
                });
        }
    }, [clients.isAuthenticated]);

    if (state.loading) {
        return <LoadingState />;
    } else if (state.error) {
        return <ErrorState>{`${state.error}`}</ErrorState>;
    } else if (state.invoices.length === 0) {
        return <EmptyState>There are no invoices to display</EmptyState>;
    } else {
        return (
            <React.Fragment>
                <InvoicesList invoices={state.invoices} />
            </React.Fragment>
        );
    }
};
