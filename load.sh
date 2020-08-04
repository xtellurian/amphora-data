#!/bin/bash

fortio load -a -c 10 -qps 100 -H "X-Azure-ClientIP: 10.11.44.1" -t 60s "https://localhost:5001/quickstart"