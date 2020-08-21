import { Transaction } from "amphoradata";


const sum = (x: number): number => {
    if (x === 0) {
        return 0;
    } else {
        return x + sum(x - 1);
    }
};


export const testTransactions = (n?: number) => {
    if (!n) {
        n = 3;
    }

    const res: Transaction[] = [];
    for (let x = 0; x < n; x++) {
        res.push({
            amount: x,
            amphoraId: `${x}-dkjnksx`,
            balance: sum(x),
            id: `${x}fbsfv`,
            label: `My ${x}th transaction`,
            timestamp: new Date(),
        });
    }

    return res;
};
