import { Invoice } from "amphoradata";

export const testInvoices = (n?: number) => {
    if (!n) {
        n = 3;
    }

    const res: Invoice[] = [];
    for (let x = 0; x < n; x++) {
        res.push({
            dateCreated: new Date(),
            id: `${x}-someid`,
            name: `The ${x}th invoice`,
            openingBalance: 0,
            invoiceBalance: 7,
            timestamp: new Date(),
            transactions: [
                {
                    amount: 7,
                    balance: 7,
                    label: "My Transaction",
                    amphoraId: "328957nw",
                    timestamp: new Date()
                }
            ]
        });
    }

    return res;
};
