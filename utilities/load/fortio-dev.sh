#!/bin/bash

fortio load -a -c 10 -qps 100 -t 30s -labels "develop app quickstart" -data-dir ./.data "https://develop.app.amphoradata.com/quickstart"