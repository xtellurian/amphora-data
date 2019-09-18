
# only sets stack if on develop or master
set_special_stack () {
  if [ $1 == "refs/heads/develop" ]; then
    export STACK="develop"
  elif [ $1 == "refs/heads/master" ]; then
    export STACK="master"
  fi
  echo "Selected Stack: $STACK"
  pulumi stack select $STACK
}

echo build reason is $BUILD_REASON

if [ $BUILD_REASON == "PullRequest" ] ; then
  set_special_stack $SYSTEM_PULLREQUEST_TARGETBRANCH
  echo "Previewing special target stack!"
  pulumi preview
  echo "Source Branch (Pull Request) is $SYSTEM_PULLREQUEST_SOURCEBRANCH"
  set_special_stack $SYSTEM_PULLREQUEST_SOURCEBRANCH
else
  # spin up the source branch stack
  echo "Source Branch is $BUILD_SOURCEBRANCH"
  set_special_stack $BUILD_SOURCEBRANCH
fi