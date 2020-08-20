import * as React from "react";
import moment from "moment";

interface DateRendererProps {
    date?: Date | null | undefined;
    format?: string;
}
export const DateRenderer: React.FC<DateRendererProps> = ({ date, format }) => {
    if (!format) {
        format = "MMM Do YY";
    }

    if (!date) {
        return <span>Unknown Date</span>;
    }

    const mom = moment(date);

    return <span>{mom.format(format)}</span>;
};
