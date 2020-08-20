import * as React from "react";
import moment from "moment";

interface DateRendererProps {
    date: Date;
    format?: string;
}
export const DateRenderer: React.FC<DateRendererProps> = ({ date, format }) => {
    if (!format) {
        format = "MMM Do YY";
    }

    const mom = moment(date);

    return <span>{mom.format(format)}</span>;
};
