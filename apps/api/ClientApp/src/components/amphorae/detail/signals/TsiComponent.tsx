import * as React from "react";
import { getData, tsiClient } from "../../../../clients/tsiClient";
import { Signal } from "amphoradata";

import "tsiclient/tsiclient.css";
import "./tsi.css";
import { LoadingState } from "../../../molecules/empty/LoadingState";

// import { data } from "./fake";

const timezone = "Local";
const lineChartOptions = {
  theme: "light",
  grid: true,
  tooltip: true,
  legend: "shown",
  // yAxisState: "stacked",
  // noAnimate: true,
  includeDots: false,
  offset: timezone,
  includeEnvelope: true,
  dateLocale: "en-AU",
  strings: { "Display Grid": "Show Table" },
};

interface TsiComponentProps {
  amphoraId: string;
  signals: Signal[];
}
interface TsiComponentState {
  lineChart?: any;
  data?: any;
}

export class TsiComponent extends React.Component<
  TsiComponentProps,
  TsiComponentState
> {
  /**
   *
   */
  constructor(props: TsiComponentProps) {
    super(props);
    this.state = {};
  }
  componentDidMount() {
    if (!this.state.lineChart) {
      const x = document.getElementById("tsichart");
      this.setState({
        lineChart: new tsiClient.ux.LineChart(x),
      });
    }
    console.log(
      `fetching ${this.props.signals.length} signals for amphora(${this.props.amphoraId}) `
    );
  }
  private fetchData() {
    console.log("Fetching data...");
    getData(this.props.amphoraId, this.props.signals, null, "")
      .then((d: any) => this.dataCallback(d))
      .catch((e) => this.handleFetchDataError(e));
  }
  private handleFetchDataError(e: any) {
    console.log("Failed to fetch data from server.");
  }

  public componentDidUpdate(prevProps: TsiComponentProps) {
    if (prevProps.signals !== this.props.signals) {
      // update the data
      this.fetchData();
    }
    if (this.state.data) {
      this.state.lineChart.render(this.state.data, lineChartOptions);
    } else {
      console.log("Refusing to render missing data.");
    }
  }

  private dataCallback(data: any) {
    this.setState({
      lineChart: this.state.lineChart,
      data,
    });
  }

  render() {
    return (
      <React.Fragment>
        {this.state.data ? null : <LoadingState />}
        <div id="tsichart"></div>
      </React.Fragment>
    );
  }
}
