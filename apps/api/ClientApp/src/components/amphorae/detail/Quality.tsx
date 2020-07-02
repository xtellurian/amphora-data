import * as React from "react";
import { Quality } from "amphoradata";
import { OneAmphora } from "./props";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Header } from "./Header";
import { HarveyPane } from "../../molecules/info/HarveyPane";
import { HarveyBall, HarveyBallLevel } from "../../molecules/info/HarveyBall";
import * as axios from "axios";
import { useAmphoraClients } from "react-amphora";
import { EmptyState } from "../../molecules/empty/EmptyState";

interface QualityState {
    quality?: Quality | null;
    isLoading: boolean;
}
export const QualityPage: React.FunctionComponent<OneAmphora> = (props) => {
    const clients = useAmphoraClients();
    const [state, setState] = React.useState<QualityState>({
        isLoading: false,
    });
    const cancelToken = axios.default.CancelToken;
    const source = cancelToken.source();

    React.useEffect(() => {
        if (props.amphora.id && !state.quality && !state.isLoading) {
            setState({ isLoading: true });

            clients.amphoraeApi
                .amphoraQualityGet(props.amphora.id, undefined, {
                    cancelToken: source.token,
                })
                .then((r) => {
                    setState({ quality: r.data, isLoading: false });
                })
                .catch((e) => {
                    setState({ isLoading: false });
                    console.log(e);
                });

            return () => source.cancel("The quality component unmounted");
        }
    }, []);

    const renderHarveyBalls = (q: Quality) => {
        return (
            <React.Fragment>
                <div className="row mr-2">
                    <div className="mt-2 col-lg-4">
                        <HarveyPane title="Reliability">
                            <HarveyBall
                                level={(q.reliability as HarveyBallLevel) || 0}
                            />
                        </HarveyPane>
                    </div>
                    <div className="mt-2 col-lg-4">
                        <HarveyPane title="Accuracy">
                            <HarveyBall
                                level={(q.accuracy as HarveyBallLevel) || 0}
                            />
                        </HarveyPane>
                    </div>
                    <div className="mt-2 col-lg-4">
                        <HarveyPane title="Completeness">
                            <HarveyBall
                                level={(q.completeness as HarveyBallLevel) || 0}
                            />
                        </HarveyPane>
                    </div>
                </div>
                <div className="row mr-2">
                    <div className="mt-2 col-lg-4">
                        <HarveyPane title="Granularity">
                            <HarveyBall
                                level={(q.granularity as HarveyBallLevel) || 0}
                            />
                        </HarveyPane>
                    </div>
                    <div className="mt-2 col-lg-4"></div>
                    <div className="mt-2 col-lg-4"></div>
                </div>
            </React.Fragment>
        );
    };

    if (state.quality) {
        const q = state.quality;
        return (
            <React.Fragment>
                <Header title="Quality"></Header>
                {renderHarveyBalls(q)}
            </React.Fragment>
        );
    } else if (state.isLoading) {
        return <LoadingState />;
    } else {
        return <EmptyState>No Quality Metrics</EmptyState>;
    }
};
