import * as React from "react";
import { Transaction } from "amphoradata";
import { Row, Col } from "reactstrap";
import { DateRenderer } from "../../molecules/dates/DateRenderer";
interface TransactionsListProps {
    transactions: Transaction[];
}

interface TransactionRowProps {
    transaction: Transaction;
}
const TransactionHeader: React.FC = () => {
    return (
        <Row>
            <Col md={2}>
                <div className="text-center align-bottom txt-md txt-bold">
                    Date
                </div>
            </Col>
            <Col md={5}>
                <div className="text-center align-bottom txt-md txt-bold">
                    Label
                </div>
            </Col>
            <Col>
                <div className="text-left txt-md txt-bold">Amount</div>
            </Col>
            <Col>
                <div className="text-left txt-md txt-bold">Balance</div>
            </Col>
        </Row>
    );
};
const TransactionRow: React.FC<TransactionRowProps> = ({ transaction }) => {
    return (
        <Row>
            <Col md={2}>
                <div className="text-center">
                    <DateRenderer date={transaction.timestamp || new Date()} />
                </div>
            </Col>
            <Col md={5}>
                <div className="text-center">{transaction.label}</div>
            </Col>
            <Col>
                <div className="text-left">{transaction.amount}</div>
            </Col>
            <Col>
                <div className="text-left">{transaction.balance}</div>
            </Col>
        </Row>
    );
};
export const TransactionsList: React.FC<TransactionsListProps> = ({
    transactions,
}) => {
    return (
        <div className="m-4">
            <TransactionHeader />
            <hr />
            {transactions.map((t, i) => (
                <TransactionRow key={i} transaction={t} />
            ))}
        </div>
    );
};
