import * as React from "react";
import { useAmphoraClients } from "react-amphora";
import { CreateApplication, AppLocation } from "amphoradata";
import { ModalWrapper, ModalContents, ModalHeading } from "../molecules/modal";
import { TextInput } from "../molecules/inputs";
import { EditApplicationLocationSection } from "./ApplicationLocationSection";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export const CreateApplicationModal: React.FC = () => {
    const clients = useAmphoraClients();

    const [state, setState] = React.useState<CreateApplication>({
        locations: [{}],
    });

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
    const addNewLocation = () => {
        if (state && state.locations) {
            setState({
                ...state,
                locations: [...state.locations, {}],
            });
        }
    };

    const updateLocation = (location: AppLocation, index: number) => {
        if (state.locations) {
            state.locations[index] = location;
        }
    };

    return (
        <React.Fragment>
            <ModalWrapper onCloseRedirectTo="/applications" isOpen={true}>
                <ModalContents>
                    <ModalHeading>Create a new Application</ModalHeading>
                    <TextInput label="Application Name" onComplete={setName} />
                    <TextInput label="Logout URL" onComplete={setLogoutUrl} />

                    <hr />
                    <h3>Locations</h3>
                    {state.locations &&
                        state.locations.map((l, index) => (
                            <EditApplicationLocationSection
                                key={index}
                                location={l}
                                onUpdated={(k) => updateLocation(k, index)}
                            />
                        ))}
                    <div onClick={() => addNewLocation()}>
                        <FontAwesomeIcon icon="plus-square" />
                        Add Location
                    </div>
                </ModalContents>
            </ModalWrapper>
        </React.Fragment>
    );
};
