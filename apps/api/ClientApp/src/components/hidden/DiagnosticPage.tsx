import * as React from 'react'
import {useAmphoraClients} from 'react-amphora'

export const DiagnosticPage: React.FunctionComponent = () => {
    const clients = useAmphoraClients()
    return <React.Fragment>
        {JSON.stringify(clients.axios.defaults)}
    </React.Fragment>
}