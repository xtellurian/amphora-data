import * as React from "react";
import { Invitation } from "amphoradata";
import { useAmphoraClients } from "react-amphora";
import { success, warning } from "../molecules/toasts";
import { ModalContents, ModalWrapper, ModalHeading } from "../molecules/modal";
import { TextInput } from "../molecules/inputs/TextInput";
import { ValidateResult } from "../molecules/inputs/inputProps";
import { ModalFooter } from "../molecules/modal/ModalContents";
import { PrimaryButton } from "../molecules/buttons";

const validateEmail = (value?: string): ValidateResult => {
    if (!value) {
        return {
            isValid: false,
            message: "An email address is required",
        };
    } else if (value.length < 3) {
        return {
            isValid: false,
            message: "The email is too short.",
        };
    } else if (!value.includes("@")) {
        return {
            isValid: false,
            message: `${value} is not a valid email address`,
        };
    } else {
        return {
            isValid: true,
        };
    }
};

export const InvitationModal: React.FC = (props) => {
    const [isOpen, setIsOpen] = React.useState(true);

    const [invitation, setInvitation] = React.useState<Invitation>({
        targetEmail: "",
    });
    const clients = useAmphoraClients();
    const setEmail = (email?: string) => {
        if (email) {
            setInvitation({ ...invitation, targetEmail: email });
        }
    };
    const submitInvitation = () => {
        clients.invitationsApi
            .invitationsInviteNewUser(invitation)
            .then((r) => {
                success(
                    { text: `${r.data.targetEmail} invited` },
                    { autoClose: 1500 }
                );
                setIsOpen(false);
            })
            .catch((e) => warning({ text: `${e}` }));
    };
    return (
        <ModalWrapper onCloseRedirectTo="/account/members" isOpen={isOpen}>
            <ModalContents>
                <ModalHeading>Invite a new team member</ModalHeading>
                <hr />
                <p>
                    If no user has this email address, then the email will be
                    invited to Amphora Data. Otherwise, an existing user will be
                    invited to join this organisation.
                </p>
                <TextInput
                    validator={(e) => validateEmail(e)}
                    onComplete={(e) => setEmail(e)}
                    label="User's email address"
                    placeholder="email"
                    type="email"
                />

                <ModalFooter>
                    <PrimaryButton onClick={() => submitInvitation()}>
                        Send Invitation
                    </PrimaryButton>
                </ModalFooter>
            </ModalContents>
        </ModalWrapper>
    );
};
