import * as React from "react";
import { AppLocation } from "amphoradata";
import { TextInput } from "../molecules/inputs";
interface EditApplicationLocationSectionProps {
    location: AppLocation;
    onUpdated: (location: AppLocation) => void;
}
export const EditApplicationLocationSection: React.FC<EditApplicationLocationSectionProps> = ({
    location,
    onUpdated,
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
    return (
        <React.Fragment>
            <div className="card p-4">
                <TextInput label="Origin" onComplete={(o) => updateOrigin(o)} />
                <TextInput
                    label="Allowed Redirect Path"
                    onComplete={(o) => updateAllowedRedirectPath(o)}
                />
                <TextInput
                    label="Post Logout Redirect"
                    onComplete={(o) => updatePostLogoutRedirect(o)}
                />
            </div>
        </React.Fragment>
    );
};
