import React from "react";
import "./harvey.css";

const ZERO = 0;
const ONE = 1;
const TWO = 2;
const THREE = 3;
const FOUR = 4;

export type HarveyBallLevel =
    | typeof ZERO
    | typeof ONE
    | typeof TWO
    | typeof THREE
    | typeof FOUR;

interface HarveyBallProps {
    level: HarveyBallLevel;
    className?: string;
    children?: React.ReactNode;
}

export const HarveyBall = (props: HarveyBallProps) => {
    const harveyClassname = `${props.className || ""} harvey`;
    switch (props.level) {
        case ZERO:
        default:
            return <div className={harveyClassname}></div>;
        case ONE:
            return (
                <div className={`${harveyClassname} quarters quarter`}></div>
            );
        case TWO:
            return <div className={`${harveyClassname} quarters half`}></div>;
        case THREE:
            return (
                <div
                    className={`${harveyClassname} quarters three-quarters`}
                ></div>
            );
        case FOUR:
            return <div className={`${harveyClassname} quarters full`}></div>;
    }
};
