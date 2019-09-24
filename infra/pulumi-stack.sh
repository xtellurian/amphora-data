
if [ $BUILD_REASON == "PullRequest" ] ; then
  echo "Source Branch (Pull Request) is $SYSTEM_PULLREQUEST_SOURCEBRANCH"
  if [ $SYSTEM_PULLREQUEST_SOURCEBRANCH == "refs/heads/develop" ]; then
    STACK="develop"
  elif [ $SYSTEM_PULLREQUEST_SOURCEBRANCH == "refs/heads/master" ]; then
    STACK="master"
  fi
  echo "Selected Stack: $STACK"
  pulumi stack select $STACK
fi

echo "Selecting stack $STACK"
pulumi stack select $STACK