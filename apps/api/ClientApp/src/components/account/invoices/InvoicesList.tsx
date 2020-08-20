import * as React from "react";
import { Invoice } from "amphoradata";
import { Row, Col } from "reactstrap";
import { DateRenderer } from "../../molecules/dates/DateRenderer";
interface InvoicesListProps {
    invoices: Invoice[];
}

interface InvoiceRowProps {
    invoice: Invoice;
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
const InvoiceRow: React.FC<InvoiceRowProps> = ({ invoice }) => {
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
        </Row>
    );
};
export const InvoicesList: React.FC<InvoicesListProps> = ({ invoices }) => {
    return (
        <React.Fragment>
            <div>
                <InvoiceHeading />
                {invoices.map((i, j) => (
                    <InvoiceRow key={j} invoice={i} />
                ))}
            </div>
        </React.Fragment>
    );
};
