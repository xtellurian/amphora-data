#!/bin/bash

fortio load -a -c 10 -qps 100 -t 30s -labels "prod app" -data-dir ./.data "https://app.amphoradata.com/quickstart"