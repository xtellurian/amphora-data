import * as React from "react";
import { useSelector, useDispatch } from "react-redux";
import { Row, Col } from "reactstrap";
import { Header } from "../layout/SettingsLayout";
import { PageContainer } from "../layout/PageContainer";
import { Toggle, ToggleOption } from "../molecules/toggles/Toggle";
import { ApplicationState } from "../../redux/state";
import { Settings } from "../../redux/state/Settings";
import { actionCreators } from "../../redux/actions/settings";
const debuggingNotificationOptions: ToggleOption[] = [
    {
        id: "enable",
        text: "Enable",
    },
    {
        id: "disable",
        text: "Disable",
    },
];
export const SettingsPage: React.FC = () => {
    const settings = useSelector<ApplicationState, Settings>((s) => s.settings);
    const dispatch = useDispatch();
    const debuggingNotificationSelected = (o: string) => {
        console.log(o);
        const newSettings = { ...settings };
        switch (o) {
            case "enable":
                newSettings.showDebuggingNotifications = true;
                break;
            case "disable":
                newSettings.showDebuggingNotifications = false;
                break;
        }

        dispatch(actionCreators.update(newSettings));
    };

    return (
        <PageContainer>
            <Header title="Settings" />
            <hr />
            <Row>
                <Col>Enable Debbuging Notifications</Col>
                <Col>
                    <Toggle
                        options={debuggingNotificationOptions}
                        onOptionSelected={debuggingNotificationSelected}
                        default={
                            settings.showDebuggingNotifications
                                ? "enable"
                                : "disable"
                        }
                    />
                </Col>
            </Row>
        </PageContainer>
    );
};
