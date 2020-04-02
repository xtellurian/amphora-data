set -e

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

echo Parent Stack is $STACK
THIS_STACK=$STACK-$REGION
echo "Selecting stack $THIS_STACK"
pulumi stack select $THIS_STACK
