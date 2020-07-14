import * as React from "react";
import { AmphoraOperationsContext } from "react-amphora";
import ReactMarkdown from "react-markdown";
import { OneAmphora } from "./props";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { MagicTextArea } from "../../molecules/magic-inputs/MagicTextArea";
import { Header } from "./Header";

export const Description: React.FunctionComponent<OneAmphora> = (props) => {
    const amphoraId = props.amphora.id;
    if (!amphoraId) {
        return <div>Unknown Amphora Id...</div>;
    }
    const disableEditing =
        !props.maxPermissionLevel || props.maxPermissionLevel < 128;
    console.log(props);
    const actions = AmphoraOperationsContext.useAmphoraOperationsDispatch();
    const onSave = (value: string) => {
        if (value !== props.amphora.description) {
            actions.dispatch({
                type: "amphora-operation-update",
                payload: {
                    id: amphoraId,
                    model: {
                        ...props.amphora,
                        description: value,
                    },
                },
            });
        }
    };
    if (props.isLoading) {
        return <LoadingState />;
    } else if (props.amphora) {
        return (
            <React.Fragment>
                <Header title="Description">
                    <div>Price: ${props.amphora.price}</div>
                </Header>
                <MagicTextArea
                    initialValue={props.amphora.description}
                    onSave={onSave}
                    disableEditing={disableEditing}
                >
                    <ReactMarkdown>
                        {props.amphora.description}
                    </ReactMarkdown>
                </MagicTextArea>
            </React.Fragment>
        );
    } else {
        return <LoadingState />;
    }
};
