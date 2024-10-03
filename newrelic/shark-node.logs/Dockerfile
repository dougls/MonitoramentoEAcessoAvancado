FROM node:18-slim

ENV NEW_RELIC_NO_CONFIG_FILE=true
ENV NEW_RELIC_DISTRIBUTED_TRACING_ENABLED=true
ENV NEW_RELIC_LOG=stdout
ENV NEW_RELIC_LICENSE_KEY="add-license-key-here"
ENV NEW_RELIC_APP_NAME="shark-node-logs"

RUN mkdir -p /home/node/app/node_modules 

WORKDIR /home/fiap/app

COPY package*.json ./

RUN npm install

COPY . .

EXPOSE 8080

CMD [ "node", "app.js" ]