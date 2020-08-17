import * as React from "react";
import { useAmphoraClients } from "react-amphora";
import { CreateApplication } from "amphoradata";
import { ModalWrapper, ModalContents } from "../molecules/modal";
import { TextInput } from "../molecules/inputs";

export const CreateApplicationModal: React.FC = () => {
    const clients = useAmphoraClients();

    const [state, setState] = React.useState<CreateApplication>({});

    const setName = (name?: string) => {
        if (name) {
            setState({ ...state, name });
        }
    };

    const setLogoutUrl = (logoutUrl?: string) => {
        if (logoutUrl) {
            setState({ ...state, logoutUrl });
        }
    };

    return (
        <React.Fragment>
            <ModalWrapper onCloseRedirectTo="/applications" isOpen={true}>
                <ModalContents>
                    <h3>Create a new Application</h3>
                    <TextInput label="Application Name" onComplete={setName} />
                    <TextInput label="Logout URL" onComplete={setLogoutUrl} />
                </ModalContents>
            </ModalWrapper>
        </React.Fragment>
    );
};
