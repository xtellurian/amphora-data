#!/bin/bash

echo PersistentStores: ${PersistentStores}
echo kvUri: ${kvUri}
echo disableKv: ${disableKv}
echo BUILD_REASON: $BUILD_REASON

docker-compose run tester
