import * as React from "react";
import { Invoice } from "amphoradata";
import { Row, Col } from "reactstrap";
import { useAmphoraClients } from "react-amphora";
import fileDownload from "js-file-download";
import { DateRenderer } from "../../molecules/dates/DateRenderer";
import { parseServerError } from "../../../utilities/errors";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { warning, error, info } from "../../molecules/toasts";
interface InvoicesListProps {
    invoices: Invoice[];
}

interface InvoiceRowProps {
    invoice: Invoice;
    downloadInvoice: (invoice: Invoice) => void;
}
const InvoiceHeading: React.FC = () => {
    return (
        <div className="m-3">
            <Row>
                <Col lg={2} className="d-none d-lg-block">
                    <div className="text-center align-bottom txt-md txt-bold">
                        Date
                    </div>
                </Col>
                <Col>
                    <div className="text-center align-bottom txt-md txt-bold">
                        Name
                    </div>
                </Col>
                <Col className="d-none d-lg-block">
                    <div className="text-center align-bottom txt-md txt-bold">
                        Opening Balance
                    </div>
                </Col>
                <Col>
                    <div className="text-center align-bottom txt-md txt-bold">
                        Invoice Balance
                    </div>
                </Col>
            </Row>
            <hr />
        </div>
    );
};
const InvoiceRow: React.FC<InvoiceRowProps> = ({
    invoice,
    downloadInvoice,
}) => {
    return (
        <Row>
            <Col lg={2}>
                <div className="text-center d-none d-lg-block">
                    <DateRenderer date={invoice.timestamp} />
                </div>
            </Col>
            <Col>
                <div className="text-center">{invoice.name}</div>
            </Col>
            <Col className="d-none d-lg-block">
                <div className="text-center">{invoice.openingBalance}</div>
            </Col>
            <Col>
                <div className="text-center">{invoice.invoiceBalance}</div>
            </Col>
            <Col
                xs={1}
                onClick={() => downloadInvoice(invoice)}
                className="cursor-pointer"
            >
                <FontAwesomeIcon icon="download" />
            </Col>
        </Row>
    );
};
export const InvoicesList: React.FC<InvoicesListProps> = ({ invoices }) => {
    const clients = useAmphoraClients();
    const downloadInvoice = (invoice: Invoice) => {
        if (invoice.id) {
            clients.accountApi
                .invoicesDownloadInvoice(invoice.id, "csv", {
                    responseType: "blob",
                })
                .then((r) => {
                    info("Downloading invoice...");
                    fileDownload(r.data as any, `${invoice.name}.csv`);
                })
                .catch((e) => {
                    error(parseServerError(e, "Failed to download invoice"));
                });
        } else {
            warning("Unknown Invoice ID");
        }
    };
    return (
        <React.Fragment>
            <div>
                <InvoiceHeading />
                {invoices.map((i, j) => (
                    <InvoiceRow
                        key={j}
                        invoice={i}
                        downloadInvoice={downloadInvoice}
                    />
                ))}
            </div>
        </React.Fragment>
    );
};
