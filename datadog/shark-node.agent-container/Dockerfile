FROM node:18-slim

ENV DD_API_KEY="add-license-key-here" \
    DD_AGENT_MAJOR_VERSION=7 \
    DD_SITE="us5.datadoghq.com" \
    DD_ENV="development" \
    DD_HOSTNAME=shark-node \
    DD_LOGS_INJECTION=true \
    DD_LOGS_ENABLED=true \
    DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL=true \
    DD_APM_INSTRUMENTATION_ENABLED=host \
    DD_INSTRUMENT_SERVICE_WITH_APM=true \
    DD_APM_NON_LOCAL_TRAFFIC=true \
    DD_DOGSTATSD_NON_LOCAL_TRAFFIC=true \
    DD_PROCESS_AGENT_ENABLED=true

RUN mkdir -p /home/node/app/node_modules 

WORKDIR /home/node/app

COPY package*.json ./

RUN npm install

COPY . .

RUN apt-get update && \
    apt-get install -y curl gnupg && \
    bash -c "$(curl -L https://s3.amazonaws.com/dd-agent/scripts/install_script_agent7.sh)" && \
    mv /etc/datadog-agent/security-agent.yaml.example /etc/datadog-agent/security-agent.yaml

EXPOSE 8080

CMD ["sh", "-c", "service datadog-agent start && node app.js"]