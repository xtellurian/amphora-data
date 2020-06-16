import * as React from "react";
import { connect } from "react-redux";
import { PrimaryButton } from "./molecules/buttons";
import { Link } from "react-router-dom";

const Home = (): React.ReactElement => (
    <div>
        <h1>Howdy!</h1>
        <p>
            We're working on this new user interface. To switch back to the old
            experience, <a href="/challenge">click here</a>
        </p>

        <img
            alt="The Amphora Data logo"
            className="img-fluid"
            src="/_content/sharedui/images/logos/amphoradata_black_rect_1.png"
        />
        <hr />
        <p>
            If you're just getting started, try seaching for weather data in
            Brisbane (it's often nice there)
        </p>
        <Link to="/search?term=Brisbane Weather">
            <PrimaryButton>Search: 'Brisbane Weather'</PrimaryButton>
        </Link>
    </div>
);

export default connect()(Home);
