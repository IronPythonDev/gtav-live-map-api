#!/bin/bash
cat ../dumps/dump.sql | docker exec -i gtav-live-map-postgresql psql -U postgres