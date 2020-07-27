import * as React from "react";
import { RouteComponentProps } from "react-router-dom";
import { connect } from "react-redux";
import { User } from "oidc-client";
import { FunctionComponent, ReactElement } from "react";
import { ApplicationState } from "../redux/state";
import { Self } from "../redux/state/self";

type UserInfoProps = {
  user: User;
  self: Self;
} & RouteComponentProps<{}>;

const UserInfo: FunctionComponent<UserInfoProps> = (
  props: UserInfoProps
): ReactElement => {
  if (!props.user) {
    return (
      <div className="empty">
        <div className="empty-icon">
          <i className="icon icon-people" />
        </div>
        <p className="empty-title h5">Please login to view user information</p>
      </div>
    );
  }
  return (
    <div className="container">
      <h3>From API</h3>
      <table className="table">
        <tbody>
          {props.self.userInfo
            ? Object.keys(props.self.userInfo).map((key) => (
                <tr key={key}>
                  <td>{key}</td>
                  <td>{(props.self.userInfo as any)[key]}</td>
                </tr>
              ))
            : null}
        </tbody>
      </table>
      <hr/>
      <h3>From Token</h3>
      <table className="table">
        <tbody>
          <tr>
            <td>token_type</td>
            <td>{props.user.token_type}</td>
          </tr>
          <tr>
            <td>access_token</td>
            <td>{props.user.access_token}</td>
          </tr>
          <tr>
            <td>refresh_token</td>
            <td>{props.user.refresh_token}</td>
          </tr>
          <tr>
            <td>expires_at</td>
            <td>{props.user.expires_at}</td>
          </tr>
          <tr>
            <td>scope</td>
            <td>{props.user.scope}</td>
          </tr>
          {Object.keys(props.user.profile).map((key) => (
            <tr key={key}>
              <td>{key}</td>
              <td>{props.user.profile[key]}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

function mapStateToProps(state: ApplicationState) {
  return {
    user: state.oidc.user,
    self: state.self,
  };
}

export default connect(mapStateToProps, null)(UserInfo);
