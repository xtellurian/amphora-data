import * as React from "react";
import { AppLocation } from "amphoradata";
import { Row, Col } from "reactstrap";
import { TextInput } from "../molecules/inputs";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { ValidateResult } from "../molecules/inputs/inputProps";
interface EditApplicationLocationSectionProps {
    location: AppLocation;
    onUpdated: (location: AppLocation) => void;
    onRemoved: (location: AppLocation) => void;
}
export const EditApplicationLocationSection: React.FC<EditApplicationLocationSectionProps> = ({
    location,
    onUpdated,
    onRemoved,
}) => {
    const updateOrigin = (origin?: string) => {
        location.origin = origin;
        onUpdated(location);
    };
    const updateAllowedRedirectPath = (allowedRedirectPath?: string) => {
        if (allowedRedirectPath) {
            location.allowedRedirectPaths = [allowedRedirectPath];
            onUpdated(location);
        }
    };
    const updatePostLogoutRedirect = (postLogoutRedirect?: string) => {
        if (postLogoutRedirect) {
            location.postLogoutRedirects = [postLogoutRedirect];
            onUpdated(location);
        }
    };

    const existingRedirectPath =
        location.allowedRedirectPaths &&
        location.allowedRedirectPaths.length > 0
            ? location.allowedRedirectPaths[0]
            : undefined;
    const existingPostLogoutRedirect =
        location.postLogoutRedirects && location.postLogoutRedirects.length > 0
            ? location.postLogoutRedirects[0]
            : undefined;

    const isUrlValidator = (
        fieldName: string,
        value?: string | undefined
    ): ValidateResult => {
        if (!value) {
            return {
                isValid: false,
                message: `${fieldName} is required`,
            };
        } else if (!value.startsWith("http")) {
            return {
                isValid: false,
                message: `${fieldName} should start with http`,
            };
        } else {
            return {
                isValid: true,
            };
        }
    };
    return (
        <React.Fragment>
            <div className="card p-3">
                <Row>
                    <Col>
                        <TextInput
                            label="Origin"
                            value={location.origin || undefined}
                            onComplete={(o) => updateOrigin(o)}
                            validator={(v) => isUrlValidator("Origin", v)}
                        />
                        <TextInput
                            label="Allowed Redirect Path"
                            value={existingRedirectPath}
                            onComplete={(o) => updateAllowedRedirectPath(o)}
                        />
                        <TextInput
                            label="Post Logout Redirect"
                            value={existingPostLogoutRedirect}
                            validator={(v) => isUrlValidator("Post Logout Redirect", v)}
                            onComplete={(o) => updatePostLogoutRedirect(o)}
                        />
                    </Col>
                    <Col xs={1}>
                        <div className="float-right">
                            <FontAwesomeIcon
                                className="m-3 float-right cursor-pointer"
                                size="2x"
                                icon="times"
                                onClick={() => onRemoved(location)}
                            />
                        </div>
                    </Col>
                </Row>
            </div>
        </React.Fragment>
    );
};
