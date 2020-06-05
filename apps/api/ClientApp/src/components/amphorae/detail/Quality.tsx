import * as React from "react";
import { connect } from "react-redux";
import { Quality as Q } from "amphoradata";
import { AmphoraDetailProps, mapStateToProps } from "./props";
import { LoadingState } from "../../molecules/empty/LoadingState";
import { Header } from "./Header";
import { HarveyPane } from "../../molecules/info/HarveyPane";
import { HarveyBall, HarveyBallLevel } from "../../molecules/info/HarveyBall";
import { EmptyState } from "../../molecules/empty/EmptyState";

import { amphoraApiClient } from "../../../clients/amphoraApiClient";

interface QualityState {
    quality?: Q | null;
}
class Quality extends React.PureComponent<AmphoraDetailProps, QualityState> {
    constructor(props: AmphoraDetailProps) {
        super(props);
        this.state = {};
    }

    componentDidMount() {
        const id = this.props.match.params.id;
        amphoraApiClient
            .amphoraQualityGet(id)
            .then((r) => this.setState({ quality: r.data }))
            .catch((e) => this.setState({ quality: null }));
    }
    renderQuality(): JSX.Element | undefined {
        if (this.state.quality) {
            const q = this.state.quality;
            return (
                <React.Fragment>
                    <div className="row mr-2">
                        <div className="mt-2 col-lg-4">
                            <HarveyPane title="Reliability">
                                <HarveyBall
                                    level={
                                        (q.reliability as HarveyBallLevel) || 0
                                    }
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
                                    level={
                                        (q.completeness as HarveyBallLevel) || 0
                                    }
                                />
                            </HarveyPane>
                        </div>
                    </div>
                    <div className="row mr-2">
                        <div className="mt-2 col-lg-4">
                            <HarveyPane title="Granularity">
                                <HarveyBall
                                    level={
                                        (q.granularity as HarveyBallLevel) || 0
                                    }
                                />
                            </HarveyPane>
                        </div>
                        <div className="mt-2 col-lg-4"></div>
                        <div className="mt-2 col-lg-4"></div>
                    </div>
                </React.Fragment>
            );
        }
    }
    public render() {
        const id = this.props.match.params.id;
        const amphora = this.props.amphora.metadata.store[id];
        if (amphora) {
            return (
                <React.Fragment>
                    <Header title="Quality"></Header>
                    {this.state.quality ? null : <LoadingState />}
                    {this.renderQuality()}
                </React.Fragment>
            );
        } else {
            return <LoadingState />;
        }
    }
}

export default connect(mapStateToProps, null)(Quality);
