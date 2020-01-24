function eventGridExportSchema() {
    return {
        items: {
            properties: {
                data: {
                    properties: {},
                    type: "object",
                },
                dataVersion: {
                    type: "string",
                },
                eventTime: {
                    type: "string",
                },
                eventType: {
                    type: "string",
                },
                id: {
                    type: "string",
                },
                metadataVersion: {
                    type: "string",
                },
                subject: {
                    type: "string",
                },
                topic: {
                    type: "string",
                },
            },
            required: [
                "id",
                "subject",
                "data",
                "eventType",
                "eventTime",
                "dataVersion",
                "metadataVersion",
                "topic",
            ],
            type: "object",
        },
        type: "array",
    };
}

export default function schemaAsJsonString() {
    return JSON.stringify(eventGridExportSchema());
}
