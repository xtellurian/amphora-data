export interface Quote {
    text: string;
    by: string;
}

const quotes: Quote[] = [
    {
        text:
            "Without big data, you are blind and deaf and in the middle of a freeway.",
        by: "Geoffrey Moore",
    },
    { text: "In God we trust, all others bring data.", by: "W Edwards Deming" },
    { text: "Data is the new oil.", by: "Clive Humby" },
    {
        text:
            "No great marketing decisions have ever been made on qualitative data.",
        by: "John Sculley",
    },
    {
        text: "Torture the data, and it will confess to anything.",
        by: "Ronald Coase",
    },
    {
        text: "Big data isn’t about bits, it’s about talent.",
        by: "Douglas Merrill",
    },
    {
        text: "It is a capital mistake to theorize before one has data.",
        by: "Sherlock Holmes",
    },
    {
        text:
            "Without a systematic way to start and keep data clean, bad data will happen.",
        by: "Donato Diorio",
    },
    {
        text:
            "You can have data without information, but you cannot have information without data",
        by: "Daniel Keys Moran",
    },
    {
        text:
            "If we have data, let’s look at data. If all we have are opinions, let’s go with mine.",
        by: "Jim Barksdale",
    },
    { text: "Above all else, show the data.", by: "Edward R. Tufte" },
    {
        text:
            "Big data is at the foundation of all of the megatrends that are happening today, from social to mobile to the cloud to gaming.",
        by: "Chris Lynch",
    },
    {
        text:
            "Data are just summaries of thousands of stories – tell a few of those stories to help make the data meaningful.",
        by: "Chip & Dan Heath",
    },
    { text: "Data beats emotions.", by: "Sean Rad" },
    {
        text:
            "Contact data ages like fish not wine…it gets worse as it gets older, not better.",
        by: "Gregg Thaler",
    },
    { text: "Data really powers everything that we do.", by: "Jeff Weiner" },
    { text: "Data that is loved tends to survive.", by: "Kurt Bollacker" },
    {
        text:
            "Errors using inadequate data are much less than those using no data at all.",
        by: "Charles Babbage",
    },
    {
        text: "Where there is data smoke, there is business fire.",
        by: "Thomas Redman",
    },
];

export const randomQuote: () => Quote = () =>
    quotes[Math.floor(Math.random() * quotes.length)];
