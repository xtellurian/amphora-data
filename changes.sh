#!/bin/bash

CURRENT_BRANCH=$(git branch --show-current)
git log --graph --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Creset %s %Cgreen(%cr)%Creset' --abbrev-commit --date=relative origin/master..$CURRENT_BRANCH
