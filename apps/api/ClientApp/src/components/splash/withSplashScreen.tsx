import * as React from "react";
import { useConfigState } from "react-amphora";
import "./splash-screen.css";
import { randomQuote } from "./quotes";
import { QuoteBlock } from "./quote";

import { ParticlesBackgound } from "./particles";

function LoadingMessage(isAuthenticated: boolean) {
    return (
        <React.Fragment>
            <ParticlesBackgound />
            <div className="splash-screen">
                <QuoteBlock quote={randomQuote()} />
            </div>
        </React.Fragment>
    );
}

const withSplashScreen = (WrappedComponent: any) => {
    const LoadingScreen: React.FunctionComponent = (props) => {
        const context = useConfigState();

        const [isLoading, setIsloading] = React.useState(true);

        React.useEffect(() => {
            setTimeout(() => {
                setIsloading(false);
            }, 3000);
        }, []);

        if (isLoading) {
            return (
                <React.Fragment>
                    {LoadingMessage(context.isAuthenticated)}
                </React.Fragment>
            );
        } else {
            return <WrappedComponent {...props} />;
        }
    };

    return LoadingScreen;
};

export default withSplashScreen;
