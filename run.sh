#!/bin/bash
export BUCKET=backups
export ACCESS_KEY=SYmuS-lZ3J39PjejLCiZ
export SECRET_KEY=5yGXYIjUTw5aecYgGoBudpMs0drw26csJY39dVkR
export TELEGRAM_CONNECTION_STRING=https://integram.org/webhook/cgn2U1gAsjC
export S3_CONNECTION_STRING=http://localhost:9000
export FILE_DELETION_PERIOD_IN_DAYS=7
export DB_USER_NAME=builder
export DB_PASSWORD=builder
export HOST=localhost
export PORT=5432
export SENTRY_CONNECTION_STRING=https://8e386e8cca8c450e85edda1c46469a9a@o421804.ingest.sentry.io/5342167
export BACKUP_PERIOD_IN_MINUTES=5
export DB_IGNORE=postgres,builder,template1,template0
export PASSWORD=1234
export LOGIN=hello
dotnet run