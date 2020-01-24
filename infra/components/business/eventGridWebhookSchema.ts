function eventGridExportSchema() {
    return {
        data: {
            userId: "",
        },
        dataVersion: "2",
        eventTime: "2017-06-26T18:41:00.9584103Z",
        eventType: "AmphoraData.Users.SignIn",
        id: "831e1650-001e-001b-66ab-eeb76e069631",
        metadataVersion: "1",
        subject: "Name of subject",
        topic: "the name of the topic",
    };
}

export default function schemaAsJsonString() {
    return JSON.stringify(eventGridExportSchema());
}
