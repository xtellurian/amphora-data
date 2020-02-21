docker run --rm -p 8080:80 --name envisaged \                                          develop
       -v $(pwd):/visualization/git_repo:ro \
       -e GOURCE_TITLE="Amphora" \
       utensils/envisaged