import * as React from "react";
import { TransactionsList } from "./TransactionsList";
import { testTransactions } from "./test.data";

it("renders without crashing when empty", () => {
    <TransactionsList transactions={[]} />;
});

it("renders with the test data", () => {
    <TransactionsList transactions={testTransactions(5)} />;
});
