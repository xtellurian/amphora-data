import * as React from 'react';

import userManager from '../../userManager';

export class Challenge extends React.PureComponent {

    componentDidMount() {
        userManager.signinRedirect({
            data: { path: "/" },
        });
    }
    render() {
        return (<div>
            Redirecting you to login...
        </div>)
    }
}