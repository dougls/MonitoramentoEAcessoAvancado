FROM node:18-slim

ARG DD_ENV

ENV DD_API_KEY="add-license-key-here" \
    DD_SERVICE="shark-node-trace" \
    DD_VERSION=1 \
    DD_ENV=$DD_ENV \
    DD_LOGS_INJECTION=true \
    DD_PROFILING_ENABLED=true \
    DD_APPSEC_ENABLED=true \
    DD_IAST_ENABLED=true \
    DD_APPSEC_SCA_ENABLED=true

RUN mkdir -p /home/node/app/node_modules 

WORKDIR /home/fiap/app

COPY package*.json ./

RUN npm install

COPY . .

EXPOSE 80

CMD [ "node", "app.js" ]