import * as React from "react";
import { useConfigState } from "react-amphora";
import "./splash-screen.css";
import { randomQuote } from "./quotes";
import { QuoteBlock } from "./quote";

function LoadingMessage(isAuthenticated: boolean) {
    return (
        <div className="splash-screen">
            <div className="loading-dot">.</div>
            <QuoteBlock quote={randomQuote()} />
        </div>
    );
}

const withSplashScreen = (WrappedComponent: any) => {
    const LoadingScreen: React.FunctionComponent = (props) => {
        const context = useConfigState();

        const [isLoading, setIsloading] = React.useState(true);

        React.useEffect(() => {
            setTimeout(() => {
                setIsloading(false);
            }, 2000);
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
