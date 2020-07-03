import * as React from "react";

import "./quote.css";
import { Quote } from "./quotes";

export const QuoteBlock: React.FunctionComponent<{ quote: Quote }> = (
    props
) => {
    return (
        <React.Fragment>
            <div className="testimonial-quote group">
                <img className="img-fluid float-left" src="/_content/sharedui/images/Amphora_Black.svg"></img>
                <div className="quote-container">
                    <blockquote>
                        <p>
                            {props.quote.text}
                        </p>
                    </blockquote>
                    <cite>
                        <span>{props.quote.by}</span>
                    </cite>
                </div>
            </div>
        </React.Fragment>
    );
};
