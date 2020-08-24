export const parseServerError = (e: any, fallback?: string): string => {
    if (typeof e === "undefined") {
        return "An unknown error occurred";
    } else if (e.response) {
        // this is an axios response
        if (e.response.data && e.response.data.message) {
            return e.response.data.message;
        } else {
            return fallback || "The server responded with an error";
        }
    } else {
        // not an axios error
        return fallback || "The did not respond as expected";
    }
};
