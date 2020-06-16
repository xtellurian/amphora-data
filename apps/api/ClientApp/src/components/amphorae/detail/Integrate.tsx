import * as React from "react";
import { connect } from "react-redux";
import { AmphoraDetailProps, mapStateToProps } from "./props";
import { LoadingState } from "../../molecules/empty/LoadingState";
import SyntaxHighlighter from "react-syntax-highlighter";
import { docco } from "react-syntax-highlighter/dist/esm/styles/hljs";
import { Header } from "./Header";
import { ApplicationState } from "../../../redux/state";

const style: React.CSSProperties = {
    width: "75%",
};

function pythonReferenceAmphora(amphoraId: string, name: string): string {
    return `
    from amphora.client import AmphoraDataRepositoryClient, Credentials
    credentials = Credentials(username='${name}', password='YOUR_PASSWORD')
    client = AmphoraDataRepositoryClient(credentials)
    amphora = client.get_amphora('${amphoraId}')
    `;
}

function pythonDownloadFiles(): string {
    return `
    files = amphora.get_files()
    if len(files) > 0:
        file_name = files[0].name
        amphora.get_file(file_name).pull(f"/a/local/path/{file_name}")
    `;
}

function pythonToPandas(): string {
    return `
    amphora.get_signals().pull().to_pandas() df.describe()
    `;
}
const Integrate = (props: AmphoraDetailProps & { name: string }) => {
    const id = props.match.params.id;
    const amphora = props.amphora.metadata.store[id];
    if (amphora) {
        return (
            <React.Fragment>
                <Header title="Integrate (Python)" />
                <div style={style}>
                    <h5>Reference this Amphora</h5>
                    <SyntaxHighlighter language="python" style={docco}>
                        {pythonReferenceAmphora(
                            amphora.id || "AmphoraID",
                            props.name
                        )}
                    </SyntaxHighlighter>
                    <h5>Download Files</h5>
                    <SyntaxHighlighter language="python" style={docco}>
                        {pythonDownloadFiles()}
                    </SyntaxHighlighter>
                    <h5>Signals to Pandas Dataframe</h5>
                    <SyntaxHighlighter language="python" style={docco}>
                        {pythonToPandas()}
                    </SyntaxHighlighter>
                    <a
                        href="https://docs.amphoradata.com/docs/python-download-file"
                        target="_blank"
                    rel="noopener noreferrer"
                    >
                        Learn More
                    </a>
                </div>
            </React.Fragment>
        );
    } else {
        return <LoadingState />;
    }
};
function mapActualStateToProps(state: ApplicationState) {
    const common = mapStateToProps(state);
    return {
        ...common,
        name: state.oidc.user ? state.oidc.user.profile.name : "Username",
    };
}

export default connect(mapActualStateToProps, null)(Integrate);
